using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portal.Data.Entities;
using Portal.Persistance.Repositories.Interfaces;
using Portal.Web.Extensions.Alerts;

namespace Portal.Web.Controllers
{

    public class PollAnswersController : Controller
    {
        #region Constructor and parameters
        private readonly PortalDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        public PollAnswersController(UserManager<User> userManager, PortalDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region Index
        public IActionResult Index()
        {
            return View();
        }
        #endregion

        #region Create
        public IActionResult Create(Guid pollID)
        {
            PollAnswer model = new PollAnswer()
            {
                PollId = pollID,
                Counter = 0

            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm]PollAnswer answer)
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ModelState.IsValid)
            {

                PollAnswer newItem = new PollAnswer()
                {
                    Id = Guid.NewGuid(),
                    PollId = answer.PollId,
                    Counter = answer.Counter,
                    Text = answer.Text
                };
                _context.Add(newItem);
                await _context.SaveChangesAsync();
                return RedirectToAction("Edit", "Polls", new { id = newItem.PollId }).WithSuccess("Message:", "Answer was added!");
            }
            return View(answer);
        }
        #endregion

        #region Delete
        // POST: Polls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var answer = await _context.PollAnswer.FindAsync(id);
            var pollId = answer.PollId;
            _context.PollAnswer.Remove(answer);
            await _context.SaveChangesAsync();
            return RedirectToAction("Edit","Polls",new { id= pollId }).WithSuccess("Message:", "Answer was deleted!"); 
        }
        #endregion


    }
}