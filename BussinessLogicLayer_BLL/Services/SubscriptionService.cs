using DataAccessLayer_DAL.Models;
using DataAccessLayer_DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogicLayer_BLL.Services
{
    public class SubscriptionService : ISubscriptionRepository
    {
        private readonly AppDbContext _context;

        public SubscriptionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddSubscriptionAsync(Subscription subscription)
        {
            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSubscriptionAsync(int subscriptionId)
        {
            var subscription = await _context.Subscriptions.FindAsync(subscriptionId);
            if( subscription != null)
            {
                _context.Subscriptions.Remove(subscription);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Subscription>> GetAllSubscriptionsAsync(string? keyword)
        {
            var subscriptions = _context.Subscriptions.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                subscriptions = subscriptions.Where(c => c.Name.Contains(keyword));
            }
            return await subscriptions.OrderByDescending(s=>s.SubscriptionId).ToListAsync();
        }

        public async Task<Subscription?> GetSubscriptionByIdAsync(int? id)
        {
            return await _context.Subscriptions.FirstOrDefaultAsync(x => x.SubscriptionId == id);
        }

        public async Task UpdateSubscriptionAsync(Subscription subscription)
        {
            _context.Subscriptions.Update(subscription);
            await _context.SaveChangesAsync();
        }
    }
}
