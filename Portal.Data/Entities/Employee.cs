using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Portal.Data.Entities
{
    public class Employee
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SocialSecurityNumber { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime StartOfEmployment { get; set; }
        public DateTime EndOfEmployment { get; set; }
        public Gender Gender { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid AddressId { get; set; }
        public Address Address { get; set; }
    }
}
