using Sales.Shared.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Sales.Shared.DTOs
{
    public class UserDTO : User
    {
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [Required(ErrorMessage = "{0} is required.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "{0} Should have between {2} y {1} characters.")]
        public string Password { get; set; } = null!;

        [Compare("Password", ErrorMessage = "Password and confirm password does not match")]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "{0} is required.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "{0} Should have between {2} y {1} characters.")]
        public string PasswordConfirm { get; set; } = null!;

    }
}
