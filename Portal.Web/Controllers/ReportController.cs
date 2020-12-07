using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.Reporting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portal.Data.Entities;
using Portal.Data.ViewModels.Report;

namespace Portal.Web.Controllers
{
    public class ReportController : Controller
    {
        #region Constructor and properties
        private readonly PortalDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly LocalReport _localReport;

        public ReportController( PortalDbContext context, UserManager<User> userManager,LocalReport localReport)
        {
            _context = context;
            _userManager = userManager;
            _localReport = localReport;
        }
        #endregion

        #region Print
        [Authorize(Policy = "EditorOnly")]
        public async Task<IActionResult> Print(ReportUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var dt = new DataTable();
                dt.Columns.Add("Comments");
                dt.Columns.Add("Title");
                dt.Columns.Add("Likes");
                dt.Columns.Add("CreateOn");
                dt.Columns.Add("Category");
                dt.Columns.Add("Journalist");
                dt.Columns.Add("Editor");

                var list = _context.Articles
                    .Include(a => a.Comments)
                    .Include(a => a.Category)
                     .Include(a => a.User)
                     .ThenInclude(u => u.Employee)
                    .Where(a => a.UserId == model.Id)
                    .ToList();
                DataRow row;
                foreach (var item in list)
                {
                    //item.Comments = _context.Comments.Where(c => c.ArticleId == item.Id).ToList();
                    row = dt.NewRow();
                    item.User.Editor = _context.Users.Include(u => u.Employee).Where(u => u.Id == item.User.EditorId).FirstOrDefault();
                    row["Comments"] = item.Comments.Count();
                    row["Title"] = item.Title;
                    row["Likes"] = item.Likes;
                    row["CreateOn"] = item.CreateOn;
                    row["Journalist"] = item.User.Employee.FirstName + " " + item.User.Employee.LastName;
                    row["Editor"] = item.User.Editor != null ? item.User.Editor.Employee.FirstName + " " + item.User.Editor.Employee.LastName : "";
                    row["Category"] = item.Category.Name;

                    dt.Rows.Add(row);

                }
                var employee = _context.Users.Include(u => u.Employee).Where(u => u.Id == model.Id).FirstOrDefault();
                string parameter = employee.Employee.FirstName + " " + employee.Employee.LastName;
                string mimetype = "";
                int extension = 1;
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("prm", parameter);
                _localReport.AddDataSource("dsEmployee", dt);
                var result = _localReport.Execute(RenderType.Pdf, extension, parameters, mimetype);
                return File(result.MainStream, "application/pdf");
            }
            var users = await _userManager.GetUsersInRoleAsync("Journalist");
            model.Users = users.ToList();
            return View("Report", model);
        }
        #endregion

        #region Get Report 
        [Authorize(Policy = "EditorOnly")]
        public async Task<IActionResult> Report()
        {
            var users = await _userManager.GetUsersInRoleAsync("Journalist");
            ReportUserViewModel model = new ReportUserViewModel();
            model.Users = users.ToList();
            return View(model);
        }
        #endregion
    }
}