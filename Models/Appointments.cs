using MedicalRecordsManager.Models;
using System.ComponentModel.DataAnnotations;

namespace MedicalRecordsManager.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        [Required] public int PatientId { get; set; }
        [Required] public string DoctorId { get; set; } = string.Empty;

        [Required, DataType(DataType.Date)]
        public DateTime AppointmentDate { get; set; }

        [Required, DataType(DataType.Time)]
        public TimeSpan AppointmentTime { get; set; }

        [Required] public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = "Scheduled";
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Patient Patient { get; set; } = null!;
        public ApplicationUser Doctor { get; set; } = null!;
    }
}