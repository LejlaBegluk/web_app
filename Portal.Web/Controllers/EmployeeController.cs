using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Portal.Data.Entities;
using Portal.Data.ViewModels;
using Portal.Persistance.Repositories.Interfaces;
using Portal.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Portal.Web.Controllers
{
    public class EmployeeController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly IPhotoService _photoService;
        private readonly IMailService _mailService;
        private readonly IConfiguration _configuration;

        public EmployeeController(IUnitOfWork unitOfWork,
            RoleManager<Role> roleManager,
            UserManager<User> userManager,
            IPhotoService photoService, IMailService mailService,
            IConfiguration configuration

           )
        {

            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _userManager = userManager;
            _photoService = photoService;
            _mailService = mailService;
            _configuration = configuration;
        }

        // GET: Employee/Create
        public async Task<IActionResult> Create()
        {
            var users = await _userManager.GetUsersInRoleAsync("Editor");
            var employeeModel = new EmployeeViewModel()
            {
                EmployeeTypes = _roleManager.Roles.Select(r => r.Name).ToList(),
                Countries = _unitOfWork.Countries.GetAll().OrderBy(c => c.Name).ToList(),
                Editors = users.Select(u => new User { Id = u.Id, UserName = u.UserName }).ToList(),
                BirthDate = DateTime.UtcNow,
                StartOfEmployment = DateTime.UtcNow,
                EndOfEmployment = DateTime.UtcNow

            };
            return View(employeeModel);
        }

        // POST: Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] EmployeeViewModel employeeViewModel)
        {
            var users = await _userManager.GetUsersInRoleAsync("Editor");
            employeeViewModel.Editors = users.Select(u => new User { Id = u.Id, UserName = u.UserName }).ToList();
            employeeViewModel.EmployeeTypes = _roleManager.Roles.Select(r => r.Name).ToList();
            if (ModelState.IsValid)
            {
                if (employeeViewModel.EmployeeType.Equals("Journalist") && employeeViewModel.EditorId == null)
                {
                    ModelState.AddModelError("", "You need to choose editor if you want to add Journalist!");
                    return View(employeeViewModel);
                }
                var user = new User()
                {
                    Id = Guid.NewGuid(),
                    UserName = employeeViewModel.UserName,
                    Email = employeeViewModel.Email,
                    PhoneNumber = employeeViewModel.PhoneNumber,
                    CreatedOn = DateTime.Now,
                    IsActive = true,
                    EditorId = employeeViewModel.EditorId
                };
                var result = await _userManager.CreateAsync(user, employeeViewModel.Password);
                if (result.Succeeded)
                {
                    user = await _userManager.FindByEmailAsync(employeeViewModel.Email);
                    var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);
                    var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);
                    string url = $"{_configuration["PortalUrl"]}/Account/ConfirmEmail?userid={user.Id}&token={validEmailToken}";

                    await _mailService.SendEmailAsync(user.Email, "Confirm your email", $"<h1>Welcome to News Portal</h1>" +
                  $"<p>Please confirm your email by <a clicktracking=off href='{url}'>Clicking here</a></p>" +
                  $"<p style='color:red;'>Your default password: '{employeeViewModel.Password}' Please change your password after account confirmation!</p>");

                    var employee = new Employee()
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        FirstName = employeeViewModel.FirstName,
                        LastName = employeeViewModel.LastName,
                        SocialSecurityNumber = employeeViewModel.SocialSecurityNumber,
                        Gender = employeeViewModel.Gender,
                        BirthDate = employeeViewModel.BirthDate,
                        StartOfEmployment = employeeViewModel.StartOfEmployment,
                        EndOfEmployment = employeeViewModel.EndOfEmployment
                    };
                    var address = new Address()
                    {
                        Id = Guid.NewGuid(),
                        Name = employeeViewModel.Address,
                        CountryId = employeeViewModel.CountryId
                    };
                    employee.AddressId = address.Id;
                    _unitOfWork.Employees.Add(employee);
                    _unitOfWork.Addresses.Add(address);
                    await _unitOfWork.CompleteAsync();

                    await _userManager.AddToRoleAsync(user, employeeViewModel.EmployeeType);

                    return RedirectToAction("UserManagement", "Admin");
                }
            }

            return View(employeeViewModel);
        }

        // GET: Employee/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emp = await _userManager.Users
                .Include(u => u.Employee)
                .ThenInclude(e => e.Address)
                .Include(u => u.Photo)
                .FirstOrDefaultAsync(u => u.Id.Equals(id));
            var users = await _userManager.GetUsersInRoleAsync("Editor");
            var roles = await _userManager.GetRolesAsync(emp);


            var employee = new EditEmployeeViewModel()
            {
                Id = emp.Id,
                Email = emp.Email,
                UserName = emp.UserName,
                Photo = emp.Photo,
                IsActive = emp.IsActive,
                PhoneNumber = emp.PhoneNumber,
                FirstName = emp.Employee == null ? "" : emp.Employee?.FirstName,
                LastName = emp.Employee == null ? "" : emp.Employee?.LastName,
                BirthDate = emp.Employee == null ? DateTime.Now : emp.Employee.BirthDate,
                StartOfEmployment = emp.Employee == null ? DateTime.Now : emp.Employee.StartOfEmployment,
                EndOfEmployment = emp.Employee == null ? DateTime.Now : emp.Employee.EndOfEmployment,
                SocialSecurityNumber = emp.Employee == null ? "" : emp.Employee.SocialSecurityNumber,
                Address = emp.Employee == null ? "" : emp.Employee.Address.Name,
                CountryId = emp.Employee == null ? Guid.Parse("6D7655D5-F0C4-4FCB-A3E6-2234B37462AD") : emp.Employee.Address.CountryId,
                Editors = users.Select(u => new User { Id = u.Id, UserName = u.UserName }).ToList(),
                Countries = _unitOfWork.Countries.GetAll().OrderBy(c => c.Name).ToList(),
                EmployeeTypes = _roleManager.Roles.Select(r => r.Name).ToList(),
                EmployeeType = roles.FirstOrDefault()

            };
            if (await _userManager.IsInRoleAsync(emp, "Journalist"))
                employee.EditorId = emp.EditorId;
            return View(employee);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute]Guid id, [FromForm] EditEmployeeViewModel editEmployeeViewModel)
        {
            var user = await _unitOfWork.Users.GetAll().Include(u => u.Employee).ThenInclude(e => e.Address).FirstOrDefaultAsync(u => u.Id.Equals(id));
            if (user == null)
            {
                return NotFound();
            }
            if (user.Employee == null)
            {
                var employee = new Employee()
                {
                    FirstName = editEmployeeViewModel.FirstName,
                    LastName = editEmployeeViewModel.LastName,
                    UserId = user.Id,
                    SocialSecurityNumber = editEmployeeViewModel.SocialSecurityNumber,
                    Gender = editEmployeeViewModel.Gender,
                    BirthDate = editEmployeeViewModel.BirthDate,
                    StartOfEmployment = editEmployeeViewModel.StartOfEmployment,
                    EndOfEmployment = editEmployeeViewModel.EndOfEmployment
                };
                var address = new Address()
                {
                    Id = Guid.NewGuid(),
                    Name = editEmployeeViewModel.Address,
                    CountryId = editEmployeeViewModel.CountryId
                };
                employee.AddressId = address.Id;
                _unitOfWork.Employees.Add(employee);
                _unitOfWork.Addresses.Add(address);


                await _unitOfWork.CompleteAsync();
            }

            if (ModelState.IsValid)
            {
                user.UserName = editEmployeeViewModel.UserName;
                user.Email = editEmployeeViewModel.Email;
                user.PhoneNumber = editEmployeeViewModel.PhoneNumber;
                user.EditorId = editEmployeeViewModel.EditorId;
                user.Employee.FirstName = editEmployeeViewModel.FirstName;
                user.Employee.LastName = editEmployeeViewModel.LastName;
                user.Employee.SocialSecurityNumber = editEmployeeViewModel.SocialSecurityNumber;
                user.Employee.Gender = editEmployeeViewModel.Gender;
                user.Employee.BirthDate = editEmployeeViewModel.BirthDate;
                user.Employee.StartOfEmployment = editEmployeeViewModel.StartOfEmployment;
                user.Employee.EndOfEmployment = editEmployeeViewModel.EndOfEmployment;
                user.Employee.Address.Name = editEmployeeViewModel.Address;
                user.Employee.Address.CountryId = editEmployeeViewModel.CountryId;
                if (editEmployeeViewModel.Image != null)
                {
                    var photos = _unitOfWork.UserPhotos.GetAll().Where(p => p.UserId.Equals(user.Id)).ToList();
                    if (photos.Count() > 0)
                        foreach (var img in photos)
                        {
                            await _photoService.DeletePhotoForUser(img.Id);
                        }
                    await _photoService.AddPhotoForUser(user.Id, editEmployeeViewModel.Image);
                }
                await _unitOfWork.CompleteAsync();
                return RedirectToAction("Details", "Employee", new { user.Id });
            }
            var users = await _userManager.GetUsersInRoleAsync("Editor");
            editEmployeeViewModel.Countries = _unitOfWork.Countries.GetAll().OrderBy(c => c.Name).ToList();
            editEmployeeViewModel.Editors = users.Select(u => new User { Id = u.Id, UserName = u.UserName }).ToList();
            editEmployeeViewModel.EmployeeTypes = _roleManager.Roles.Select(r => r.Name).ToList();
            if (await _userManager.IsInRoleAsync(user, "Journalist"))
                editEmployeeViewModel.EditorId = user.EditorId;
            return RedirectToAction("Details", new { user.Id });
        }

        [Authorize]
        public async Task<IActionResult> Details([FromRoute]Guid id)
        {

            var user = await _unitOfWork.Users.GetAll().Include(u => u.Editor).Include(u => u.Photo).Include(u => u.Employee).ThenInclude(e => e.Address).ThenInclude(a => a.Country).FirstOrDefaultAsync(u => u.Id.Equals(id));
            if (user == null)
                return RedirectToAction("UserManagement", GetUsersForView());
            if (user.Employee == null)
                return RedirectToAction("Edit", new { user.Id });
            var editUser = new EditEmployeeViewModel();
            editUser.Id = user.Id;
            editUser.UserName = user.UserName;
            editUser.Email = user.Email;
            editUser.PhoneNumber = user.PhoneNumber;
            editUser.EditorId = user.EditorId;
            editUser.FirstName = user.Employee?.FirstName;
            editUser.LastName = user.Employee?.LastName;
            editUser.Photo = user.Photo;
            editUser.IsActive = user.IsActive;
            editUser.SocialSecurityNumber = user.Employee.SocialSecurityNumber;
            editUser.Gender = user.Employee.Gender;
            editUser.BirthDate = user.Employee.BirthDate;
            editUser.StartOfEmployment = user.Employee.StartOfEmployment;
            editUser.EndOfEmployment = user.Employee.EndOfEmployment;
            editUser.Address = user.Employee.Address.Name;
            editUser.Country = user.Employee.Address.Country.Name;
            var users = await _userManager.GetUsersInRoleAsync("Editor");
            editUser.Editors = users.Select(u => new User { Id = u.Id, UserName = u.UserName }).ToList();
            var usersRoles = await _userManager.GetRolesAsync(user);
            editUser.EmployeeTypes = usersRoles.ToList();
            return View(editUser);
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

    }

}
