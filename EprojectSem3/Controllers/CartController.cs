using BussinessLogicLayer_BLL.Services;
using DataAccessLayer_DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
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
        private readonly EmailService _emailService;
        public CartController(PaypalClient paypalClient, AppDbContext context,EmailService emailService) {
			_paypalClient=paypalClient;
			_context=context;
			_emailService=emailService;
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var newTransaction = new Transaction
			{
				UserId = int.Parse(userId),
				SubscriptionId = buySub.SubscriptionId,
				Amount = buySub.Price,
				TransactionDate = DateTime.Now,
                IsPaid= false
            };
            await _context.Transactions.AddAsync(newTransaction);
			await _context.SaveChangesAsync();

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
                Console.WriteLine(response);

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

                    var transaction = await _context.Transactions
                    .FirstOrDefaultAsync(t => t.UserId == user.UserId && t.SubscriptionId == subcriptionId && !t.IsPaid);

                    if (transaction != null)
                    {
                        transaction.IsPaid = true;
						transaction.PaymentDate = DateTime.Now;
                        _context.Transactions.Update(transaction);
                        await _context.SaveChangesAsync();
                    }

					if (transaction.IsPaid == true)
					{
						var Statistical = await _context.Statisticals
							.FirstOrDefaultAsync(x => x.CreatedAt.Value.Date == transaction.PaymentDate.Value.Date);

						if(Statistical != null)
						{
							Statistical.TransactionCount += 1;
							Statistical.PriceCount += transaction.Amount;
							_context.Update(Statistical);
						}
						else
						{
							int TransactionCount = 1;
							decimal PriceCount = transaction.Amount;

							Statistical = new Statistical
							{
								TransactionCount = TransactionCount,
								PriceCount = PriceCount,
                                CreatedAt = transaction.PaymentDate,
								ListingCount = 0,
								UserCount = 0
                            };
							_context.Add(Statistical);
                            _context.SaveChanges();
                        }
                    }

                    // check UserSubscription 
                    var existingUserSubscription = await _context.UserSubscriptions
						.FirstOrDefaultAsync(us => us.UserId == user.UserId && us.SubscriptionId == subcriptionId);

					if (existingUserSubscription != null)
					{
                        // update EndDate
                        existingUserSubscription.EndDate = existingUserSubscription.EndDate.AddDays(subscription.Duration);
                        _context.UserSubscriptions.Update(existingUserSubscription);
						await _context.SaveChangesAsync();
                        var subject = "Subscription Extended Successfully";
                        var body = $"Dear {user.FullName},<br/>Your subscription has been successfully extended until <strong>{existingUserSubscription.EndDate:yyyy-MM-dd}</strong>.<br/>Thank you for your continued support!";
                        await _emailService.SendEmailAsync(user.Email, subject, body);
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
                        var subject = "Subscription Activated";
                        var body = $"Dear {user.FullName},<br/>Your subscription has been activated and is valid until <strong>{newUserSubscription.EndDate:yyyy-MM-dd}</strong>.<br/>Thank you for joining us!";
                        await _emailService.SendEmailAsync(user.Email, subject, body);

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
