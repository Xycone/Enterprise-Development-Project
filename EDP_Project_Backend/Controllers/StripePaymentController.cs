using Microsoft.CodeAnalysis.Scripting;
using System;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using EDP_Project_Backend.Models; // Import your data models
using Stripe.Checkout;
using Stripe.Issuing;
using System.Security.Claims;
using AutoMapper;
using Newtonsoft.Json;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace EDP_Project_Backend.Controllers
{
    [ApiController]
    [Route("/stripepayment/api")]
    public class StripePaymentController : ControllerBase
    {
		private readonly MyDbContext _context;

        public StripePaymentController(MyDbContext context, IMapper mapper)
        {
            _context = context;
        }

		private int GetUserId()
		{
			return Convert.ToInt32(User.Claims
			.Where(c => c.Type == ClaimTypes.NameIdentifier)
			.Select(c => c.Value).SingleOrDefault());
		}

		private readonly string StripeSecretKey = "sk_test_51OdgVvEFMXlO8edaRRR5R3dq7CPNvWDvoYKsrUWmSjVmnhbB7ad2JFUV2RT6vtiKzjpHquxy08TwSdo6Isvlm3XL00GrEsctoZ"; // Replace with your Stripe secret key

        [HttpPost]
		[Authorize]
        [Route("create-checkout-session")]
        public ActionResult CreateCheckoutSession([FromBody] CheckoutRequest request)
        {
			List<StripeItems> cartItems = request.SelectedCartItems;
			int? appliedVoucher = request.SelectedVoucherId;
			var id = GetUserId();

			// Initialize Stripe with your secret key
			StripeConfiguration.ApiKey = StripeSecretKey;

            var lineItems = new List<SessionLineItemOptions>();

            // Original Cart Value
			var totalCartValue = cartItems.Sum(item => item.Price * item.Quantity);
            if (totalCartValue <= 0)
            {
				return BadRequest("totalCartValue cannot be smaller or equal to 0.");
			}

			// Total Cart Quantity
			int totalQuantityInCart = cartItems.Sum(item => item.Quantity);

			// Discount Amount 
			float discountAmount = totalCartValue;

			if (appliedVoucher.HasValue)
            {
				var voucher = _context.Vouchers.FirstOrDefault(v => v.UserId == id && v.Id == appliedVoucher);

				// Checks if the voucher applied actually belongs to the user logged in
				if (voucher == null)
                {
                    // Voucher not found or doesn't belong to the user, handle accordingly
                    return BadRequest("Invalid voucher or voucher does not belong to the user.");
                }


				// Retrieve the voucher's associated perk
				var voucherInfo = _context.Perks.FirstOrDefault(p => p.Id == voucher.PerkId);

				if (voucherInfo == null)
				{
					return BadRequest("Invalid voucher or voucher does not belong to the user.");
				}

				if (totalCartValue < voucherInfo.MinSpend)
				{
					return BadRequest("Total cart value does not meet the minimum spend requirement for the voucher to apply.");
				}

                if (totalQuantityInCart <= voucherInfo.MinGroupSize)
                {
                    return BadRequest("Total cart item quantity does not meet the minimum group size for the voucher to apply.");
                }

				// Voucher Discount logic

				// Apply voucher discount based on the voucher type
				if (voucherInfo.FixedDiscount == 0)
				{
					discountAmount -= totalCartValue * (voucherInfo.PercentageDiscount / 100);
				}
				else if (voucherInfo.PercentageDiscount == 0)
				{
					discountAmount -= voucherInfo.FixedDiscount;
				}
			}

			lineItems.Add(new SessionLineItemOptions
			{
				PriceData = new SessionLineItemPriceDataOptions
				{
					Currency = "sgd",
					ProductData = new SessionLineItemPriceDataProductDataOptions
					{
						Name = "Total Amount Payable"
					},
					UnitAmount = (long)(discountAmount * 100) // Convert total amount to cents
				},
				Quantity = 1 // Only one line item for total amount payable
			});


			var options = new SessionCreateOptions
			{
				PaymentMethodTypes = new List<string>
				{
					"card",
					"grabpay",
					"paynow",
				},
				LineItems = lineItems,
				Mode = "payment",
				SuccessUrl = "http://localhost:3000/success?" + "&appliedVoucher=" + appliedVoucher + "&cartItems=" + JsonConvert.SerializeObject(cartItems),
				CancelUrl = "http://localhost:3000/cart",
				Metadata = new Dictionary<string, string>
				{
					{ "appliedVoucher", appliedVoucher?.ToString() },
					{ "cartItems", JsonConvert.SerializeObject(cartItems) },
					{ "amountPayable", discountAmount.ToString() },
					{ "userId", id.ToString() }
				}
			};

			var service = new SessionService();
            var session = service.Create(options);


			return Ok(new { sessionId = session.Id, appliedVoucher, cartItems });
        }

		// Add more actions as needed, such as webhook handler for payment events
		[HttpPost]
		[Route("stripe-webhook")]
		public async Task<IActionResult> StripeWebhook()
		{
			// Parse the incoming webhook event
			var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
			var stripeEvent = EventUtility.ParseEvent(json);

			// Handle the event based on its type
			if (stripeEvent.Type == Events.CheckoutSessionCompleted)
			{
				var session = stripeEvent.Data.Object as Session;

				if (session != null)
				{
					// Retrieve metadata from the Stripe event
					var metadata = session.Metadata;
					var appliedVoucher = metadata["appliedVoucher"];
					var cartItemsJson = metadata["cartItems"];
					var id = metadata["userId"];
					var cartItems = JsonConvert.DeserializeObject<List<StripeItems>>(cartItemsJson);
					var totalSpendings = cartItems.Sum(item => item.Price * item.Quantity);
					var totalBookings = cartItems.Sum(item => item.Quantity);
					

					// Updates total spendings and total bookings
					var user = _context.Users.Find(Convert.ToInt32(id));
					if (user != null)
					{
						user.TotalSpent += totalSpendings;
						user.TotalBookings += totalBookings;
					}
					_context.SaveChanges();

					// Removes used voucher from db
					if (appliedVoucher != null)
					{
						var myVoucher = _context.Vouchers.Find(Convert.ToInt32(appliedVoucher));
						if (myVoucher != null)
						{
							_context.Vouchers.Remove(myVoucher);
						}
						_context.SaveChanges();
					}






				}

			}

			// Return a response to acknowledge receipt of the event
			return Ok();
		}
	}
}
