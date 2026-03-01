using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalRecordsManager.Data;
using MedicalRecordsManager.Models;

namespace MedicalRecordsManager.Controllers
{
    [Authorize]
    public class LabResultsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public LabResultsController(ApplicationDbContext db) => _db = db;

        // GET: /LabResults
        public async Task<IActionResult> Index()
        {
            var results = await _db.LabResults
                .Include(l => l.Patient)
                .Include(l => l.MedicalRecord)
                .OrderByDescending(l => l.TestedAt)
                .ToListAsync();

            return View(results);
        }

        // GET: /LabResults/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Patients = await _db.Patients
                                         .Where(p => p.IsActive).ToListAsync();
            ViewBag.MedicalRecords = await _db.MedicalRecords
                                         .Include(r => r.Patient)
                                         .OrderByDescending(r => r.VisitDate)
                                         .ToListAsync();
            return View();
        }

        // POST: /LabResults/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LabResult model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Patients = await _db.Patients
                                             .Where(p => p.IsActive).ToListAsync();
                ViewBag.MedicalRecords = await _db.MedicalRecords
                                             .Include(r => r.Patient)
                                             .OrderByDescending(r => r.VisitDate)
                                             .ToListAsync();
                return View(model);
            }

            _db.LabResults.Add(model);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Lab result saved successfully.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /LabResults/MarkReady
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkReady(int id)
        {
            var result = await _db.LabResults.FindAsync(id);
            if (result != null)
            {
                result.Status = "Ready";
                await _db.SaveChangesAsync();
                TempData["Success"] = "Lab result marked as ready.";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: /LabResults/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _db.LabResults.FindAsync(id);
            if (result != null)
            {
                _db.LabResults.Remove(result);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Lab result deleted.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}