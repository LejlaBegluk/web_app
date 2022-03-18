using Microsoft.AspNetCore.Http;
using Portal.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Portal.Data.ViewModels.CorrespondentArticle
{
    public class AddCorrespondentArticleViewModel
    {
        public Guid Id { get; set; }
        [Required]
        public string Content { get; set; }
        public int Likes { get; set; }
        [Display(Name = "Created date")]
        public DateTime CreateOn { get; set; }
        public bool Active { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }
        [Display(Name = "Photo")]
        public List<IFormFile> Photos { get; set; }
        public ICollection<Portal.Data.Entities.CorrespondentArticlePhoto> PhotoList { get; set; }
    }
}
