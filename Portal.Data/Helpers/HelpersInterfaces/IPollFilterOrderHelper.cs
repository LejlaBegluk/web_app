using Portal.Data.Entities;
using Portal.Data.ViewModels.Poll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Data.Helpers.HelpersInterfaces
{
    public interface IPollFilterOrderHelper
    {
        Task<IQueryable<Poll>> GetFilteredPollAsync(PollIndexViewModel Params);
    }
}
