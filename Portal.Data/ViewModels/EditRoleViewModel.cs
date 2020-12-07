using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Portal.Data.ViewModels
{
    public class EditRoleViewModel
    {
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "Role name")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "User name must contain upper case (A-Z) and lower case (a-z) characters!")]
        public string RoleName { get; set; }

        public List<UserInfo> Users { get; set; }
    }
}
