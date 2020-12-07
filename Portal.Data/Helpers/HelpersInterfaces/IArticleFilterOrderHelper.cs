using Portal.Data.Entities;
using Portal.Data.ViewModels.Article;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Data.Helpers.HelpersInterfaces
{
    public interface IArticleFilterOrderHelper
    {
        Task<IQueryable<Article>> GetFilteredArticlesAsync(ArticleIndexViewModel Params);
    }
}
