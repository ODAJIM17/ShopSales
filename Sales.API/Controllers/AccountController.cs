using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Sales.API.Helpers;
using Sales.Shared.DTOs;
using Sales.Shared.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Sales.API.Controllers
{
   
    [ApiController]
    [Route("api/accounts")]
    public class AccountController : ControllerBase
    {
        private readonly IUserHelper _userHelper;
        private readonly IConfiguration _configuration;
        private readonly IFileStorage _fileStorage;
        private readonly IMailHelper _mailHelper;
        private readonly string _container;

        public AccountController(IUserHelper userHelper, IConfiguration configuration, 
            IFileStorage fileStorage, IMailHelper mailHelper)
        {
            _userHelper = userHelper;
            _configuration = configuration;
            _fileStorage = fileStorage;
            _mailHelper = mailHelper;
            _container = "users";
        }

        [HttpPost("RecoverPassword")]
        public async Task<ActionResult> RecoverPassword([FromBody] EmailDTO model)
        {
            User user = await _userHelper.GetUserAsync(model.Email);
            if (user == null)
            {
                return NotFound();
            }

            var myToken = await _userHelper.GeneratePasswordResetTokenAsync(user);
            var tokenLink = Url.Action("ResetPassword", "account", new
            {
                userid = user.Id,
                token = myToken
            }, HttpContext.Request.Scheme, _configuration["UrlWEB"]);

            var response = _mailHelper.SendMail(user.FullName, user.Email!,
                $"Sales - Recover Password",
                $"<h1>Sales -  Recover Password</h1>" +
                $"<p>Para recuperar su contraseña, por favor hacer click 'Recover Password':</p>" +
                $"<b><a class='btn btn-sm btn-primary' href ={tokenLink}>Recover Password</a></b>");

            if (response.IsSuccess)
            {
                return NoContent();
            }

            return BadRequest(response.Message);
        }


        [HttpPost("ResetPassword")]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordDTO model)
        {
            User user = await _userHelper.GetUserAsync(model.Email);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userHelper.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                return NoContent();
            }

            return BadRequest(result.Errors.FirstOrDefault()!.Description);
        }



        [HttpPost("changePassword")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> ChangePasswordAsync(ChangePasswordDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userHelper.GetUserAsync(User.Identity!.Name!);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userHelper.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.FirstOrDefault()?.Description);
            }

            return NoContent();
        }


        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Put(User user)
        {
            try
            {
                if (!string.IsNullOrEmpty(user.Photo))
                {
                    var photoUser = Convert.FromBase64String(user.Photo);
                    user.Photo = await _fileStorage.SaveFileAsync(photoUser, ".jpg", _container);
                }

                var currentUser = await _userHelper.GetUserAsync(user.Email!);
                if (currentUser == null)
                {
                    return NotFound();
                }

                currentUser.Document = user.Document;
                currentUser.FirstName = user.FirstName;
                currentUser.LastName = user.LastName;
                currentUser.Address = user.Address;
                currentUser.PhoneNumber = user.PhoneNumber;
                currentUser.Photo = !string.IsNullOrEmpty(user.Photo) && user.Photo != currentUser.Photo ? user.Photo : currentUser.Photo;
                currentUser.CityId = user.CityId;

                var result = await _userHelper.UpdateUserAsync(currentUser);
                if (result.Succeeded)
                {
                    return Ok(BuildToken(currentUser));
                }

                return BadRequest(result.Errors.FirstOrDefault());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Get()
        {
            return Ok(await _userHelper.GetUserAsync(User.Identity!.Name!));
        }



        [HttpPost("CreateUser")]
        public async Task<ActionResult> CreateUser([FromBody] UserDTO model)
        {
            if (!string.IsNullOrEmpty(model.Photo))
            {
                var photoUser = Convert.FromBase64String(model.Photo);
                model.Photo = await _fileStorage.SaveFileAsync(photoUser, ".jpg", _container);
            }

            User user = model;
            var result = await _userHelper.AddUserAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userHelper.AddUserToRoleAsync(user, user.UserType.ToString());
                var myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                var tokenLink = Url.Action("ConfirmEmail", "Account", new
                {
                    userid = user.Id,
                    token = myToken
                }, HttpContext.Request.Scheme, _configuration["UrlWEB"]);

                var response = _mailHelper.SendMail(user.FullName, user.Email!,
                    $"Sales- Account Confirmation",
                    $"<h1>Sales - Email Confirmation</h1>" +
                    $"<p>Thank you for registering with the Shop-Sales Online customer store. In order to complete your registration please, activate your account by clicking 'Confirm Email':</p>" +
                    $"<b><a href ={tokenLink}>Confirm Email</a></b>");

                if (response.IsSuccess)
                {
                    return NoContent();
                }

                return BadRequest(response.Message);
            }

            return BadRequest(result.Errors.FirstOrDefault());
        }


        [HttpPost("ResedToken")]
        public async Task<ActionResult> ResendToken([FromBody] EmailDTO model)
        {
            User user = await _userHelper.GetUserAsync(model.Email);
            if (user == null)
            {
                return NotFound();
            }

            //TODO Improve code
            var myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
            var tokenLink = Url.Action("ConfirmEmail", "account", new
            {
                userid = user.Id,
                token = myToken
            }, HttpContext.Request.Scheme, _configuration["UrlWEB"]);

            var response = _mailHelper.SendMail(user.FullName, user.Email!,
                 $"Sales- Account Confirmation",
                $"<h1>Sales - Email Confirmation</h1>" +
                  $"<p>Thank you for registering with the Shop-Sales Online customer store. In order to complete your registration please, activate your account by clicking 'Confirm Email':</p>" +
                $"<b><a href ={tokenLink}>Confirm Email</a></b>");

            if (response.IsSuccess)
            {
                return NoContent();
            }

            return BadRequest(response.Message);
        }



        [HttpGet("ConfirmEmail")]
        public async Task<ActionResult> ConfirmEmailAsync(string userId, string token)
        {
            token = token.Replace(" ", "+");
            var user = await _userHelper.GetUserAsync(new Guid(userId));
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userHelper.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.FirstOrDefault());
            }

            return NoContent();
        }


        [HttpPost("Login")]
        public async Task<ActionResult> Login([FromBody] LoginDTO model)
        {
            var result = await _userHelper.LoginAsync(model);
            if (result.Succeeded)
            {
                var user = await _userHelper.GetUserAsync(model.Email);
                return Ok(BuildToken(user));
            }

            if (result.IsLockedOut)
            {
                return BadRequest("To many tries your account has been blocked. Please try again in 5 minutes.");
            }

            if (result.IsNotAllowed)
            {
                //return BadRequest("Your Email is not confirmed. Please check your email and follow the instructions.");
                return BadRequest("The email address has not been verified. Upon registration an email was sent to you with instructions to verify your email address. Click the Resend Email Button to resend the email.");
            }


            return BadRequest("Invalid email or password.");
        }



        private TokenDTO BuildToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email!),
                new Claim(ClaimTypes.Role, user.UserType.ToString()),
                new Claim("Document", user.Document),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName),
                new Claim("Address", user.Address),
                new Claim("Photo", user.Photo ?? string.Empty),
                new Claim("CityId", user.CityId.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwtKey"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddDays(30);
            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials);

            return new TokenDTO
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };
        }
    }

}

