using NinicoFinalTask.ViewModel.Product;

namespace NinicoFinalTask.ViewModel.Blog
{
    public class BlogDetailVm
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedTime { get; set; }
        public List<string> OtherImagesUrl { get; set; }
        public List<BlogItemVM> OtherBlogs { get; set; }
        public BlogDetailVm()
        {
            OtherBlogs = new List<BlogItemVM>();
        }
    }
}
