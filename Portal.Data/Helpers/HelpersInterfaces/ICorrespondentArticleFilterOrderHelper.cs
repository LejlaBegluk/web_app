using Portal.Data.Entities;
using Portal.Data.ViewModels.CorrespondentArticle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Data.Helpers.HelpersInterfaces
{
    public interface ICorrespondentArticleFilterOrderHelper
    {
        Task<IQueryable<CorrespondentArticle>> GetFilteredArticlesAsync(CorrespondentArticleIndexViewModel Params);
    }
}
