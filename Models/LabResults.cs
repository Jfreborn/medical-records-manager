using MedicalRecordsManager.Models;

namespace MedicalRecordsManager.Models
{
    public class LabResult
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int MedicalRecordId { get; set; }
        public string TestName { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
        public string? NormalRange { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime TestedAt { get; set; } = DateTime.UtcNow;

        public Patient Patient { get; set; } = null!;
        public MedicalRecord MedicalRecord { get; set; } = null!;
    }
}