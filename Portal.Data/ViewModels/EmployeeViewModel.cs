using Portal.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Portal.Data.ViewModels
{
    public class EmployeeViewModel
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "User name is required!")]
        [Display(Name = "User name")]
        [RegularExpression(@"^[a-z0-9]+$", ErrorMessage = "User name must contain  lower case (a-z) and number (0-9) characters!")]
        [StringLength(18, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required!")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])",
            ErrorMessage = "The email address is not entered in a correct format (example.example@mail.example.example.com)")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [StringLength(18, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [RegularExpression(@"^((?=.*[a-z])(?=.*[A-Z])(?=.*\d)).+$", ErrorMessage = "Passwords must be at least 8 characters and contain at 3 of 4 of the following: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*)")]

        public string Password { get; set; }
        [Required(ErrorMessage = "Phone number is required!")]
        [Display(Name = "Phone number")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Phone number is not valid! (000 000 0000)")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Activity is required!")]
        public bool IsActive { get; set; }
        public Photo Photo { get; set; }
        //Data for Employee
        [Required(ErrorMessage = "Please choose employee type!")]
        [Display(Name = "Employe type")]
        public string EmployeeType { get; set; }
        public List<string> EmployeeTypes { get; set; }
        [Display(Name = "Editor")]
        public Guid EditorId { get; set; }
        public List<User> Editors { get; set; }
        public List<Gender> Genders { get; set; } = new List<Gender> { Gender.Male, Gender.Female, Gender.Other };
        public List<Country> Countries { get; set; }
        [Required(ErrorMessage = "First name is required!")]
        [Display(Name = "First name")]
        [RegularExpression("^([a-zA-Z .&'-]+)$", ErrorMessage = "First name is not valid!")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last name is required!")]
        [Display(Name = "Last name")]
        [RegularExpression("^([a-zA-Z .&'-]+)$", ErrorMessage = "Last name is not valid!")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Social Security Number is required.")]
        [RegularExpression(@"^\d{9}|\d{3}-\d{2}-\d{4}$", ErrorMessage = "Invalid Social Security Number")]
        [Display(Name = "Social security number")]
        public string SocialSecurityNumber { get; set; }
        [Required(ErrorMessage = "Birthdate is required.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{dd-MM-yyyy}")]
        [Display(Name = "Birthdate")]
        public DateTime BirthDate { get; set; }
        [Required(ErrorMessage = "Start of employment date is required.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{dd-MM-yyyy}")]
        [Display(Name = "Start of employment")]
        public DateTime StartOfEmployment { get; set; }
        [Required(ErrorMessage = "End of employment date is required.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{dd-MM-yyyy}")]
        [Display(Name = "End of employment")]
        public DateTime EndOfEmployment { get; set; }
        [Required(ErrorMessage = "Gender is required.")]
        public Gender Gender { get; set; }
        [Required(ErrorMessage = "Address is required.")]
        [RegularExpression(@"^[A-Za-z0-9]+(?:\s[A-Za-z0-9'_-]+)+$", ErrorMessage = "Invalid address.")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Country is required.")]
        [Display(Name = "Country")]
        public Guid CountryId { get; set; }
    }


}
//[EnumDataType(typeof(Gender))]