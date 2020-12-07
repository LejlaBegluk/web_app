using Portal.Data.Helpers;
using Wangkanai.Detection;

namespace Portal.Data.ViewModels
{
    public class UserManagemenViewModel
    {
        private const int MaxPageSize = 50;
        public int CurrentPage { get; set; } = 1;
        private int pageSize = 10;

        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = (value > MaxPageSize ? MaxPageSize : value); }
        }
        public PagedList<UserViewModel> Users;
        public string Filter { get; set; }
        public string Search { get; set; }
        public string OrderBy { get; set; }
        public IDevice FromDevice { get; set; }
    }
}
