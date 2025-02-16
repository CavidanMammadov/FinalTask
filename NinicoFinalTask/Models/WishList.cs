namespace NinicoFinalTask.Models
{
    public class WishList:BaseEntity
    {
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
