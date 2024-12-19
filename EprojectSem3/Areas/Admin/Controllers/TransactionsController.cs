using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer_DAL.Models;

namespace Realtors_Portal.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TransactionsController : Controller
    {
        private readonly AppDbContext _context;

        public TransactionsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Transactions
        public async Task<IActionResult> Index(string keyword = "", DateTime? startDate = null, DateTime? endDate = null)
        {
            var appDbContext = _context.Transactions
                .Include(t => t.Subscription)
                .Include(t => t.User)
                .AsQueryable();

            // Apply keyword filter
            if (!string.IsNullOrEmpty(keyword))
            {
                appDbContext = appDbContext.Where(t =>
                    t.User.FullName.Contains(keyword) ||
                    t.Subscription.Name.Contains(keyword) ||
                    t.User.Email.Contains(keyword)
                    );
            }

            // Apply date range filter
            if (startDate.HasValue)
            {
                appDbContext = appDbContext.Where(t => t.TransactionDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                appDbContext = appDbContext.Where(t => t.TransactionDate <= endDate.Value);
            }

            // Pass filter values back to the view
            ViewData["Keyword"] = keyword;
            ViewData["StartDate"] = startDate?.ToString("yyyy-MM-dd");
            ViewData["EndDate"] = endDate?.ToString("yyyy-MM-dd");

            return View(await appDbContext.ToListAsync());
        }

        // GET: Admin/Transactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .Include(t => t.Subscription)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.TransactionId == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // GET: Admin/Transactions/Create
        public IActionResult Create()
        {
            ViewData["SubscriptionId"] = new SelectList(_context.Subscriptions, "SubscriptionId", "Name");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Address");
            return View();
        }

        // POST: Admin/Transactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TransactionId,UserId,SubscriptionId,Amount,TransactionDate,PaymentDate,IsPaid")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                _context.Add(transaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SubscriptionId"] = new SelectList(_context.Subscriptions, "SubscriptionId", "Name", transaction.SubscriptionId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Address", transaction.UserId);
            return View(transaction);
        }

        // GET: Admin/Transactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }
            ViewData["SubscriptionId"] = new SelectList(_context.Subscriptions, "SubscriptionId", "Name", transaction.SubscriptionId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Address", transaction.UserId);
            return View(transaction);
        }

        // POST: Admin/Transactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TransactionId,UserId,SubscriptionId,Amount,TransactionDate,PaymentDate,IsPaid")] Transaction transaction)
        {
            if (id != transaction.TransactionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(transaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionExists(transaction.TransactionId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["SubscriptionId"] = new SelectList(_context.Subscriptions, "SubscriptionId", "Name", transaction.SubscriptionId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Address", transaction.UserId);
            return View(transaction);
        }

        // GET: Admin/Transactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .Include(t => t.Subscription)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.TransactionId == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // POST: Admin/Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransactionExists(int id)
        {
            return _context.Transactions.Any(e => e.TransactionId == id);
        }
    }
}
