using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MedicalRecordsManager.Models;

namespace MedicalRecordsManager.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // GET: /Users
        public IActionResult Index()
        {
            var users = _userManager.Users
                .OrderBy(u => u.UserRole)
                .ToList();
            return View(users);
        }

        // GET: /Users/Create
        public IActionResult Create() => View();

        // POST: /Users/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string FullName, string Email,
            string UserRole, string? Department,
            string? Specialization, string Password)
        {
            if (await _userManager.FindByEmailAsync(Email) != null)
            {
                ViewData["Error"] = "A user with this email already exists.";
                return View();
            }

            var user = new ApplicationUser
            {
                FullName = FullName,
                UserName = Email,
                Email = Email,
                UserRole = UserRole,
                Department = Department,
                Specialization = Specialization,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, UserRole);
                TempData["Success"] = $"{UserRole} {FullName} created successfully.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["Error"] = string.Join(", ", result.Errors.Select(e => e.Description));
            return View();
        }

        // GET: /Users/Edit/id
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }

        // POST: /Users/Edit
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string Id, string FullName,
            string UserRole, string? Department,
            string? Specialization, string? NewPassword)
        {
            var user = await _userManager.FindByIdAsync(Id);
            if (user == null) return NotFound();

            // Update old role
            var oldRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, oldRoles);

            user.FullName = FullName;
            user.UserRole = UserRole;
            user.Department = Department;
            user.Specialization = Specialization;

            await _userManager.UpdateAsync(user);
            await _userManager.AddToRoleAsync(user, UserRole);

            // Change password if provided
            if (!string.IsNullOrEmpty(NewPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                await _userManager.ResetPasswordAsync(user, token, NewPassword);
            }

            TempData["Success"] = "User updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Users/ToggleStatus
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.IsActive = !user.IsActive;
                await _userManager.UpdateAsync(user);
                TempData["Success"] = $"User {(user.IsActive ? "activated" : "deactivated")}.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}