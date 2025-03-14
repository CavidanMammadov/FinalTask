using System.ComponentModel.DataAnnotations;

namespace NinicoFinalTask.Models
{
    public class Order:BaseEntity
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public decimal TotalAmount { get; set; } 

        public DateTime PaymentDate { get; set; } = DateTime.Now; 

        public string PaymentStatus { get; set; } = "Pending";

        public string StripeSessionId { get; set; }
    }
}
