namespace NinicoFinalTask.Models
{
    public class Blog :BaseEntity
    {
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Description { get; set; }
        public int UserId { get; set; }
        public string ImageUrl { get; set; }
        public List<BlogImage>? Images { get; set; }
    }
}
