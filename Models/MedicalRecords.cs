using MedicalRecordsManager.Models;
using Microsoft.AspNetCore.SignalR.Protocol;
using System.ComponentModel.DataAnnotations;

namespace MedicalRecordsManager.Models
{
    public class MedicalRecord
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string DoctorId { get; set; } = string.Empty;
        public int? AppointmentId { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime VisitDate { get; set; }

        [Required] public string Diagnosis { get; set; } = string.Empty;
        [Required] public string Symptoms { get; set; } = string.Empty;
        [Required] public string Treatment { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Patient Patient { get; set; } = null!;
        public ApplicationUser Doctor { get; set; } = null!;
        public Appointment? Appointment { get; set; }
        public ICollection<Prescriptions> Prescriptions { get; set; } = new List<Prescriptions>();
        public ICollection<LabResult> LabResults { get; set; } = new List<LabResult>();
    }
}