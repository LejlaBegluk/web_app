using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Portal.Data.Entities;
using Portal.Data.Helpers;
using Portal.Data.Helpers.HelpersInterfaces;
using Portal.Data.ViewModels.Category;
using Portal.Persistance.Repositories.Interfaces;
using Portal.Web.Extensions.Alerts;

namespace Portal.Web.Controllers
{
    public class CategoriesController : Controller
    {
        #region Constructor and parameters
        private readonly PortalDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICategoryFilterOrderHelper _categoryFilterOrderHelper;
     
        public CategoriesController(UserManager<User> userManager,PortalDbContext context, IUnitOfWork unitOfWork
                                    ,ICategoryFilterOrderHelper categoryFilterOrderHelper)
        {
             _context = context;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _categoryFilterOrderHelper = categoryFilterOrderHelper;
        }
        #endregion

        #region Index
        [Authorize(Policy = "JournalistOnly")]
        public async Task<IActionResult> Index([FromQuery]int? pageNumber, [FromQuery]int? pageSize, [FromQuery]string filter, [FromQuery] string Search, [FromQuery]string orderBy)
        {
            var conditions = new CategoryIndexViewModel
            {
                CurrentPage = pageNumber ?? 1,
                PageSize = pageSize ?? 10,
                Filter = filter ?? "Category",
                Search = Search,
                OrderBy = orderBy ?? "Title"//,

                //  FromDevice = _deviceResolver.Device
            };
            var categories = await _categoryFilterOrderHelper.GetFilteredCategoryAsync(conditions);
            conditions.categories = await GetCategoryView(categories, conditions.CurrentPage, conditions.PageSize);
            ViewBag.Number = (conditions.CurrentPage * conditions.PageSize) - 10;

            
            return View(conditions);
        }
        private async Task<PagedList<CategoryViewModel>> GetCategoryView(IQueryable<Category> source, int pageNumber, int pageSize)
        {
            var sourceView = source.Select(c => new CategoryViewModel
            {
                Id = c.Id,
                Active =c.Active,
                Name =c.Name,
                CreateOn = c.CreateOn,
                UpdatedOn=c.UpdatedOn
            });
            return await PagedList<CategoryViewModel>.CreateAsync(sourceView, pageNumber, pageSize);
        }
        #endregion

        #region Create
        [Authorize(Policy = "JournalistOnly")]

        public async Task<IActionResult> Create()
        {
            var users = await _userManager.GetUsersInRoleAsync("Editor");
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var IsInListUser = users.Where(u=>u.Id==Guid.Parse(user)).FirstOrDefault();
            CategoryViewModel model = new CategoryViewModel()
            {
                IsEditor = IsInListUser!=null ? true : false,
            };

            return View(model);
        }
        [Authorize(Policy = "JournalistOnly")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                Category category = new Category()
                {
                    Name = model.Name,
                    Active = model.Active,
                    CreateOn = DateTime.Now,
                    Id = Guid.NewGuid()
                };
                _unitOfWork.Categories.Add(category);
                await _unitOfWork.CompleteAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
        #endregion

        #region Edit
        [Authorize(Policy = "JournalistOnly")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var users = await _userManager.GetUsersInRoleAsync("Editor");
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var IsInListUser = users.Where(u => u.Id == Guid.Parse(user)).FirstOrDefault();

            var category = await _unitOfWork.Categories.GetAsync((Guid)id);
            CategoryViewModel model = new CategoryViewModel()
            {
                Id = category.Id,
                Name = category.Name,
                CreateOn = category.CreateOn,
                UpdatedOn = category.UpdatedOn,
                IsEditor = IsInListUser != null ? true : false,
                Active=category.Active
            };
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Policy = "JournalistOnly")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Category category = new Category()
                    {
                        Name = model.Name,
                        Active = model.Active,
                        CreateOn = model.CreateOn,
                        Id = model.Id,
                        UpdatedOn = DateTime.Now
                    };
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {

                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
        #endregion

        #region Delete
        [Authorize(Policy = "JournalistOnly")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var category = await _unitOfWork.Categories.GetAsync(id);
            var articles = _unitOfWork.Articles.GetAll().Where(a => a.Category.Id == id).ToList();
            if (articles.Any())
            {
                return RedirectToAction(nameof(Index)).WithWarning("Message:", "There are active articles under this category!");
            }
            if (category == null)
            {
                ModelState.AddModelError("", "This category can't be found.");
            }
            if(category!= null && !articles.Any()) { 
            _unitOfWork.Categories.Remove(category);
            await _unitOfWork.CompleteAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        #endregion

    }
}
