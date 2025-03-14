using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NinicoFinalTask.DataAcces;
using NinicoFinalTask.Models;
using Stripe.Checkout;

namespace NinicoFinalTask.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly StripeSettings _stripeSettings;
        private readonly NinicoDbContext _context;

        public CheckoutController(IOptions<StripeSettings> stripeSettings, NinicoDbContext context)
        {
            _stripeSettings = stripeSettings.Value;
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.PublishableKey = _stripeSettings.PublishableKey;
            return View();
        }

        [HttpPost]
        public IActionResult CreateCheckoutSession()
        {
            var domain = "https://localhost:7067"; 

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
        {
            new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "usd",
                    UnitAmount = 2000, // 20.00 USD
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "Avtomobil icarəsi"
                    }
                },
                Quantity = 1,
            },
        },
                Mode = "payment",
                SuccessUrl = domain + "/Checkout/Success?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = domain + "/Checkout/Cancel",
            };

            var service = new SessionService();
            Session session = service.Create(options);

            return Json(new { id = session.Id });
        }

        [HttpGet]
        public IActionResult Success(string session_id)
        {
            var service = new SessionService();
            var session = service.Get(session_id);

            if (session.PaymentStatus == "paid")
            {
                var order = new Order
                {
                    UserId = User.Identity.Name,
                    TotalAmount = (session.AmountTotal ?? 0) / 100m, // Düzəliş olundu
                    PaymentStatus = "Paid",
                    StripeSessionId = session.Id
                };

                _context.Orders.Add(order);
                _context.SaveChanges();
            }

            ViewBag.Message = "Ödəniş uğurla tamamlandı!";
            return View();
        }


        public IActionResult Cancel()
        {
            ViewBag.Message = "Ödəniş ləğv edildi.";
            return View();
        }
    }
}
    