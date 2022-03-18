using System;
using System.Collections.Generic;
using System.Text;

namespace Portal.Data.Entities
{
    public class CorrespondentArticlePhoto
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public string PublicId { get; set; }
        public Guid CorrespondentArticleId { get; set; }
        public CorrespondentArticle CorrespondentArticle { get; set; }
    }
}
