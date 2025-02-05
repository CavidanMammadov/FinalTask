using System.ComponentModel.DataAnnotations;

namespace NinicoFinalTask.ViewModel.Product
{
    public class ProductCreateVM
    {
        [MaxLength(32, ErrorMessage = "Title length must be less than 32"), Required(ErrorMessage = "You must enter ProductName")]
        public string Name { get; set; } = null!;
        [Required(ErrorMessage = "You must enter description")]
        public string Description { get; set; } = null!;
        [Required(ErrorMessage = "You must enter CostPrice")]
        public decimal CostPrice { get; set; }
        [Required(ErrorMessage = "You must enter SellPrice")]
        public decimal SellPrice { get; set; }
        [Required(ErrorMessage = "You must enter Quantity")]
        public int Quantity { get; set; }
        public int Discount { get; set; }
        public IFormFile CoverFile { get; set; } = null!;
        public int? CategoryId { get; set; }
    }
}
