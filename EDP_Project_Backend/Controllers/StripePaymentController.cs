using Microsoft.CodeAnalysis.Scripting;
using System;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using EDP_Project_Backend.Models; // Import your data models
using Stripe.Checkout;
using Stripe.Issuing;

namespace EDP_Project_Backend.Controllers
{
    [ApiController]
    [Route("/stripepayment/api")]
    public class StripePaymentController : ControllerBase
    {
        private readonly string StripeSecretKey = "sk_test_51OdgVvEFMXlO8edaRRR5R3dq7CPNvWDvoYKsrUWmSjVmnhbB7ad2JFUV2RT6vtiKzjpHquxy08TwSdo6Isvlm3XL00GrEsctoZ"; // Replace with your Stripe secret key

        [HttpPost]
        [Route("create-checkout-session")]
        public ActionResult CreateCheckoutSession([FromBody] List<StripeItems> cartItems)
        {
            // Initialize Stripe with your secret key
            StripeConfiguration.ApiKey = StripeSecretKey;

            var lineItems = new List<SessionLineItemOptions>();

            // Create line items for each cart item
            foreach (var cartItem in cartItems)
            {
                lineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = cartItem.Name
                        },
                        UnitAmount = (long?)(cartItem.Price * 100) // Stripe requires the amount in cents
                    },
                    Quantity = cartItem.Quantity
                });
            }

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = "http://localhost:3000/success",
                CancelUrl = "http://localhost:3000/cancel",
            };

            var service = new SessionService();
            var session = service.Create(options);

            return Ok(new { sessionId = session.Id });
        }

        // Add more actions as needed, such as webhook handler for payment events
    }
}
