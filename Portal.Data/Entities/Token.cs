using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Portal.Data.Entities
{
    public class Token : IdentityUserToken<Guid>
    {
        public virtual User User { get; set; }

    }
}
