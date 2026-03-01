using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalRecordsManager.Data;
using MedicalRecordsManager.Models;

namespace MedicalRecordsManager.Controllers
{
    [Authorize]
    public class PaymentsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public PaymentsController(ApplicationDbContext db) => _db = db;

        // GET: /Payments
        public async Task<IActionResult> Index()
        {
            var payments = await _db.Payments
                .Include(p => p.Patient)
                .Include(p => p.Appointment)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

            return View(payments);
        }

        // GET: /Payments/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Patients = await _db.Patients
                                       .Where(p => p.IsActive).ToListAsync();
            ViewBag.Appointments = await _db.Appointments
                                       .Include(a => a.Patient)
                                       .Where(a => a.Status == "Completed")
                                       .ToListAsync();
            return View();
        }

        // POST: /Payments/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Payment model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Patients = await _db.Patients
                                           .Where(p => p.IsActive).ToListAsync();
                ViewBag.Appointments = await _db.Appointments
                                           .Include(a => a.Patient)
                                           .Where(a => a.Status == "Completed")
                                           .ToListAsync();
                return View(model);
            }

            _db.Payments.Add(model);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Payment recorded successfully.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Payments/MarkPaid
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkPaid(int id)
        {
            var payment = await _db.Payments.FindAsync(id);
            if (payment != null)
            {
                payment.Status = "Paid";
                await _db.SaveChangesAsync();
                TempData["Success"] = "Payment marked as paid.";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: /Payments/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var payment = await _db.Payments.FindAsync(id);
            if (payment != null)
            {
                _db.Payments.Remove(payment);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Payment deleted.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}