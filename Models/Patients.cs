using System.ComponentModel.DataAnnotations;

namespace MedicalRecordsManager.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public string PatientNumber { get; set; } = string.Empty;

        [Required] public string FirstName { get; set; } = string.Empty;
        [Required] public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";

        [Required, DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required] public string Gender { get; set; } = string.Empty;

        public string BloodGroup { get; set; } = string.Empty;

        [Phone] public string PhoneNumber { get; set; } = string.Empty;

        [EmailAddress] public string Email { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string? EmergencyContactName { get; set; }

        public string? EmergencyContactPhone { get; set; }

        public string? Allergies { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? DeletedAt { get; set; }

        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

        public ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();

        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}