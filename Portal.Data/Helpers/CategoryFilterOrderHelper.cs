using Microsoft.EntityFrameworkCore;
using Portal.Data.Entities;
using Portal.Data.Helpers.HelpersInterfaces;
using Portal.Data.ViewModels.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Data.Helpers
{
  public  class CategoryFilterOrderHelper:ICategoryFilterOrderHelper
    {
        private readonly PortalDbContext _context;

        public CategoryFilterOrderHelper(PortalDbContext context)
        {
            _context = context;
        }
        public async Task<IQueryable<Category>> GetFilteredCategoryAsync(CategoryIndexViewModel Params)
        {

            var categories = _context.Categories.AsQueryable();


            var t = await _context.Categories.ToListAsync();
            switch (Params.OrderBy)
            {
                case "Name":
                    categories = categories.OrderBy(a => a.Name);
                    break;
                case "Name_Desc":
                    categories = categories.OrderByDescending(a => a.Name);
                    break;
                case "DateCreated":
                    categories = categories.OrderBy(a => a.CreateOn);
                    break;
                case "DateCreated_Desc":
                    categories = categories.OrderByDescending(a => a.CreateOn);
                    break;
                case "Active":
                    categories = categories.OrderBy(a => a.Active);
                    break;
                case "Active_Desc":
                    categories = categories.OrderByDescending(a => a.Active);
                    break;
            }
            if (Params.Search != null)
            {
                categories = categories.Where(a => a.Name.Contains(Params.Search));

            }

            return categories;
        }
    }
}
