using System;

namespace Portal.Data.ViewModels
{
    public class UserViewModel
    {

        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Url { get; set; }
        public bool IsActive { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

    }
}
