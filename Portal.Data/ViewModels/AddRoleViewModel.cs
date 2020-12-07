using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Portal.Data.ViewModels
{
    public class AddRoleViewModel
    {
        [Required]
        [Display(Name = "Role name")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "User name must contain upper case (A-Z) and lower case (a-z) characters!")]
        public string RoleName { get; set; }
    }
}
