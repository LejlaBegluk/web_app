using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portal.Web.Services
{
    public interface IPhotoService
    {
        public Task<bool> AddPhotoForUser(Guid id, IFormFile file);
        public Task<bool> DeletePhotoForUser(Guid id);
    }
}
