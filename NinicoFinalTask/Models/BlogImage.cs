namespace NinicoFinalTask.Models
{
    public class BlogImage :BaseEntity
    {
        public string ImageUrl { get; set; }
        public int? BlogId { get; set; }
        public Blog? Blog { get; set; }
    }
}
