using Portal.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Portal.Data.ViewModels.CorrespondentArticle
{
    public class CorrespondentArticleViewModel
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime CreateOn { get; set; }
        public bool Active { get; set; }
        public Guid UserId { get; set; }
        public ICollection<Portal.Data.Entities.ArticlePhoto> PhotoList { get; set; }
        public Portal.Data.Entities.ArticlePhoto mainPhoto { get; set; }
        public User Journalist { get; set; }
    }
}
