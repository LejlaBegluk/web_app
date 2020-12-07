using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Portal.Data.Entities;
using Portal.Data.Helpers.HelpersInterfaces;
using Portal.Data.ViewModels.Article;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Data.Helpers
{
   public class ArticleFilterOrderHelper: IArticleFilterOrderHelper
    {
        private readonly PortalDbContext _context;

        public ArticleFilterOrderHelper(PortalDbContext context)
        {
            _context = context;
        }
        public async Task<IQueryable<Article>> GetFilteredArticlesAsync(ArticleIndexViewModel Params)
        {
   
           var Articles = _context.Articles.Include(a => a.User).Include(a => a.Category).AsQueryable();
    
             
            var t =await _context.Articles.ToListAsync();
            switch (Params.OrderBy)
            {
                case "Title":
                    Articles = Articles.OrderBy(a => a.Title);
                    break;
                case "Title_Desc":
                    Articles = Articles.OrderByDescending(a => a.Title);
                    break;
                case "CategoryName":
                    Articles = Articles.OrderBy(a => a.Category.Name);
                    break;
                case "CategoryName_Desc":
                    Articles = Articles.OrderByDescending(a => a.Category.Name);
                    break;
                case "Likes":
                    Articles = Articles.OrderBy(a => a.Likes);
                    break;
                case "Likes_Desc":
                    Articles = Articles.OrderByDescending(a => a.Likes);
                    break;

                case "JournalistUserName":
                    Articles = Articles.OrderBy(a => a.User.UserName);
                    break;
                case "JournalistUserName_Desc":
                    Articles = Articles.OrderByDescending(a=>a.User.UserName);
                    break;
                case "DateCreated":
                    Articles = Articles.OrderBy(a => a.CreateOn);
                    break;
                case "DateCreated_Desc":
                    Articles = Articles.OrderByDescending(a => a.CreateOn);
                    break;
                case "Active":
                    Articles = Articles.OrderBy(a => a.Active);
                    break;
                case "Active_Desc":
                    Articles = Articles.OrderByDescending(a => a.Active);
                    break;
            }
            if (Params.Search != null)
            {
                Articles = Articles.Where(a => a.Title.Contains(Params.Search) || a.Content.Contains(Params.Search));
               
            }

            return Articles;
        }
    }
}
