using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Portal.Data.Entities
{
    public class Poll
    {
        public Guid Id { get; set; }
        public string Question { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public bool Active { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }
        public virtual ICollection<PollAnswer> PollAnswers { get; set; }

    }
}
