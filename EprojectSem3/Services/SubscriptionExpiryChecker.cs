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

                // Simulate a specific date (e.g., January 27, 2025)
                DateTime simulatedNow = new DateTime(2025, 1, 27);

                var expiringSubscriptions = await _context.UserSubscriptions
                 .Include(us => us.User)
                 .Include(us => us.Subscription)
                 .Where(us => us.EndDate <= DateTime.Now.Date && us.EndDate >= DateTime.Now.AddDays(-3).Date) // Expired today or within the last 3 days
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

                // find listing expired
                //DateTime simulatedNow = new DateTime(2025, 5, 17);
                var expiredListings = await _context.Listings
               .Where(listing => _context.UserSubscriptions
                   .Any(subscription => subscription.UserId == listing.UserId
                                        && subscription.EndDate.Date < DateTime.Now.Date)
                   && listing.Status == 1)
               .ToListAsync();

                // Update listing expired
                foreach (var listing in expiredListings)
                {
                    listing.Status = 0;
                }

                // Save change
                await _context.SaveChangesAsync();
            }
        }

    }
}
