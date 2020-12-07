using Portal.Data.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using Wangkanai.Detection;

namespace Portal.Data.ViewModels.Category
{
    public class CategoryIndexViewModel
    {
        private const int MaxPageSize = 50;
        public int CurrentPage { get; set; } = 1;
        private int pageSize = 10;

        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = (value > MaxPageSize ? MaxPageSize : value); }
        }
        public PagedList<CategoryViewModel> categories;
        public string Filter { get; set; }
        public string Search { get; set; }
        public string OrderBy { get; set; }
        public IDevice FromDevice { get; set; }
    }
}
