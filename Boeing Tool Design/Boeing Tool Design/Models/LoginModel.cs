using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace Boeing_Tool_Design.Models
{

    public class LoginModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required.")]
        [Display(Name = "Email")]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
