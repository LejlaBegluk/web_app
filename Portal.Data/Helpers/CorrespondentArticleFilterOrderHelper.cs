using Portal.Data.Entities;
using Portal.Data.Helpers.HelpersInterfaces;
using Portal.Data.ViewModels.CorrespondentArticle;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Data.Helpers
{
    public class CorrespondentArticleFilterOrderHelper : ICorrespondentArticleFilterOrderHelper
    {
        private readonly PortalDbContext _context;

        public CorrespondentArticleFilterOrderHelper(PortalDbContext context)
        {
            _context = context;
        }
        public async Task<IQueryable<CorrespondentArticle>> GetFilteredArticlesAsync(CorrespondentArticleIndexViewModel Params)
        {

            var Articles = _context.CorrespondentArticles.Include(a => a.User).AsQueryable();


           // var t = await _context.CorrespondentArticles.ToListAsync();
            switch (Params.OrderBy)
            {
                

                case "JournalistUserName":
                    Articles = Articles.OrderBy(a => a.User.UserName);
                    break;
                case "JournalistUserName_Desc":
                    Articles = Articles.OrderByDescending(a => a.User.UserName);
                    break;
                case "DateCreated":
                    Articles = Articles.OrderBy(a => a.CreateOn);
                    break;
                case "DateCreated_Desc":
                    Articles = Articles.OrderByDescending(a => a.CreateOn);
                    break;
                case "Content":
                    Articles = Articles.OrderBy(a => a.Content);
                    break;
                case "Content_Desc":
                    Articles = Articles.OrderByDescending(a => a.Content);
                    break;
            }
            if (Params.Search != null)
            {
                Articles = Articles.Where(a => a.Content.Contains(Params.Search) || a.Content.Contains(Params.Search));

            }

            return Articles;
        }
    }
}
