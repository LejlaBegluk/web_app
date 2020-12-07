using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Portal.Data.Entities;
using Portal.Data.Helpers;
using Portal.Data.ViewModels;
using Portal.Persistance.Repositories.Interfaces;
using System;
using System.Threading.Tasks;

namespace Portal.Web.Services
{
    public class PhotoService : IPhotoService
    {
        private Cloudinary _cloudinary;

        private readonly IUnitOfWork _unitOfWork;

        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        public PhotoService(IOptions<CloudinarySettings> cloudinaryConfig,
            IUnitOfWork unitOfWork)
        {
            _cloudinaryConfig = cloudinaryConfig;
            _unitOfWork = unitOfWork;
            Account acc = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(acc);
        }

        public async Task<bool> AddPhotoForUser(Guid id, IFormFile file)
        {

            //var IFormFile Photo
            var photoForCreationDto = new UserPhotoCreateViewModel()
            {
                DateAdded = DateTime.Now
            };

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
            else return false;
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
                return true;
            return false;
        }
        public async Task<bool> DeletePhotoForUser(Guid id)
        {
            if (id == null)
                return false;

            var photo = await _unitOfWork.UserPhotos.GetAsync(id);
            if (photo == null)
                return false;

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
                return true;
            }
            return false;
        }

    }

}
