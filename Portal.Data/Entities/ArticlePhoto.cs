using System;
using System.Collections.Generic;
using System.Text;

namespace Portal.Data.Entities
{
    public class ArticlePhoto
    {
         public Guid Id { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public string PublicId { get; set; }
        public bool IsMain { get; set; }
        public Guid ArticleId { get; set; }
        public Article Article { get; set; }
    }
}
