using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NinicoFinalTask.Services.Implements;
using NinicoFinalTask.ViewModel.Basket;
using Stripe;
 using Stripe.Checkout;
using System.Security.Claims;

namespace NinicoFinalTask.Controllers
{
    public class PaymentController(PaymentService _payment , OrderService _orderService) : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Checkout()
        {
            string basketJson = Request.Cookies["Basket"];

            if (string.IsNullOrEmpty(basketJson))
            {
                return BadRequest("Səbət boşdur.");
            }

            List<GetBasketItemVM> basket = JsonConvert.DeserializeObject<List<GetBasketItemVM>>(basketJson)!;

            decimal totalAmount = basket.Sum(item => item.SellPrice * item.Count);

            if (totalAmount <= 0)
            {
                return BadRequest("Səbətdə düzgün məbləğ yoxdur.");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 
            await _orderService.CreateOrderAsync(userId, totalAmount);

            string checkoutUrl = await _payment.CreateCheckoutSessionAsync(
                basket,
                Url.Action("Success", "Payment", null, Request.Scheme)!,
                Url.Action("Cancel", "Payment", null, Request.Scheme)!
            );

            return Redirect(checkoutUrl);
        }

        [HttpGet]
        public async Task<IActionResult> Success()
        {
            ViewBag.Message = "Payment completed successfully.";
            return View();
        }

        [HttpGet]
        public async  Task<IActionResult> Cancel()
        {
            ViewBag.Message = "Payment canceled";
            return View();
        }
        public async Task<IActionResult> Webhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], "STRIPE_SECRET_WEBHOOK_KEY");

            if (stripeEvent.Type == "checkout.session.completed")
            {
                var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
                if (session != null && session.CustomerEmail != null)
                {
                    await _orderService.ConfirmOrderAsync(session.CustomerEmail);
                }
            }

            return Ok();
        }



    }
}
