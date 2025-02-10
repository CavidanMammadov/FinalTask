using NinicoFinalTask.Models;

namespace NinicoFinalTask.ViewModel.Product
{
    public class ProductDetailVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Discount { get; set; }
        public int Quantity { get; set; }
        public bool IsInStock { get; set; }
        public List<string> OtherImagesUrl { get; set; }
        public int? CategoryId { get; set; }
        public List<ProductItemVM> RelatedProducts { get; set; }
        public ProductDetailVM()
        {
            RelatedProducts = new List<ProductItemVM>();
        }
    }
}
