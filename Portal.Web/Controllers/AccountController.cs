using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Portal.Data.Entities;
using Portal.Data.ViewModels;
using Portal.Persistance.Repositories.Interfaces;
using Portal.Web.Services;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IMailService _mailService;
        private readonly IPhotoService _photoService;
        public AccountController(UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration,
            IMailService mailService,
            IPhotoService photoService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _mailService = mailService;
            _photoService = photoService;

        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
                return View(loginViewModel);

            var user = await _userManager.FindByEmailAsync(loginViewModel.Email);

            if (user != null)
            {
                if (user.IsActive == false || user.EmailConfirmed == false)
                {
                    ModelState.AddModelError("", "Your account is not active. Contract support news.portal.support@gmail.com");
                    return View(loginViewModel);
                }
                var result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, false, false);
                if (result.Succeeded)
                {
                    if (string.IsNullOrEmpty(loginViewModel.ReturnUrl))
                        return RedirectToAction("Index", "Home");

                    return Redirect(loginViewModel.ReturnUrl);
                }
            }

            ModelState.AddModelError("", "Username/password not found");
            return View(loginViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = new User()
                {
                    Id = Guid.NewGuid(),
                    UserName = registerViewModel.UserName,
                    Email = registerViewModel.Email,
                    IsActive = true,
                    CreatedOn = DateTime.Now
                };
                var result = await _userManager.CreateAsync(user, registerViewModel.Password);
                if (result.Succeeded == false)
                {

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(registerViewModel);
                }

                if (result.Succeeded)
                {
                    var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);
                    var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);
                    string url = $"{_configuration["PortalUrl"]}/Account/ConfirmEmail?userid={user.Id}&token={validEmailToken}";

                    await _mailService.SendEmailAsync(user.Email, "Confirm your email", $"<h1>Welcome to News Portal</h1>" +
                  $"<p>Please confirm your email by <a clicktracking=off href='{url}'>Clicking here</a></p>");

                    if (registerViewModel.Photo != null)
                        await _photoService.AddPhotoForUser(user.Id, registerViewModel.Photo);


                    return RedirectToAction("Info");
                }
            }
            return View(registerViewModel);
        }
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(Guid userId, string token)
        {
            if (userId == null || string.IsNullOrWhiteSpace(token))
                return NotFound();
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return BadRequest("User does not exist!");
            var decodedToken = WebEncoders.Base64UrlDecode(token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);
            var result = await _userManager.ConfirmEmailAsync(user, normalToken);
            if (result.Succeeded)
            {
                return View("Login", new LoginViewModel { EmailConfirmed = true });
            }

            return BadRequest(result);
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult ForgotPassword() => View();
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword([FromForm]ForgotPasswordViewModel model)
        {
            if (string.IsNullOrEmpty(model.Email))
                return NotFound();
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "User for this emal does not exist on system!");

                return View();
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Encoding.UTF8.GetBytes(token);
            var validToken = WebEncoders.Base64UrlEncode(encodedToken);

            string url = $"{_configuration["PortalUrl"]}/Account/ResetPassword?email={model.Email}&token={validToken}";

            await _mailService.SendEmailAsync(model.Email, "Reset Password", "<h1>Follow the instructions to reset your password</h1>" +
                $"<p>To reset your password <a href='{url}'>Click here</a></p>");
            return RedirectToAction("Info");
        }
        public IActionResult ResetPassword(string email, string token) => View(new ResetPasswordViewModel { Email = email, Token = token });

        [HttpPost]
        public async Task<IActionResult> ResetPassword([FromForm]ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                    BadRequest("User not Found!");
                var decodedToken = WebEncoders.Base64UrlDecode(model.Token);
                string normalToken = Encoding.UTF8.GetString(decodedToken);
                var result = await _userManager.ResetPasswordAsync(user, normalToken, model.NewPassword);
                if (result.Succeeded)
                    return View("Login", new LoginViewModel { PasswordReset = true });
            }
            return View(model);
        }

        public IActionResult Register() => View();
        public IActionResult AccessDenied() => View();
        public IActionResult Info() => View(new InfoViewModel { Message = "Please check your email!" });

    }
}