using NinicoFinalTask.ViewModel.Product;
using NinicoFinalTask.ViewModel.Slider;

namespace NinicoFinalTask.ViewModel.Common
{
    public class HomeVM
    {
        public IEnumerable<SliderItemVM> Sliders { get; set; }
        public IEnumerable<ProductItemVM> Products { get; set; }
    }
}
