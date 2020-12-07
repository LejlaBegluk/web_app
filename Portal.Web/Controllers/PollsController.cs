using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Portal.Data.Entities;
using Portal.Data.Helpers;
using Portal.Data.Helpers.HelpersInterfaces;
using Portal.Data.ViewModels.Poll;
using Portal.Persistance.Repositories.Interfaces;
using Portal.Web.Extensions.Alerts;

namespace Portal.Web.Controllers
{
    public class PollsController : Controller
    {
        #region Constructor and parameters
        private readonly PortalDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPollFilterOrderHelper _pollFilterOrderHelper;
        public PollsController(UserManager<User> userManager, PortalDbContext context, IUnitOfWork unitOfWork
                                    , IPollFilterOrderHelper pollFilterOrderHelper)
        {
            _context = context;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _pollFilterOrderHelper = pollFilterOrderHelper;
        }
        #endregion

        #region Index
        // GET: Polls
        [Authorize(Policy = "JournalistOnly")]
        public async Task<IActionResult> Index([FromQuery]int? pageNumber, [FromQuery]int? pageSize, [FromQuery]string filter, [FromQuery] string Search, [FromQuery]string orderBy)
        {
            var conditions = new PollIndexViewModel
            {
                CurrentPage = pageNumber ?? 1,
                PageSize = pageSize ?? 10,
                Filter = filter ?? "Category",
                Search = Search,
                OrderBy = orderBy ?? "Title"//,

                //  FromDevice = _deviceResolver.Device
            };
            var polls = await _pollFilterOrderHelper.GetFilteredPollAsync(conditions);
            conditions.polls = await GetPollView(polls, conditions.CurrentPage, conditions.PageSize);
            ViewBag.Number = (conditions.CurrentPage * conditions.PageSize) - 10;


            return View(conditions);
        }
        private async Task<PagedList<PollViewModel>> GetPollView(IQueryable<Poll> source, int pageNumber, int pageSize)
        {
            var sourceView = source.Select(p => new PollViewModel
            {
                Id = p.Id,
                Active = p.Active,
                Question = p.Question,
                CreateDate = p.CreateDate,
                UpdateDate = p.UpdateDate
            });
            return await PagedList<PollViewModel>.CreateAsync(sourceView, pageNumber, pageSize);
        }
        #endregion

        #region Display answer functions
        // GET: Polls/Details/5
        public async Task<IActionResult> Details(bool answered=false)
        {
            var poll = _context.Polls.Where(p=>p.Active==true).FirstOrDefault();
            poll.PollAnswers = await _context.PollAnswer.Where(a => a.PollId == poll.Id).ToListAsync();
            if (answered)
            {
                ViewBag.QuestionAnswered = "Yes";
            }
            else
            {
                ViewBag.QuestionAnswered = "No";
            }
            return PartialView(poll);
        }
        [HttpPost]
        public async Task<IActionResult> Answer(Guid Answered)
        {
            var answer = _context.PollAnswer.Find(Answered);
            if (answer != null)
            {
                answer.Counter++;
                _context.PollAnswer.Update(answer);
                await _context.SaveChangesAsync();
               
            }
            return RedirectToAction("Details",new { answered =true});
        }

        #endregion

        #region Create
        // GET: Polls/Create
        [Authorize(Policy = "JournalistOnly")]
        public IActionResult Create()
        {
            return View(new PollViewModel());
        }

        [Authorize(Policy = "JournalistOnly")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm]PollViewModel poll)
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ModelState.IsValid)
            {

                Poll newItem = new Poll()
                {
                    Id = Guid.NewGuid(),
                    CreateDate = DateTime.Now,
                    Question = poll.Question,
                    UserId = Guid.Parse(user),
                    Active=poll.Active
                };
                if (newItem.Active == true)
                {
                    var CurrentActivePoll = _context.Polls.Where(p => p.Active == true).FirstOrDefault();
                    if (CurrentActivePoll != null)
                    {
                        CurrentActivePoll.Active = false;
                        _context.Polls.Update(CurrentActivePoll);
                    }
                  
                }
                _context.Add(newItem);
                await _context.SaveChangesAsync();
                return RedirectToAction("Edit","Polls",new { id= newItem.Id});
            }
            return View(poll);
        }
        #endregion

        #region Edit
        [Authorize(Policy = "JournalistOnly")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var poll = await _unitOfWork.Polls.GetAll().Include(a => a.User).FirstOrDefaultAsync(a => a.Id.Equals(id));
            if (poll == null)
            {
                return NotFound();
            }
            ViewBag.Number = 0;
            var Polluser = await _userManager.Users.FirstOrDefaultAsync(u => u.Id.Equals(poll.User.Id));
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            PollViewModel model = new PollViewModel()
            {
                Id = poll.Id,
                Question = poll.Question,
                CreateDate = poll.CreateDate,
                Active = poll.Active,
                PollAnswers = await _context.PollAnswer.Where(pa => pa.PollId == poll.Id).ToListAsync(),
                UpdateDate = DateTime.Now,
                UserId = poll.UserId,
                IsEditor= Guid.Parse(user) ==Polluser.EditorId ? true : false,
            };

            return View(model);
        }

        // POST: Polls/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Policy = "JournalistOnly")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Question,CreateDate,UserId,Active")] Poll poll)
        {
            if (id != poll.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (poll.Active == true)
                    {
                        var CurrentActivePoll = _context.Polls.Where(p => p.Active == true).FirstOrDefault();
                        if (CurrentActivePoll != null) { 
                        CurrentActivePoll.Active = false;
                        _context.Polls.Update(CurrentActivePoll);
                        }
                    }
                    _context.Update(poll);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PollExists(poll.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", poll.UserId);
            return View(poll);
        }
        private bool PollExists(Guid id)
        {
            return _context.Polls.Any(e => e.Id == id);
        }
        #endregion

        #region Delete
        [Authorize(Policy = "JournalistOnly")]
        // GET: Polls/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var poll = await _context.Polls
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (poll == null)
            {
                return NotFound();
            }

            return View(poll);
        }

        // POST: Polls/Delete/5
        [Authorize(Policy = "JournalistOnly")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var poll = await _context.Polls.FindAsync(id);
            _context.Polls.Remove(poll);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        #endregion
       
    }
}
