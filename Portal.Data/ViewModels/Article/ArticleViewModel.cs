using Portal.Data.Entities;
using Portal.Data.ViewModels.Comment;
using System;
using System.Collections.Generic;
using System.Text;

namespace Portal.Data.ViewModels.Article
{
    public class ArticleViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int Likes { get; set; }
        public DateTime CreateOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool Active { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid CategoryId { get; set; }
        public Portal.Data.Entities.Category Category { get; set; }
        public User Journalist { get; set; }
        public CommentSelectViewModel Comment { get; set; }
        public ICollection<Portal.Data.Entities.ArticlePhoto> PhotoList { get; set; }
        public bool IsLogged { get; set; }
        public Portal.Data.Entities.ArticlePhoto mainPhoto { get; set; }
    }
}
