using Portal.Data.ViewModels.PollAnswer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Portal.Data.ViewModels.Poll
{
    public class PollDetailsViewModel
    {
        public Guid Id { get; set; }
        public string Question { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public Guid UserId { get; set; }
        public bool Active { get; set; }
        public bool IsEditor { get; set; }
        public List<PollAnswerIndexViewModel> PollAnswers { get; set; }
    }
}
