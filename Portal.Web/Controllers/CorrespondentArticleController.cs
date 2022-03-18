using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Portal.Data.Entities;
using Portal.Data.Helpers;
using Portal.Data.Helpers.HelpersInterfaces;
using Portal.Data.ViewModels;
using Portal.Data.ViewModels.CorrespondentArticle;
using Portal.Persistance.Repositories.Interfaces;
using Portal.Web.Extensions.Alerts;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Portal.Web.Controllers
{
    public class CorrespondentArticleController : Controller
    {
        private readonly PortalDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUnitOfWork _unitOfWork;
          private readonly ICorrespondentArticleFilterOrderHelper _articleFilterOrderHelper;
        private Cloudinary _cloudinary;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        public CorrespondentArticleController(PortalDbContext context, UserManager<User> userManager, IAuthorizationService authorizationService, IUnitOfWork unitOfWork
                        , IOptions<CloudinarySettings> cloudinaryConfig, ICorrespondentArticleFilterOrderHelper articleFilterOrderHelper)
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
        [Authorize(Policy = "JournalistOnly")]
        public async Task<IActionResult> Index([FromQuery] int? pageNumber, [FromQuery] int? pageSize, [FromQuery] string filter, [FromQuery] string Search, [FromQuery] string orderBy)
        {
            var conditions = new CorrespondentArticleIndexViewModel
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
        private async Task<PagedList<CorrespondentArticleViewModel>> GetArticlesView(IQueryable<CorrespondentArticle> source, int pageNumber, int pageSize)
        {
            var sourceView = source.Select(a => new CorrespondentArticleViewModel
            {
                Id = a.Id,
                Active = a.Active,
                Journalist = a.User,
                 CreateOn=a.CreateOn
            });
            return await PagedList<CorrespondentArticleViewModel>.CreateAsync(sourceView, pageNumber, pageSize);
        }
        public IActionResult Create()
        {
            AddCorrespondentArticleViewModel model = new AddCorrespondentArticleViewModel();


            return PartialView("Create", model);
        }
        [HttpPost]
        public async Task <IActionResult> Create(AddCorrespondentArticleViewModel model)
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            CorrespondentArticle NewArticle = new CorrespondentArticle
            {
                Id = new Guid(),
                Active = false,
                Content = model.Content,
                CreateOn = DateTime.Now,
                UserId = Guid.Parse(user)
            };
            _unitOfWork.CorrespondentArticles.Add(NewArticle);
            await _unitOfWork.CompleteAsync();

            if (model.Photos != null && model.Photos.Count() > 0)
            {
                // Loop thru each selected file
                foreach (IFormFile photo in model.Photos)
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
                        CorrespondentArticlePhoto Articlephoto = new CorrespondentArticlePhoto()
                        {
                            Id = Guid.NewGuid(),
                             CorrespondentArticleId = NewArticle.Id,
                            DateAdded = DateTime.Now,      
                            PublicId = uploadResult.PublicId,
                            Url = uploadResult.SecureUri.ToString()
                        };

                        _unitOfWork.CorrespondentArticlePhotos.Add(Articlephoto);
                        var result = await _unitOfWork.CompleteAsync();
                    }
                }
            }


            return RedirectToAction("Index", "Home").WithSuccess("Message:", "Thank you for sending us information!"); ;
        }
        [Authorize(Policy = "JournalistOnly")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = _context.CorrespondentArticles.Include(x => x.User).Include(x => x.ArticlePhotos).Where(x => x.Id == id).FirstOrDefault();
                //await _unitOfWork.CorrespondentArticles.GetAll().Include(a => a.ArticlePhotos).Include(a => a.User).FirstOrDefaultAsync(a => a.Id.Equals(id));
            if (article == null)
            {
                return NotFound();
            }
                var Articleuser = _context.Users.Where(u=>u.Id==article.UserId).FirstOrDefault();
                //await _userManager.Users.FirstOrDefaultAsync(u => u.Id.Equals(article.User.Id));
                var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            AddCorrespondentArticleViewModel model = new AddCorrespondentArticleViewModel()
                {
                    Id = article.Id,
                    Active = article.Active,
                    Content = article.Content,
                    CreateOn = article.CreateOn,
                    User = article.User,
                    UserId = article.UserId,
                    PhotoList = _context.CorrespondentArticlePhotos.Where(x => x.CorrespondentArticleId == article.Id).ToList()
            };
                return View(model);
   
        }
    }
}
