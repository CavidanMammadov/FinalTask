using NinicoFinalTask.ViewModel.Common;
using System.ComponentModel.DataAnnotations;

namespace NinicoFinalTask.ViewModel.Product
{
    public class ProductUpdateVM
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal CostPrice { get; set; }
        public decimal SellPrice { get; set; }
        public int Quantity { get; set; }
        public int Discount { get; set; }
        public string CoverFileUrl { get; set; }
        public IEnumerable<ImageUrlAndId> OtherImagesUrl { get; set; }
        public IFormFile CoverFile { get; set; } = null!;
        public IEnumerable<IFormFile> OtherImages { get; set; }
        public int? CategoryId { get; set; }
    }
}
