using MedicalRecordsManager.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalRecordsManager.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int? AppointmentId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public string Description { get; set; } = string.Empty;
        public string? ReceiptNumber { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        public Patient Patient { get; set; } = null!;
        public Appointment? Appointment { get; set; }
    }
}