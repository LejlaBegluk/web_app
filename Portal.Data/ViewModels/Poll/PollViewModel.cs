using Portal.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Portal.Data.ViewModels.Poll
{
    public class PollViewModel
    {
        public Guid Id { get; set; }
        [Display(Name = "Question")]
        [Required]
        public string Question { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public Guid UserId { get; set; }
        public bool Active { get; set; }
        public bool IsEditor { get; set; }
        public virtual ICollection<Portal.Data.Entities.PollAnswer> PollAnswers { get; set; }
    }
}
