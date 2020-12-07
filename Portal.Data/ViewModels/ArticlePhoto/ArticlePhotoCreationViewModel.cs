using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Portal.Data.ViewModels.ArticlePhoto
{
    public class ArticlePhotoCreationViewModel
    {
        public string Url { get; set; }
        public IFormFile File { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public string PublicId { get; set; }
       public ArticlePhotoCreationViewModel()
        {
            DateAdded = DateTime.Now;
        }
    }
}
