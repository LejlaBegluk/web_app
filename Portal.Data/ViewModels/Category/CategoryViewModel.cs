using Portal.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Portal.Data.ViewModels.Category
{
    public class CategoryViewModel
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        public bool Active { get; set; }
        public DateTime CreateOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsEditor { get; set; }

        public virtual ICollection<Portal.Data.Entities.Article> Articles { get; set; }
    }
}
