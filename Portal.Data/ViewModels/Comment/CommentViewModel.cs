using Portal.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Portal.Data.ViewModels.Comment
{
    public class CommentViewModel
    {
        public Guid Id {get; set; }
        [Display(Name = "Comment")]
        public string Content { get; set; }
        public Guid? UserId { get; set; }
        public DateTime CreateOn { get; set; }
        public Guid ArticleId { get; set; }
        public bool CanDelete { get; set; }
        public string UserName { get; set; }

    }
}
