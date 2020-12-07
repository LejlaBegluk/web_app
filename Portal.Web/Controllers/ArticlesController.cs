using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Portal.Data.Entities;
using Portal.Data.Helpers;
using Portal.Data.Helpers.HelpersInterfaces;
using Portal.Data.ViewModels.Article;
using Portal.Data.ViewModels.Comment;
using Portal.Persistance.Repositories.Interfaces;
using Portal.Web.Extensions.Alerts;

namespace Portal.Web.Controllers
{
    //[Authorize]
    public class ArticlesController : Controller
    {
        #region Constructor and properties
        private readonly PortalDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IArticleFilterOrderHelper _articleFilterOrderHelper;
        private Cloudinary _cloudinary;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        public ArticlesController(PortalDbContext context, UserManager<User> userManager, IAuthorizationService authorizationService, IUnitOfWork unitOfWork
                        ,IOptions<CloudinarySettings> cloudinaryConfig , IArticleFilterOrderHelper articleFilterOrderHelper)
        {
            _context = context;
            _userManager = userManager;
            _authorizationService = authorizationService;
            _unitOfWork = unitOfWork;
            _articleFilterOrderHelper = articleFilterOrderHelper;
            _cloudinaryConfig = cloudinaryConfig;
            Account acc = new Account(
              _cloudinaryConfig.Value.CloudName,
              _cloudinaryConfig.Value.ApiKey,
              _cloudinaryConfig.Value.ApiSecret
          );
            _cloudinary = new Cloudinary(acc);
        }
        #endregion

        #region Index
        [Authorize(Policy = "JournalistOnly")]
        public async Task<IActionResult> Index([FromQuery]int? pageNumber, [FromQuery]int? pageSize, [FromQuery]string filter, [FromQuery] string Search, [FromQuery]string orderBy)
        {
            var conditions = new ArticleIndexViewModel
            {
                CurrentPage = pageNumber ?? 1,
                PageSize = pageSize ?? 10,
                Filter = filter ?? "Article",
                Search = Search,
                OrderBy = orderBy ?? "DateCreated_Desc"//,

                //  FromDevice = _deviceResolver.Device
            };
            var articles = await _articleFilterOrderHelper.GetFilteredArticlesAsync(conditions);
            conditions.Articles = await GetArticlesView(articles, conditions.CurrentPage, conditions.PageSize);
            ViewBag.Number = (conditions.CurrentPage * conditions.PageSize) - 10;


            return View(conditions);
        }
        public async Task<IActionResult> Search([FromQuery]int? pageNumber, [FromQuery]int? pageSize, [FromQuery]string filter, [FromQuery] string Search, [FromQuery]string orderBy)
        {
            var conditions = new ArticleIndexViewModel
            {
                CurrentPage = pageNumber ?? 1,
                PageSize = pageSize ?? 10,
                Filter = filter ?? "Article",
                Search = Search,
                OrderBy = orderBy ?? "DateCreated_Desc"//,

                //  FromDevice = _deviceResolver.Device
            };
            var articles = await _articleFilterOrderHelper.GetFilteredArticlesAsync(conditions);
            conditions.Articles = await GetArticlesView(articles, conditions.CurrentPage, conditions.PageSize);
            ViewBag.Number = (conditions.CurrentPage * conditions.PageSize) - 10;
            foreach(var item in conditions.Articles)
            {
                item.mainPhoto= _unitOfWork.ArticlePhotos.GetAll().Where(p => p.ArticleId == item.Id).Take(1).FirstOrDefault();
            }
            return View(conditions);
        }
        private async Task<PagedList<ArticleViewModel>> GetArticlesView(IQueryable<Article> source, int pageNumber, int pageSize)
        {
            var sourceView = source.Select(a => new ArticleViewModel
            {
                Id = a.Id,
               Active = a.Active,
               Category = a.Category,
              CreateOn = a.CreateOn,
              Likes = a.Likes,
              Title = a.Title,
              Journalist = a.User
            });
            return await PagedList<ArticleViewModel>.CreateAsync(sourceView, pageNumber, pageSize);
        }
        #endregion

