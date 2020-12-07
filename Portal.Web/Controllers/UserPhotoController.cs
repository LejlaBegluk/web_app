using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
using Portal.Persistance.Repositories.Interfaces;

namespace Portal.Web.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class UserPhotoController : ControllerBase
    {



        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;

        public UserPhotoController(IUnitOfWork unitOfWork,
         IOptions<CloudinarySettings> cloudinaryConfig,
         UserManager<User> userManager)
        {
            _cloudinaryConfig = cloudinaryConfig;
            _unitOfWork = unitOfWork;
            _userManager = userManager;

            Account acc = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(acc);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetPhoto(Guid id)
        {
            var photoFromRepo = await _unitOfWork.UserPhotos.GetAsync(id);
            var photo = new UserPhotoViewModel()
            {
                Id = photoFromRepo.Id,
                Url = photoFromRepo.Url,
                Description = photoFromRepo.Description,
                DateAdded = photoFromRepo.DateAdded,
                PublicId = photoFromRepo.PublicId
            };
            return Ok(photo);
        }

        [HttpPost]
        [Route("{id}")]
        public async Task<IActionResult> AddPhotoForPortalUser([FromRoute]Guid id,
        [FromForm]UserPhotoCreateViewModel photoForCreationDto)
        {

            var photos = _unitOfWork.UserPhotos
                .GetAll()
                .Where(p => p.UserId.Equals(id.ToString()))
                .ToList();
            if (photos.Count() > 0)
                foreach (var img in photos)
                {
                    await DeletePhotoForPortalUser(img.Id);
                }
            photoForCreationDto.DateAdded = DateTime.Now;
            var file = photoForCreationDto.File;
            var uploadResult = new ImageUploadResult();
            if (file.Length > 0 && file.ContentType.Contains("image"))
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation()
                        .Width(500).Height(500).Crop("fill").Gravity("face")
                    };
                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }
            else return BadRequest("File format incorect. Please send image!");
            photoForCreationDto.Url = uploadResult.SecureUri.ToString();
            photoForCreationDto.PublicId = uploadResult.PublicId;
            var photo = new Photo()
            {
                Id = Guid.NewGuid(),
                Url = photoForCreationDto.Url,
                Description = photoForCreationDto.Description,
                DateAdded = photoForCreationDto.DateAdded,
                PublicId = photoForCreationDto.PublicId,
                UserId = id


            };

            _unitOfWork.UserPhotos.Add(photo);
            var result = await _unitOfWork.CompleteAsync();
            if (result > 0)
                return Ok(photo.Id);
            return BadRequest("Could not add the photo!");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhotoForPortalUser(Guid id)
        {
            if (id == null)
                return BadRequest("Invalid photo id!");

            var photo = await _unitOfWork.UserPhotos.GetAsync(id);
            if (photo == null)
                return BadRequest("Photo doese not exist!");

            if (photo.PublicId != null)
            {
                var deleteParams = new DeletionParams(photo.PublicId);
                var result = _cloudinary.Destroy(deleteParams);
                if (result.Result == "ok")
                {
                    _unitOfWork.UserPhotos.Remove(photo);
                }
            }
            if (photo.PublicId == null)
            {
                _unitOfWork.UserPhotos.Remove(photo);
            }

            if (await _unitOfWork.CompleteAsync() > 0)
            {
                return Ok(photo.Id);
            }
            return BadRequest("Faild to delete the photo!");
        }

    }
}