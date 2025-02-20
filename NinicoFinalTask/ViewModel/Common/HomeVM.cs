using NinicoFinalTask.ViewModel.Blog;
using NinicoFinalTask.ViewModel.Category;
using NinicoFinalTask.ViewModel.Product;
using NinicoFinalTask.ViewModel.Slider;

namespace NinicoFinalTask.ViewModel.Common
{
    public class HomeVM
    {
        public IEnumerable<SliderItemVM> Sliders { get; set; }
        public IEnumerable<ProductItemVM> Products { get; set; }
        public IEnumerable<CategoryItemVM> Categories { get; set; }
        public IEnumerable<BlogItemVM> Blogs { get; set; }
    }
}
