using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Portal.Data.Entities;
using Portal.Data.Helpers;
using Portal.Data.ViewModels;
using Portal.Data.ViewModels.ArticlePhoto;
using Portal.Persistance.Repositories.Interfaces;
using Portal.Web.Extensions.Alerts;

namespace Portal.Web.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ArticlePhotoController : ControllerBase
    {
        #region Constructor and parameters
        // private readonly PortalDbContext _context;

        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private readonly PortalDbContext _context;
        private Cloudinary _cloudinary;


        public ArticlePhotoController(PortalDbContext context, IUnitOfWork unitOfWork,
         IOptions<CloudinarySettings> cloudinaryConfig,
         UserManager<User> userManager)
        {
            _cloudinaryConfig = cloudinaryConfig;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _context = context;
            Account acc = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(acc);
        }
        #endregion

        #region add photo
        // GET: ArticlePhotoes
        [HttpPost]
        [Route("{id}")]
        public async Task<IActionResult> AddPhotoForArticle(Guid articleID, [FromForm] ArticlePhotoCreationViewModel PhotoCreation)
        {
            var Article = await _unitOfWork.Articles.GetAsync(articleID);
            if (Article.UserId != Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            var file = PhotoCreation.File;
            var uploadResults = new ImageUploadResult();
            if (file.Length > 0 && file.ContentType.Contains("image"))
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };
                    uploadResults = _cloudinary.Upload(uploadParams);

                }
            }
            else return BadRequest("File format incorect. Please send image!");
            PhotoCreation.Url = uploadResults.Uri.ToString();
            PhotoCreation.PublicId = uploadResults.PublicId;

            ArticlePhoto photo = new ArticlePhoto()
            {
                Id = Guid.NewGuid(),
                ArticleId = articleID,
                DateAdded = PhotoCreation.DateAdded,
                Description = PhotoCreation.Description,
                PublicId = PhotoCreation.PublicId,
                Url = PhotoCreation.Url
            };
            //if (!Article.ArticlePhotos.Any(u=>u.IsMain))
            //{
            // photo.IsMain = true;
            //}
            _unitOfWork.ArticlePhotos.Add(photo);
            var result = await _unitOfWork.CompleteAsync();
            if (result > 0)
                return Ok(photo.Id);//createdatroute()
            return BadRequest("Could not add the photo!");
        }
        #endregion

        #region delete photo
        public async Task<IActionResult> DeleteArticlePhoto(Guid id)
        {
            if (id == null)
                return BadRequest("Could not delete the photo!"); 

            var photo = await _unitOfWork.ArticlePhotos.GetAsync(id);
            if (photo == null)
                return BadRequest("Could not delete the photo!");

            var ArticleId = photo.ArticleId;

            if (photo.PublicId != null)
            {
                var deleteParams = new DeletionParams(photo.PublicId);
                var result = _cloudinary.Destroy(deleteParams);
                if (result.Result == "ok")
                {
                    _unitOfWork.ArticlePhotos.Remove(photo);
                }
            }
            if (photo.PublicId == null)
            {
                _unitOfWork.ArticlePhotos.Remove(photo);
            }

            if (await _unitOfWork.CompleteAsync() > 0)
            {
                return RedirectToAction("Edit", "Articles", new { id = ArticleId }).WithSuccess("Message:", "Photo was deleted!");
            }
            return BadRequest("Could not delete the photo!");
        }
        #endregion

        #region update photo
        public async Task<int> UpdatePhotoForArticle( ArticlePhoto item)
        {
            ArticlePhoto photo = new ArticlePhoto()
            {
                ArticleId = item.ArticleId,
                DateAdded = item.DateAdded,
                Id = item.Id,
                Description = item.Description,
                IsMain = item.IsMain,
                PublicId = item.PublicId,
                Url = item.Url
            };
           _context.ArticlePhotos.Update(photo);
            var x=await _context.SaveChangesAsync();
            return x;
        }
        #endregion
    }
}
