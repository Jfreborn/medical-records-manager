using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MedicalRecordsManager.Models;

namespace MedicalRecordsManager.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(SignInManager<ApplicationUser> signInManager,
                                 UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // GET: /Account/Login
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password,
                                               bool rememberMe, string? returnUrl = null)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewData["Error"] = "Email and password are required.";
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(
                email, password, rememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(email);

                if (user != null)
                {
                    if (await _userManager.IsInRoleAsync(user, "Admin"))
                        return RedirectToAction("Index", "Home");

                    if (await _userManager.IsInRoleAsync(user, "Doctor"))
                        return RedirectToAction("Doctor", "Dashboard");

                    if (await _userManager.IsInRoleAsync(user, "Nurse"))
                        return RedirectToAction("Nurse", "Dashboard");

                    if (await _userManager.IsInRoleAsync(user, "Accountant"))
                        return RedirectToAction("Accountant", "Dashboard");
                }

                return RedirectToAction("Index", "Home");
            }

            ViewData["Error"] = "Invalid email or password.";
            return View();
        }

        // POST: /Account/Logout
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        // Remove the "Forbidden" string so it looks for AccessDenied.cshtml
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}