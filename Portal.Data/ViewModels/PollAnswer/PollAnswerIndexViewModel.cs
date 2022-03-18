using System;
using System.Collections.Generic;
using System.Text;

namespace Portal.Data.ViewModels.PollAnswer
{
    public class PollAnswerIndexViewModel
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public Guid PollId { get; set; }
        public int Counter { get; set; }
        public decimal Percentage { get; set; }

    }
}
