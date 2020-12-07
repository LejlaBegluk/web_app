using System;
using System.Collections.Generic;
using System.Text;

namespace Portal.Data.Entities
{
    public class Country
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
