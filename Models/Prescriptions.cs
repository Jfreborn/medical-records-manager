using MedicalRecordsManager.Models;

namespace MedicalRecordsManager.Models
{
    public class Prescriptions
    {
        public int Id { get; set; }
        public int MedicalRecordId { get; set; }
        public int PatientId { get; set; }
        public string DoctorId { get; set; } = string.Empty;
        public string MedicineName { get; set; } = string.Empty;
        public string Dosage { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;
        public int DurationDays { get; set; }
        public string? Instructions { get; set; }
        public DateTime PrescribedAt { get; set; } = DateTime.UtcNow;

        public MedicalRecord MedicalRecord { get; set; } = null!;
        public Patient Patient { get; set; } = null!;
        public ApplicationUser Doctor { get; set; } = null!;
    }
}