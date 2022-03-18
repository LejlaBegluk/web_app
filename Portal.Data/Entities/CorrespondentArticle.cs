using System;
using System.Collections.Generic;
using System.Text;

namespace Portal.Data.Entities
{
    public class CorrespondentArticle
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime CreateOn { get; set; }
        public bool Active { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public virtual ICollection<CorrespondentArticlePhoto> ArticlePhotos { get; set; }
    }
}
