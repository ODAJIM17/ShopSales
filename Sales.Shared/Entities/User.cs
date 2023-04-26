﻿using Microsoft.AspNetCore.Identity;
using Sales.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Sales.Shared.Entities
{
    public class User : IdentityUser
    {
        [Display(Name = "Document")]
        [MaxLength(20, ErrorMessage = "{0} Allows max {1} characters.")]
        [Required(ErrorMessage = "{0} is required")]
        public string Document { get; set; } = null!;

        [Display(Name = "First Name")]
        [MaxLength(50, ErrorMessage = "{0} Allows max {1} characters.")]
        [Required(ErrorMessage = "{0} is required")]
        public string FirstName { get; set; } = null!;

        [Display(Name = "Last Name")]
        [MaxLength(50, ErrorMessage = "{0} Allows max {1} characters.")]
        [Required(ErrorMessage = "{0} is required")]
        public string LastName { get; set; } = null!;

        [Display(Name = "Address")]
        [MaxLength(200, ErrorMessage = "{0} Allows max {1} characters.")]
        [Required(ErrorMessage = "{0} is required")]
        public string Address { get; set; } = null!;

        [Display(Name = "Photo")]
        public string? Photo { get; set; }

        [Display(Name = "User Type")]
        public UserType UserType { get; set; }

        public City? City { get; set; }

        [Display(Name = "City")]
        [Range(1, int.MaxValue, ErrorMessage = "Select a {0}.")]
        public int CityId { get; set; }

        [Display(Name = "User Name")]
        public string FullName => $"{FirstName} {LastName}";

        public ICollection<TemporalSale>? TemporalSales { get; set; }
        public ICollection<Sale>? Sales { get; set; }
    }

}

