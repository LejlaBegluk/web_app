using System;
using System.Collections.Generic;
using System.Text;

namespace Portal.Data.ViewModels.Article
{
    public class ArticleHomeListViewModel
    {
        public List<ArticleHomeViewModel> News { get; set; }
        public List<ArticleHomeViewModel> Buisness { get; set; }
        public List<ArticleHomeViewModel> Sport { get; set; }
        public string SelectedList { get; set; }

    }
}
