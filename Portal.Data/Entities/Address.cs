using System;
using System.Collections.Generic;
using System.Text;

namespace Portal.Data.Entities
{
    public class Address
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid CountryId { get; set; }
        public Country Country { get; set; }
        public Employee Employee { get; set; }
    }
}
