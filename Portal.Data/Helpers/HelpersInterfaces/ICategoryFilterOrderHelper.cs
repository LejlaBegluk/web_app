using Portal.Data.Entities;
using Portal.Data.ViewModels.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Data.Helpers.HelpersInterfaces
{
    public interface ICategoryFilterOrderHelper
    {
        Task<IQueryable<Category>> GetFilteredCategoryAsync(CategoryIndexViewModel Params);
    }
}
