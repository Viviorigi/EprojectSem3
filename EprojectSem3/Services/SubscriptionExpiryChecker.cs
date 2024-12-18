using Azure.Core;
using BussinessLogicLayer_BLL.Services;
using DataAccessLayer_DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Policy;
namespace Realtors_Portal.Services
{
    public class SubscriptionExpiryChecker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly EmailService _emailService;


        public SubscriptionExpiryChecker(IServiceScopeFactory scopeFactory, EmailService emailService)
        {
            _scopeFactory = scopeFactory;
            _emailService = emailService;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CheckAndNotifyExpiringSubscriptions();
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken); // Runs daily
            }
        }

        private async Task CheckAndNotifyExpiringSubscriptions()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // Get subscriptions expiring in the next 5 days
                //DateTime simulatedNow = new DateTime(2025, 2, 13);  test time
                var expiringSubscriptions = await _context.UserSubscriptions
                    .Include(us => us.User)
                    .Include(us => us.Subscription)
                    .Where(us => us.EndDate.Date <= DateTime.Now.AddDays(5))
                    .ToListAsync();

                foreach (var subscription in expiringSubscriptions)
                {
                    var user = subscription.User;
                    var daysLeft = (subscription.EndDate.Date - DateTime.Now.Date).Days;

                    var subject = "Your Ad Package is About to Expire";
                    var link = "https://localhost:44369/Home/Pricing";
                    var body = $@"
                        Dear {user.FullName},<br/>
                        Your ad package <strong>{subscription.Subscription.Name}</strong> is expiring in <strong>{daysLeft} day(s)</strong> on <strong>{subscription.EndDate:yyyy-MM-dd}</strong>.<br/>
                        Please renew your subscription to continue enjoying the benefits.<br/><br/>
                        <a href='{link}'>Renew Now</a><br/><br/>
                        Thank you!";

                    await _emailService.SendEmailAsync(user.Email, subject, body);
                }
            }
        }

    }
}
