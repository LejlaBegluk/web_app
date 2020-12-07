using Portal.Data.Entities;
using System;
using System.Collections.Generic;

namespace Portal.Data.ViewModels
{
    public class UserRoleViewModel
    {
        public UserRoleViewModel()
        {
            Users = new List<UserInfo>();
        }
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
        public List<UserInfo> Users { get; set; }

    }
    public class UserInfo
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
    }
}
