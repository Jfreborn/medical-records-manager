using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalRecordsManager.Data;
using MedicalRecordsManager.Models;

namespace MedicalRecordsManager.Controllers
{
    [Authorize]
    public class PrescriptionsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public PrescriptionsController(ApplicationDbContext db,
            UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // GET: /Prescriptions
        public async Task<IActionResult> Index()
        {
            var prescriptions = await _db.Prescriptions
                .Include(p => p.Patient)
                .Include(p => p.Doctor)
                .Include(p => p.MedicalRecord)
                .OrderByDescending(p => p.PrescribedAt)
                .ToListAsync();

            return View(prescriptions);
        }

        // GET: /Prescriptions/Create
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Patients = await _db.Patients
                                         .Where(p => p.IsActive).ToListAsync();
            ViewBag.Doctors = await _userManager.GetUsersInRoleAsync("Doctor");
            ViewBag.MedicalRecords = await _db.MedicalRecords
                                         .Include(r => r.Patient)
                                         .OrderByDescending(r => r.VisitDate)
                                         .ToListAsync();
            return View();
        }

        // POST: /Prescriptions/Create
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> Create(Prescriptions model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Patients = await _db.Patients
                                             .Where(p => p.IsActive).ToListAsync();
                ViewBag.Doctors = await _userManager.GetUsersInRoleAsync("Doctor");
                ViewBag.MedicalRecords = await _db.MedicalRecords
                                             .Include(r => r.Patient)
                                             .OrderByDescending(r => r.VisitDate)
                                             .ToListAsync();
                return View(model);
            }

            _db.Prescriptions.Add(model);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Prescription saved successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}