        #region Create
        // GET: Articles/Create
        [Authorize(Policy = "JournalistOnly")]
        public IActionResult Create()
        {
            ArticleAddEditViewModel model = new ArticleAddEditViewModel()
            {
                Categories = _unitOfWork.Categories.GetAll().Where(c => c.Active == true).ToList()
            };
            return View(model);
        }
        [Authorize(Policy = "JournalistOnly")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm]ArticleAddEditViewModel article)
        {

            if (ModelState.IsValid)
            {
                var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Article NewArticle = new Article
                {
                    Id = new Guid(),
                    Active = false,
                    CategoryId = article.CategoryId,
                    Content = article.Content,
                    CreateOn = DateTime.Now,
                    Title = article.Title,
                    Likes = 0,
                    UserId = Guid.Parse(user)
                };
                _unitOfWork.Articles.Add(NewArticle);
               await _unitOfWork.CompleteAsync();

                if (article.Photos != null && article.Photos.Count() > 0)
                {
                    // Loop thru each selected file
                    foreach (IFormFile photo in article.Photos)
                    {
                        var uploadResult = new ImageUploadResult();
                        if (photo.Length > 0 && photo.ContentType.Contains("image"))
                        {
                            using (var stream = photo.OpenReadStream())
                            {
                                var uploadParams = new ImageUploadParams()
                                {
                                    File = new FileDescription(photo.Name, stream),
                                    Transformation = new Transformation()
                                    .Width(800).Height(800)//.Crop("fill").Gravity("face")
                                };
                                uploadResult = _cloudinary.Upload(uploadParams);
                            }
                            ArticlePhoto Articlephoto = new ArticlePhoto()
                            {
                                Id = Guid.NewGuid(),
                                ArticleId = NewArticle.Id,
                                DateAdded = DateTime.Now,
                                //Description = uploadResult.Description,
                                PublicId = uploadResult.PublicId,
                                Url = uploadResult.SecureUri.ToString()
                        };

                            _unitOfWork.ArticlePhotos.Add(Articlephoto);
                            var result = await _unitOfWork.CompleteAsync();
                        }
                    }
                }
                        return RedirectToAction(nameof(Index));
            }
            article.Categories = _unitOfWork.Categories.GetAll().Where(c => c.Active == true).ToList();
            return View(article);
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

            var article = await _unitOfWork.Articles.GetAll().Include(a => a.ArticlePhotos).Include(a => a.User).FirstOrDefaultAsync(a => a.Id.Equals(id));
            if (article == null)
            {
                return NotFound();
            }
            //For this authentication handler always pass article with user included
            if ((await _authorizationService
              .AuthorizeAsync(User, article, "ArticleOwnerOnly")).Succeeded)
            {
                // var roles = await _userManager.GetUsersInRoleAsync("Editor");
                var Articleuser = await _userManager.Users.FirstOrDefaultAsync(u => u.Id.Equals(article.User.Id));
                var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
                ArticleAddEditViewModel model = new ArticleAddEditViewModel()
                {
                    Id = article.Id,
                    Categories = _unitOfWork.Categories.GetAll().ToList(),
                    Active = article.Active,
                    Likes = article.Likes,
                    Content = article.Content,
                    Title = article.Title,
                    CategoryId = article.CategoryId,
                    CreateOn = article.CreateOn,
                    UpdatedOn = article.UpdatedOn,
                    IsEditor =Guid.Parse(user)== Articleuser.EditorId?true:false,
                    User = article.User,
                    UserId=article.UserId,
                    PhotoList = article.ArticlePhotos
                };
                return View(model); 
            }
            else
                return RedirectToAction("AccessDenied", "Account");
        }

