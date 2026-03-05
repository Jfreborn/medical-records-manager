using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalRecordsManager.Data;

namespace MedicalRecordsManager.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var today = DateTime.UtcNow.Date;

            ViewBag.TotalPatients = await _db.Patients
                                            .CountAsync(p => p.IsActive);
            ViewBag.TodayAppointments = await _db.Appointments
                                            .CountAsync(a => a.AppointmentDate.Date == today);
            ViewBag.PendingLabs = await _db.LabResults
                                            .CountAsync(l => l.Status == "Pending");
            ViewBag.TotalRevenue = await _db.Payments
                                            .Where(p => p.Status == "Paid")
                                            .SumAsync(p => (decimal?)p.Amount) ?? 0;

            var recentAppointments = await _db.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Where(a => a.AppointmentDate.Date == today)
                .OrderBy(a => a.AppointmentTime)
                .Take(8)
                .ToListAsync();

            return View(recentAppointments);
        }
    }
}