using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalRecordsManager.Data;
using MedicalRecordsManager.Models;

namespace MedicalRecordsManager.Controllers
{
    [Authorize]
    public class MedicalRecordsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public MedicalRecordsController(ApplicationDbContext db,
            UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // GET: /MedicalRecords
        public async Task<IActionResult> Index()
        {
            var records = await _db.MedicalRecords
                .Include(r => r.Patient)
                .Include(r => r.Doctor)
                .OrderByDescending(r => r.VisitDate)
                .ToListAsync();

            return View(records);
        }

        // GET: /MedicalRecords/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var record = await _db.MedicalRecords
                .Include(r => r.Patient)
                .Include(r => r.Doctor)
                .Include(r => r.Prescriptions)
                .Include(r => r.LabResults)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (record == null) return NotFound();
            return View(record);
        }

        // GET: /MedicalRecords/Create
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Patients = await _db.Patients
                                       .Where(p => p.IsActive).ToListAsync();
            ViewBag.Doctors = await _userManager.GetUsersInRoleAsync("Doctor");
            ViewBag.Appointments = await _db.Appointments
                                       .Include(a => a.Patient)
                                       .Where(a => a.Status == "Scheduled")
                                       .ToListAsync();
            return View();
        }

        // POST: /MedicalRecords/Create
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> Create(MedicalRecord model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Patients = await _db.Patients
                                           .Where(p => p.IsActive).ToListAsync();
                ViewBag.Doctors = await _userManager.GetUsersInRoleAsync("Doctor");
                ViewBag.Appointments = await _db.Appointments
                                           .Include(a => a.Patient)
                                           .Where(a => a.Status == "Scheduled")
                                           .ToListAsync();
                return View(model);
            }

            _db.MedicalRecords.Add(model);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Medical record created successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}