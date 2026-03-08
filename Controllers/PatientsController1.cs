using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalRecordsManager.Data;
using MedicalRecordsManager.Models;

namespace MedicalRecordsManager.Controllers
{
    [Authorize]
    public class PatientsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public PatientsController(ApplicationDbContext db) => _db = db;

        // GET: /Patients
        public async Task<IActionResult> Index(string? search)
        {
            var query = _db.Patients.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = query.Where(p =>
                    p.FirstName.ToLower().Contains(search) ||
                    p.LastName.ToLower().Contains(search) ||
                    p.PatientNumber.Contains(search) ||
                    p.PhoneNumber.Contains(search));
            }

            ViewBag.Search = search;
            return View(await query
                .OrderByDescending(p => p.RegisteredAt)
                .ToListAsync());
        }

        // GET: /Patients/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var patient = await _db.Patients
                .Include(p => p.Appointments).ThenInclude(a => a.Doctor)
                .Include(p => p.MedicalRecords).ThenInclude(r => r.Doctor)
                .Include(p => p.Payments)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (patient == null) return NotFound();
            return View(patient);
        }

        // GET: /Patients/Create
        public IActionResult Create() => View();

        // POST: /Patients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Patient model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Convert DateOfBirth to UTC for PostgreSQL
            model.DateOfBirth = DateTime.SpecifyKind(model.DateOfBirth, DateTimeKind.Utc);

            var count = await _db.Patients.CountAsync() + 1;
            model.PatientNumber = $"PAT-{count:D4}";

            _db.Patients.Add(model);
            await _db.SaveChangesAsync();

            TempData["Success"] = $"Patient {model.FullName} registered successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Patients/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var patient = await _db.Patients.FindAsync(id);
            if (patient == null) return NotFound();
            return View(patient);
        }

        // POST: /Patients/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Patient model)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            model.DateOfBirth = DateTime.SpecifyKind(model.DateOfBirth, DateTimeKind.Utc);

            _db.Patients.Update(model);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Patient updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Patients/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var patient = await _db.Patients.FindAsync(id);
            if (patient != null)
            {
                patient.IsActive = false;
                await _db.SaveChangesAsync();
                TempData["Success"] = "Patient removed.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}