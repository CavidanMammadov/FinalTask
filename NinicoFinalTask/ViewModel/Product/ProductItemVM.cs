namespace NinicoFinalTask.ViewModel.Product
{
    public class ProductItemVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Discount { get; set; }
        public int Quantity { get; set; }
        public bool IsInStock { get; set; }

    }
}
