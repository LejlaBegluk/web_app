using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Portal.Data.Entities
{
    public class PollAnswer
    {
        public Guid Id { get; set; }
        [Display(Name = "Answer")]
        [Required]
        public string Text { get; set; }
        public Guid PollId { get; set; }
        public Poll Poll { get; set; }
        public int Counter { get; set; }
    }
}
