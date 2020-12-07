using Microsoft.AspNetCore.Http;
using Portal.Data.Entities;
using Portal.Data.ViewModels.ArticlePhoto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Portal.Data.ViewModels.Article
{
    public class ArticleAddEditViewModel
    {
        public Guid Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        public int Likes { get; set; }
        [Display(Name = "Created date")]
        public DateTime CreateOn { get; set; }
        [Display(Name = "Last update date")]
        public DateTime UpdatedOn { get; set; }
        public bool Active { get; set; }
        [Display(Name = "Category")]
        [Required]
        public Guid CategoryId { get; set; }
        public List<Portal.Data.Entities.Category> Categories { get; set; }
        public bool IsEditor { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid MainPhoto { get; set; }
        [Display(Name = "Photo")]
        public List<IFormFile> Photos { get; set; }
        public ICollection<Portal.Data.Entities.ArticlePhoto> PhotoList { get; set; }

    }
}
