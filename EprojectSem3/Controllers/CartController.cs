using DataAccessLayer_DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Realtors_Portal.Services;
using System.Security.Claims;

namespace Realtors_Portal.Controllers
{
	public class CartController : Controller
	{
		private readonly PaypalClient _paypalClient;
		private readonly AppDbContext _context;
		public CartController(PaypalClient paypalClient, AppDbContext context) {
			_paypalClient=paypalClient;
			_context=context;
		}

		[Authorize(AuthenticationSchemes = "MyAuthenticationSchema")]
		[Authorize(AuthenticationSchemes = "MyAppAuthenticationAdmin")]
		[HttpPost("/Cart/create-paypal-order")]
		public async Task<IActionResult> CreatePayPalOrder(int id,CancellationToken cancellationToken)
		{
			var buySub = await _context.Subscriptions.SingleOrDefaultAsync(s=> s.SubscriptionId== id);
			var totalPrice = buySub.Price.ToString();
			var currency = "USD";
			var codeOrder = "Od" + DateTime.Now.Ticks.ToString();

			try
			{
				var response = await _paypalClient.CreateOrder(totalPrice, currency, codeOrder);
				return Ok(response);
			}
			catch (Exception ex)
			{
				var error = new { ex.GetBaseException().Message };
				return BadRequest(error);
			}
		}

		[Authorize(AuthenticationSchemes = "MyAuthenticationSchema")]
		[Authorize(AuthenticationSchemes = "MyAppAuthenticationAdmin")]
		[HttpPost("/Cart/capture-paypal-order")]
		public async Task<IActionResult> CapturePalpalOrder(string orderID,int subcriptionId, CancellationToken cancellationToken)
		{

			try
			{
				var response = await _paypalClient.CaptureOrder(orderID);

				if (response.status == "COMPLETED")
				{
					var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

					var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId.ToString() == userId);

					if (user == null)
					{
						return BadRequest(new { message = "User not found." });
					}

					var subscription = await _context.Subscriptions.FirstOrDefaultAsync(s => s.SubscriptionId == subcriptionId);

					if (subscription == null)
					{
						return BadRequest(new { message = "Subscription not found." });
					}

					// check UserSubscription 
					var existingUserSubscription = await _context.UserSubscriptions
						.FirstOrDefaultAsync(us => us.UserId == user.UserId && us.SubscriptionId == subscription.SubscriptionId);

					if (existingUserSubscription != null)
					{
						// update EndDate
						existingUserSubscription.EndDate = DateTime.Now.AddDays(subscription.Duration); /
						_context.UserSubscriptions.Update(existingUserSubscription);
						await _context.SaveChangesAsync();

						return Ok(new { message = "Payment successful, subscription updated." });
					}
					else
					{
						// create new UserSubscription
						var newUserSubscription = new UserSubscription
						{
							UserId = user.UserId,
							SubscriptionId = subscription.SubscriptionId,
							StartDate = DateTime.Now,
							EndDate = DateTime.Now.AddDays(subscription.Duration)
						};

						_context.UserSubscriptions.Add(newUserSubscription);
						await _context.SaveChangesAsync();

						return Ok(new { message = "Payment successful, and new subscription created." });
					}
				}
				else
				{
					return BadRequest(new { message = "Payment failed or not completed." });
				}
			}
			catch (Exception ex)
			{
				var error = new { ex.GetBaseException().Message };
				return BadRequest(error);
			}

		}

		[Authorize(AuthenticationSchemes = "MyAuthenticationSchema")]
		[Authorize(AuthenticationSchemes = "MyAppAuthenticationAdmin")]
		[HttpGet("/Cart/Success")]
		public IActionResult CheckOutSuccess()
		{
			return View();
		}



	}
}
