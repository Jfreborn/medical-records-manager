using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicalRecordsManager.Data;
using Microsoft.EntityFrameworkCore;

namespace MedicalRecordsManager.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _db;

        public DashboardController(ApplicationDbContext db)
        {
            _db = db;
        }

        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Doctor()
        {
            ViewBag.MyAppointments = await _db.Appointments
                .Where(a => a.Status == "Scheduled")
                .CountAsync();

            ViewBag.CompletedVisits = await _db.Appointments
                .Where(a => a.Status == "Completed")
                .CountAsync();

            ViewBag.PendingLabResults = await _db.LabResults
                .Where(l => l.Status == "Pending")
                .CountAsync();

            return View();
        }

        [Authorize(Roles = "Nurse")]
        public async Task<IActionResult> Nurse()
        {
            var today = DateTime.UtcNow.Date;

            ViewBag.TodayAppointments = await _db.Appointments
                .Where(a => a.AppointmentDate.Date == today)
                .CountAsync();

            ViewBag.ActivePatients = await _db.Patients
                .Where(p => p.IsActive)
                .CountAsync();

            ViewBag.PendingLabResults = await _db.LabResults
                .Where(l => l.Status == "Pending")
                .CountAsync();

            return View();
        }

        [Authorize(Roles = "Accountant")]
        public async Task<IActionResult> Accountant()
        {
            ViewBag.TotalPayments = await _db.Payments
                .Where(p => p.Status == "Paid")
                .SumAsync(p => (decimal?)p.Amount) ?? 0;

            ViewBag.PendingPayments = await _db.Payments
                .Where(p => p.Status == "Pending")
                .CountAsync();

            ViewBag.GiftCardPayments = await _db.Payments
                .Where(p => p.PaymentMethod == "GiftCard")
                .CountAsync();

            return View();
        }
    }
}