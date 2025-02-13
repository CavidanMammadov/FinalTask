namespace NinicoFinalTask.ViewModel.Basket
{
    public class BasketVM
    {
        public IEnumerable<GetBasketItemVM> Products { get; set; }
        public decimal SubTotal { get; set; }
    }
}
