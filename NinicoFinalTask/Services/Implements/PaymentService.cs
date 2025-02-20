using Microsoft.AspNetCore.Mvc;
using NinicoFinalTask.Services.Abstracts;
using NinicoFinalTask.ViewModel.Basket;
using Stripe.BillingPortal;
using Stripe.Checkout;

namespace NinicoFinalTask.Services.Implements
{
    public class PaymentService : IPaymentService
    {
        public async Task<string> CreateCheckoutSessionAsync(List<GetBasketItemVM> basket, string successUrl, string cancelUrl)
        {
            if (basket == null || !basket.Any())
            {
                return " Basket is empty"; 
            }

            var options = new Stripe.Checkout.SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                Mode = "payment",
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl,
                LineItems = basket.Select(item => new Stripe.Checkout.SessionLineItemOptions
                {
                    PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Name
                        },
                        UnitAmount = (long)(item.SellPrice * 100)
                    },
                    Quantity = item.Count
                }).ToList()
            };

            var service = new Stripe.Checkout.SessionService();
            Stripe.Checkout.Session session = await service.CreateAsync(options);

            return session?.Url ?? "Error: Failed to create session";
        }


    }
}

