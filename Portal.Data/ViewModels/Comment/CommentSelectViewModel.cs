using System;
using System.Collections.Generic;
using System.Text;

namespace Portal.Data.ViewModels.Comment
{
    public class CommentSelectViewModel
    {
        public Guid ArticleID { get; set; }
        public bool IsLogged { get; set; }
        public IEnumerable<CommentViewModel> Comments { get; set; }
    }
}
