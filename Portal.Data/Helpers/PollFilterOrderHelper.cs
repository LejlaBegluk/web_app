using Microsoft.EntityFrameworkCore;
using Portal.Data.Entities;
using Portal.Data.Helpers.HelpersInterfaces;
using Portal.Data.ViewModels.Poll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Data.Helpers
{
    public class PollFilterOrderHelper : IPollFilterOrderHelper
    {
        private readonly PortalDbContext _context;

        public PollFilterOrderHelper(PortalDbContext context)
        {
            _context = context;
        }
        public async Task<IQueryable<Poll>> GetFilteredPollAsync(PollIndexViewModel Params)
        {

            var polls = _context.Polls.AsQueryable();


            var t = await _context.Polls.ToListAsync();
            switch (Params.OrderBy)
            {
                case "Question":
                    polls = polls.OrderBy(a => a.Question);
                    break;
                case "Question_Desc":
                    polls = polls.OrderByDescending(a => a.Question);
                    break;
                case "DateCreated":
                    polls = polls.OrderBy(a => a.CreateDate);
                    break;
                case "DateCreated_Desc":
                    polls = polls.OrderByDescending(a => a.CreateDate);
                    break;
                case "Active":
                    polls = polls.OrderBy(a => a.Active);
                    break;
                case "Active_Desc":
                    polls = polls.OrderByDescending(a => a.Active);
                    break;
            }
            if (Params.Search != null)
            {
                polls = polls.Where(a => a.Question.Contains(Params.Search));

            }

            return polls;
        }
    }
}
