using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Portal.Data.Entities;
using Portal.Data.ViewModels.Comment;

namespace Portal.Web.Controllers
{
    public class CommentsController : Controller
    {
        #region Constructor and parameters
        private readonly PortalDbContext _context;

        public CommentsController(PortalDbContext context)
        {
            _context = context;
        }
        #endregion


        #region Create
        public IActionResult Create(Guid ArticleID)
        {
            CommentViewModel model = new CommentViewModel()
            {
                ArticleId = ArticleID

            };
            return View(model);
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(string Content, string ArticleID)
        {
            bool success = false;
            string message = string.Empty;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ModelState.IsValid)
            {
                try
                {
                    if (!String.IsNullOrEmpty(Content)) { 
                   
                    Comment saveItem = new Comment()
                    {
                        ArticleId = Guid.Parse(ArticleID.ToString()),
                        Content = Content.Trim(),
                        CreateOn = DateTime.Now,
                        Id = new Guid(),
                        UserId = Guid.Parse(userId)
                    };

                    var x = _context.Comments.Add(saveItem);
                    await _context.SaveChangesAsync();
                        if (x !=null)
                        {
                            success = true;
                        }
                    }
                    else
                    {
                        return RedirectToAction("Details", "Articles", new { id = ArticleID });
                    }
                  
                }



                catch (Exception ex)
                {
                    success = false;
                    message = ex.Message;
                }
            
        }
            else
            {
                success = false;
            }
            var Comments = await _context.Comments.Include(c => c.User).Where(c => c.ArticleId.Equals(ArticleID)).ToListAsync();
            CommentSelectViewModel selectViewModel = new CommentSelectViewModel()
            {
                ArticleID = Guid.Parse(ArticleID.ToString()),
                IsLogged = userId == null ? false : true,
                Comments = Comments.Select(c => new CommentViewModel()
                {
                    ArticleId = c.ArticleId,
                    Content = c.Content,
                    CanDelete = c.UserId == Guid.Parse(userId) ? true : false,
                    UserId = (Guid)c.UserId,
                    CreateOn = c.CreateOn,
                    Id = c.Id
                })
            };

            return Json(selectViewModel);
        }

        #endregion

        #region Delete
        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var comment = await _context.Comments.FindAsync(id);
            var ArticleID = comment.ArticleId;
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Articles", new { id = ArticleID });
        }
        #endregion
        
    }
}
