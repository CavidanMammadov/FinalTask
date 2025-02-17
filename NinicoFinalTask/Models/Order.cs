namespace NinicoFinalTask.Models
{
    public class Order:BaseEntity
    {
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string  EmailLine { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

    }
}
