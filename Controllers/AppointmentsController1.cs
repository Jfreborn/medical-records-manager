using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalRecordsManager.Data;
using MedicalRecordsManager.Models;

namespace MedicalRecordsManager.Controllers
{
    [Authorize]
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public AppointmentsController(ApplicationDbContext db,
            UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // GET: /Appointments
        public async Task<IActionResult> Index()
        {
            var appointments = await _db.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();

            return View(appointments);
        }

        // GET: /Appointments/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Patients = await _db.Patients
                .Where(p => p.IsActive).ToListAsync();
            ViewBag.Doctors = await _userManager
                .GetUsersInRoleAsync("Doctor");
            return View();
        }

        // POST: /Appointments/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Appointment model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Patients = await _db.Patients
                    .Where(p => p.IsActive).ToListAsync();
                ViewBag.Doctors = await _userManager
                    .GetUsersInRoleAsync("Doctor");
                return View(model);
            }

            _db.Appointments.Add(model);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Appointment booked successfully.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Appointments/UpdateStatus
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var appt = await _db.Appointments.FindAsync(id);
            if (appt != null)
            {
                appt.Status = status;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: /Appointments/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var appt = await _db.Appointments.FindAsync(id);
            if (appt != null)
            {
                _db.Appointments.Remove(appt);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Appointment deleted.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}