using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Portal.Data.Entities;
using Portal.Data.Helpers;
using Portal.Data.Helpers.HelpersInterfaces;
using Portal.Data.ViewModels;
using Portal.Persistance.Repositories.Interfaces;
using Portal.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Wangkanai.Detection;

namespace Portal.Web.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserFilterOrderHelper _userFilterOrderHelper;
        private readonly IDeviceResolver _deviceResolver;
        private readonly IConfiguration _configuration;
        private readonly IMailService _mailService;
        private readonly IPhotoService _photoService;

        public AdminController(UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IAuthorizationService authorizationService,
            IPhotoService photoService,
            IUnitOfWork unitOfWork, IUserFilterOrderHelper userFilterOrderHelper,
            IDeviceResolver deviceResolver, IConfiguration configuration, IMailService mailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _authorizationService = authorizationService;
            _unitOfWork = unitOfWork;
            _userFilterOrderHelper = userFilterOrderHelper;
            _deviceResolver = deviceResolver;
            _configuration = configuration;
            _mailService = mailService;
            _photoService = photoService;
        }
        [HttpGet]
        [Authorize(Policy = "AdministratorOnly")]
        public async Task<IActionResult> UserManagement([FromQuery]int? pageNumber, [FromQuery]int? pageSize, [FromQuery]string filter, [FromQuery] string Search, [FromQuery]string orderBy)
        {

            var conditions = new UserManagemenViewModel
            {
                CurrentPage = pageNumber ?? 1,
                PageSize = pageSize ?? 10,
                Filter = filter ?? "Users",
                Search = Search,
                OrderBy = orderBy ?? "UserName",
                FromDevice = _deviceResolver.Device
            };
            var users = await _userFilterOrderHelper.GetFilteredUsersAsync(conditions);

            conditions.Users = await GetUsersForUserManagementView(users, conditions.CurrentPage, conditions.PageSize);
            return View(conditions);
        }
        [Authorize(Policy = "AdministratorOnly")]
        public IActionResult AddUser()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Policy = "AdministratorOnly")]
        public async Task<IActionResult> AddUser(AddUserViewModel addUserViewModel)
        {
            if (!ModelState.IsValid) return View(addUserViewModel);

            var user = new User()
            {
                Id = Guid.NewGuid(),
                UserName = addUserViewModel.UserName,
                Email = addUserViewModel.Email,
                IsActive = true
            };

            IdentityResult result = await _userManager.CreateAsync(user, addUserViewModel.Password);

            if (result.Succeeded)
            {
                var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);
                var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);
                string url = $"{_configuration["PortalUrl"]}/Account/ConfirmEmail?userid={user.Id}&token={validEmailToken}";

                await _mailService.SendEmailAsync(user.Email, "Confirm your email", $"<h1>Welcome to News Portal</h1>" +
              $"<p>Please confirm your email by <a clicktracking=off href='{url}'>Clicking here</a></p>" +
              $"<p style='color:red;'>Your default password: '{addUserViewModel.Password}' Please change your password after account confirmation!</p>");

                return RedirectToAction("UserManagement", GetUsersForView());
            }

            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(addUserViewModel);
        }

        [Authorize]
        public async Task<IActionResult> DetailsUser(Guid id)
        {

            var user = await _userManager.Users.Include(u => u.Photo).FirstOrDefaultAsync(u => u.Id.Equals(id));
            if (user == null)
                return RedirectToAction("UserManagement", GetUsersForView());
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Any())
                return RedirectToAction("Details", "Employee", new { user.Id });
            var claims = await _userManager.GetClaimsAsync(user);
            var editUser = new EditUserViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                ProfilePhoto = user.Photo,
                IsActive = user.IsActive
            };

            return View(editUser);
        }
        [Authorize]
        public async Task<IActionResult> EditUser(Guid id)
        {

            var user = await _userManager.Users.Include(u => u.Photo).FirstOrDefaultAsync(u => u.Id.Equals(id));
            if (user == null)
                return RedirectToAction("DetailsUser", new { id });

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Any())
            {
                return RedirectToAction("Edit", "Employee", new { user.Id });
            }
            if ((await _authorizationService
               .AuthorizeAsync(User, user, "AccountOwnerOnly")).Succeeded)
            {
                var claims = await _userManager.GetClaimsAsync(user);
                var editUser = new EditUserViewModel()
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    ProfilePhoto = user.Photo,
                    IsActive = user.IsActive

                };

                return View(editUser);
            }
            return RedirectToAction("AccessDenied", "Account");

        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditUser([FromRoute]Guid id, [FromForm] EditUserViewModel editUserViewModel)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user != null)
            {
                if ((await _authorizationService
               .AuthorizeAsync(User, user, "AccountOwnerOnly")).Succeeded)
                {
                    user.Email = editUserViewModel.Email;
                    user.UserName = editUserViewModel.UserName;
                    user.IsActive = editUserViewModel.IsActive;
                    if (editUserViewModel.Photo != null)
                    {
                        var photos = _unitOfWork.UserPhotos.GetAll().Where(p => p.UserId.Equals(user.Id.ToString())).ToList();
                        if (photos.Count() > 0)
                            foreach (var img in photos)
                            {
                                await _photoService.DeletePhotoForUser(img.Id);
                            }
                        await _photoService.AddPhotoForUser(user.Id, editUserViewModel.Photo);
                        await _unitOfWork.CompleteAsync();
                    }
                    var result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                        return RedirectToAction("DetailsUser", new { id });
                    ModelState.AddModelError("", "User not updated, something went wrong.");
                    return View(editUserViewModel);
                }
                else return RedirectToAction("AccessDenied", "Account"); ;
            }

            return RedirectToAction("DetailsUser", new { id });
        }


        [HttpPost]
        [Authorize(Policy = "AdministratorOnly")]
        public async Task<IActionResult> DeleteUser(Guid Id)
        {
            var user = await _userManager.Users
                .Include(u => u.Photo)
                .Include(u => u.Employee)
                .ThenInclude(e => e.Address)
                .Include(u => u.Articles)
                .FirstOrDefaultAsync(u => u.Id.Equals(Id));
            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                    return RedirectToAction("UserManagement", GetUsersForView());
                else
                    ModelState.AddModelError("", "Something went wrong while deleting this user.");
            }
            else
            {
                ModelState.AddModelError("", "This user can't be found");
            }
            return View("UserManagement", GetUsersForView());
        }

        //Roles
        [Authorize(Policy = "AdministratorOnly")]
        public IActionResult RoleManagement()
        {
            List<RoleViewModel> roles = GetRolesForView();
            return View(roles);
        }

        [Authorize(Policy = "AdministratorOnly")]
        public IActionResult AddNewRole() => View();

        [HttpPost]
        [Authorize(Policy = "AdministratorOnly")]
        public async Task<IActionResult> AddNewRole(AddRoleViewModel addRoleViewModel)
        {

            if (!ModelState.IsValid) return View(addRoleViewModel);

            var role = new Role
            {
                Id = Guid.NewGuid(),
                Name = addRoleViewModel.RoleName
            };

            IdentityResult result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                return RedirectToAction("RoleManagement", GetRolesForView());
            }

            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(addRoleViewModel);
        }
        [Authorize(Policy = "AdministratorOnly")]
        public async Task<IActionResult> EditRole(Guid id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());

            if (role == null)
                return RedirectToAction("RoleManagement", GetRolesForView());

            var editRoleViewModel = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name,
                Users = new List<UserInfo>()
            };

            var users = await _userManager.GetUsersInRoleAsync(role.Name);
            editRoleViewModel.Users = users.Select(u => new UserInfo { Text = u.UserName + "  " + u.Email }).ToList();

            return View(editRoleViewModel);
        }

        [HttpPost]
        [Authorize(Policy = "AdministratorOnly")]
        public async Task<IActionResult> EditRole(EditRoleViewModel editRoleViewModel)
        {
            var role = await _roleManager.FindByIdAsync(editRoleViewModel.Id.ToString());

            if (role != null)
            {
                role.Name = editRoleViewModel.RoleName;

                var result = await _roleManager.UpdateAsync(role);

                if (result.Succeeded)
                    return RedirectToAction("RoleManagement", GetRolesForView());

                ModelState.AddModelError("", "Role not updated, something went wrong.");

                return View(editRoleViewModel);
            }

            return RedirectToAction("RoleManagement", GetRolesForView());
        }

        [HttpPost]
        [Authorize(Policy = "AdministratorOnly")]
        public async Task<IActionResult> DeleteRole(Guid id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role != null)
            {
                var result = await _roleManager.DeleteAsync(role);
                if (result.Succeeded)
                    return RedirectToAction("RoleManagement", GetRolesForView());
                ModelState.AddModelError("", "Something went wrong while deleting this role.");
            }
            else
            {
                ModelState.AddModelError("", "This role can't be found.");
            }
            return View("RoleManagement", GetRolesForView());
        }


        //Users in roles
        [Authorize(Policy = "AdministratorOnly")]
        public async Task<IActionResult> AddUserToRole(Guid roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());

            if (role == null)
                return RedirectToAction("RoleManagement", GetRolesForView());

            var addUserToRoleViewModel = new UserRoleViewModel { RoleId = role.Id };
            var usersInRole = _userManager.GetUsersInRoleAsync(role.Name).Result.Select(u => u.Id).ToList();
            var users = await _userManager.Users.Where(u => !usersInRole.Contains(u.Id)).Select(u => new UserInfo { Id = u.Id, Text = u.UserName + " " + u.Email }).ToListAsync();
            addUserToRoleViewModel.Users = users;

            return View(addUserToRoleViewModel);
        }

        [HttpPost]
        [Authorize(Policy = "AdministratorOnly")]
        public async Task<IActionResult> AddUserToRole(UserRoleViewModel userRoleViewModel)
        {
            var user = await _userManager.FindByIdAsync(userRoleViewModel.UserId.ToString());
            var role = await _roleManager.FindByIdAsync(userRoleViewModel.RoleId.ToString());

            var result = await _userManager.AddToRoleAsync(user, role.Name);

            if (result.Succeeded)
            {
                return RedirectToAction("RoleManagement", GetRolesForView());
            }

            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(userRoleViewModel);
        }
        [Authorize(Policy = "AdministratorOnly")]
        public async Task<IActionResult> DeleteUserFromRole(Guid roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());

            if (role == null)
                return RedirectToAction("RoleManagement", GetRolesForView());

            var addUserToRoleViewModel = new UserRoleViewModel { RoleId = role.Id };

            addUserToRoleViewModel.Users = _userManager.GetUsersInRoleAsync(role.Name).Result.Select(u => new UserInfo { Id = u.Id, Text = u.UserName + " " + u.Email }).ToList();
            return View(addUserToRoleViewModel);
        }


        [HttpPost]
        [Authorize(Policy = "AdministratorOnly")]
        public async Task<IActionResult> DeleteUserFromRole(UserRoleViewModel userRoleViewModel)
        {
            var user = await _userManager.FindByIdAsync(userRoleViewModel.UserId.ToString());
            var role = await _roleManager.FindByIdAsync(userRoleViewModel.RoleId.ToString());

            var result = await _userManager.RemoveFromRoleAsync(user, role.Name);

            if (result.Succeeded)
            {
                return RedirectToAction("RoleManagement", GetRolesForView());
            }

            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(userRoleViewModel);
        }

        private List<RoleViewModel> GetRolesForView()
        {
            return _roleManager.Roles.Select(r => new RoleViewModel
            {
                Id = r.Id,
                Name = r.Name
            }).ToList();
        }
        private List<UserViewModel> GetUsersForView()
        {
            return _userManager.Users.Select(u => new UserViewModel
            {
                Id = u.Id,
                Email = u.Email,
                UserName = u.UserName
            }).ToList();
        }

        private async Task<PagedList<UserViewModel>> GetUsersForUserManagementView(IQueryable<User> source, int pageNumber, int pageSize)
        {
            var sourceView = source.Select(u => new UserViewModel
            {
                Id = u.Id,
                Email = u.Email,
                UserName = u.UserName,
                Url = u.Photo.Url,
                IsActive = u.IsActive,
                FirstName = u.Employee.FirstName,
                LastName = u.Employee.LastName
            });
            return await PagedList<UserViewModel>.CreateAsync(sourceView, pageNumber, pageSize);
        }


    }
}