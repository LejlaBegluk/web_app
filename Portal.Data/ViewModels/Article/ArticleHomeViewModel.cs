using System;
using System.Collections.Generic;
using System.Text;

namespace Portal.Data.ViewModels.Article
{
    public class ArticleHomeViewModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }
        public int Likes { get; set; }

        public DateTime CreateOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool Active { get; set; }
        public Guid CategoryId { get; set; }
        public Guid UserId { get; set; }
        public Portal.Data.Entities.ArticlePhoto Photo { get; set; }
    }
}