        // POST: Articles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Policy = "JournalistOnly")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ArticleAddEditViewModel article)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (ModelState.IsValid)
            {
                try
                {
                    Article SaveArticle = new Article()
                    {
                        Active = article.Active,
                        CategoryId = article.CategoryId,
                        Content = article.Content,
                        CreateOn = article.CreateOn,
                        Id = article.Id,
                        Likes = article.Likes,
                        Title = article.Title,
                        UpdatedOn = DateTime.Now,
                        User = await _userManager.FindByIdAsync(article.UserId.ToString()) //this is not required UserId is, User.FindFirstValue(ClaimTypes.NameIdentifier); will give you id of current user
                    };
                    if (article.Photos != null)
                    {
                        if (article.Photos != null && article.Photos.Count() > 0)
                        {
                            // Loop thru each selected file
                            foreach (IFormFile photo in article.Photos)
                            {
                                var uploadResult = new ImageUploadResult();
                                if (photo.Length > 0 && photo.ContentType.Contains("image"))
                                {
                                    using (var stream = photo.OpenReadStream())
                                    {
                                        var uploadParams = new ImageUploadParams()
                                        {
                                            File = new FileDescription(photo.Name, stream),
                                            Transformation = new Transformation()
                                            .Width(800).Height(800)//.Crop("fill").Gravity("face")
                                        };
                                        uploadResult = _cloudinary.Upload(uploadParams);
                                    }
                                    ArticlePhoto Articlephoto = new ArticlePhoto()
                                    {
                                        Id = Guid.NewGuid(),
                                        ArticleId = article.Id,
                                        DateAdded = DateTime.Now,
                                        //Description = uploadResult.Description,
                                        PublicId = uploadResult.PublicId,
                                        Url = uploadResult.SecureUri.ToString()
                                    };

                                    _unitOfWork.ArticlePhotos.Add(Articlephoto);
                                    var result = await _unitOfWork.CompleteAsync();
                                }
                            }
                        }
                        }
                    _context.Update(SaveArticle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticleExists(article.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Edit),new { id=article.Id}).WithSuccess("Message:", "Article was edited!");
            }
            article.Categories = _unitOfWork.Categories.GetAll().ToList();
            return View(article).WithDanger("Message:", "Changes were not applied!"); 
        }
        #endregion

        #region Delete
        // POST: Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var article = await _unitOfWork.Articles.GetAll().Include(a=>a.User).Include(a => a.ArticlePhotos).FirstOrDefaultAsync(a => a.Id.Equals(id));
            if (article == null)
            {
                return RedirectToAction(nameof(Index)).WithDanger("Message:", "Something went wrong!");
            }
            if ((await _authorizationService
              .AuthorizeAsync(User, article, "ArticleOwnerOnly")).Succeeded)
            {
                if (article.ArticlePhotos.Count > 0 && article.ArticlePhotos != null)
                {
                    foreach (var item in article.ArticlePhotos)
                    {
                        var result = new ArticlePhotoController(_context, _unitOfWork, _cloudinaryConfig, _userManager).DeleteArticlePhoto(item.Id);
                    }

                }
                _unitOfWork.Articles.Remove(article);
                await _unitOfWork.CompleteAsync();
                return RedirectToAction(nameof(Index)).WithSuccess("Message:", "Article was deleted!");
            }
            else
                return RedirectToAction(nameof(Index)).WithWarning("Message:", "You don't have permissions for this action!");

        }
        #endregion

        #region AdditionalMethods
        private bool ArticleExists(Guid id)
        {
            return _context.Articles.Any(e => e.Id == id);
        }
        #endregion

        #region Details
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var article = await _unitOfWork.Articles.GetAll().Include(a => a.ArticlePhotos).Include(a=>a.Comments).Include(a=>a.User).ThenInclude(u=>u.Employee).FirstOrDefaultAsync(a => a.Id.Equals(id));

            var Comments = _context.Comments.Include(c=>c.User).Where(c => c.ArticleId == id).ToList();
            var u = userId == null ? Guid.Empty : Guid.Parse(userId);
            CommentSelectViewModel selectViewModel = new CommentSelectViewModel()
            {
                ArticleID = Guid.Parse(id.ToString()),
                IsLogged = userId == null ? false : true,
                Comments = Comments.Select(c => new CommentViewModel()
                {
                    ArticleId = c.ArticleId,
                    Content = c.Content,
                    CanDelete = c.UserId == u || article.UserId == u ? true : false,
                    UserId = (Guid)c.UserId,
                    UserName=c.User.UserName,
                    CreateOn = c.CreateOn,
                    Id = c.Id
                })
            };


            ArticleViewModel model = new ArticleViewModel()
            {
                CategoryId = article.CategoryId,
                Comment= selectViewModel,
                Content = article.Content,//.Replace(Environment.NewLine, " <br /> "),
                CreateOn = article.CreateOn,
                Id = article.Id,
                Journalist = article.User,
                Title = article.Title,
                PhotoList = article.ArticlePhotos,
                IsLogged = userId == null ? false : true,
                 Likes=article.Likes
            };
            return View(model);
        }
        #endregion

        #region Like 
        public  async Task<IActionResult> Like(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article =  _context.Articles.Where(a => a.Id.Equals(id)).FirstOrDefault();
            if (article == null)
            {
                return NotFound();

            }
            else
            {
                article.Likes += 1;
                _context.Update(article);
                await _context.SaveChangesAsync();
            }
            return Ok(article);
        }
        #endregion
    }
}
