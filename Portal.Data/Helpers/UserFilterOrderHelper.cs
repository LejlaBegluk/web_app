using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Portal.Data.Entities;
using Portal.Data.Helpers.HelpersInterfaces;
using Portal.Data.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Portal.Data.Helpers
{
    public class UserFilterOrderHelper : IUserFilterOrderHelper
    {
        private readonly UserManager<User> _userManager;

        public UserFilterOrderHelper(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        public async Task<IQueryable<User>> GetFilteredUsersAsync(UserManagemenViewModel userParams)
        {
            var users = _userManager.Users.Include(u => u.Photo).Include(u => u.Employee).AsQueryable();


            switch (userParams.Filter)
            {
                case "Administrator":
                    var admins = await _userManager.GetUsersInRoleAsync(userParams.Filter);
                    var adminsIds = admins.Select(a => a.Id);
                    users = users.Where(u => adminsIds.Contains(u.Id));
                    break;
                case "Journalist":
                    var journalist = await _userManager.GetUsersInRoleAsync(userParams.Filter);
                    var journalistIds = journalist.Select(a => a.Id).ToList();
                    users = users.Where(u => journalistIds.Contains(u.Id));
                    break;
                case "Editor":
                    var editors = await _userManager.GetUsersInRoleAsync(userParams.Filter);
                    var editorsIds = editors.Select(a => a.Id).ToList();
                    users = users.Where(u => editorsIds.Contains(u.Id));
                    break;
            }

            switch (userParams.OrderBy)
            {
                case "UserName":
                    users = users.OrderBy(u => u.UserName);
                    break;
                case "UserName_Desc":
                    users = users.OrderByDescending(u => u.UserName);
                    break;
                case "Email":
                    users = users.OrderBy(u => u.Email);
                    break;
                case "Email_Desc":
                    users = users.OrderByDescending(u => u.Email);
                    break;
            }
            if (userParams.Search != null)
            {
                users = users.Where(u => u.UserName.Contains(userParams.Search) || u.Id.ToString().Contains(userParams.Search) || u.Email.Contains(userParams.Search));
            }

            return users;
        }
    }
}

