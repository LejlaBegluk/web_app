using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Portal.Data.Entities
{

    public class User : IdentityUser<Guid>
    {
        public Photo Photo { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsActive { get; set; }
        public Guid? EditorId { get; set; }
        public User Editor { get; set; }
        public Employee Employee { get; set; }
        public User UserEditor { get; set; }
        public virtual ICollection<Article> Articles { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual ICollection<Claim> Claims { get; set; }
        public virtual ICollection<Login> Logins { get; set; }
        public virtual ICollection<Token> Tokens { get; set; }


    }
}
