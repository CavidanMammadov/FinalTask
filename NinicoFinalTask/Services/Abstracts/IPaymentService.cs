using NinicoFinalTask.ViewModel.Basket;

namespace NinicoFinalTask.Services.Abstracts
{
    public interface IPaymentService
    {
        Task<string> CreateCheckoutSessionAsync(List<GetBasketItemVM> basket, string successUrl, string cancelUrl);
    }
}
