namespace NinicoFinalTask.Models
{
    public class Order:BaseEntity
    {
        public string UserId { get; set; } 
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }

    }
}
