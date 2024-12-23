using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer_DAL.Models;
using DataAccessLayer_DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace EprojectSem3.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SubscriptionsController : BaseController
    {
        private readonly AppDbContext _context;
        private readonly ISubscriptionRepository _subscription;

        public SubscriptionsController(AppDbContext context, ISubscriptionRepository subscription)
        {
            _context = context;
            _subscription = subscription;
        }

        // GET: Admin/Subscriptions
        public async Task<IActionResult> Index(string? keyword)
        {
            var subscription = await _subscription.GetAllSubscriptionsAsync(keyword);
            ViewData["Keyword"] = keyword;
            return View(subscription);
        }

        // GET: Admin/Subscriptions/Create
        public IActionResult Create()
        {
            ViewBag.IsAgent = new SelectList(new[]
              {
                new { Value = true, Text = "Yes" },
                new { Value = false, Text = "No" }
            }, "Value", "Text");

            return View();
        }

        // POST: Admin/Subscriptions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SubscriptionId,Name,Price,Duration,MaxAds,IsAgent")] Subscription subscription)
        {
            if (ModelState.IsValid)
            {
                await _subscription.AddSubscriptionAsync(subscription);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.IsAgent = new SelectList(new[]
            {
                new { Value = true, Text = "Yes" },
                new { Value = false, Text = "No" }
            }, "Value", "Text");

            return View(subscription);
        }

        // GET: Admin/Subscriptions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subscription = await _subscription.GetSubscriptionByIdAsync(id);
            if (subscription == null)
            {
                return NotFound();
            }
            ViewBag.IsAgent = new SelectList(new[]
          {
                new { Value = true, Text = "Yes" },
                new { Value = false, Text = "No" }
            }, "Value", "Text");

            return View(subscription);
        }

        // POST: Admin/Subscriptions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SubscriptionId,Name,Price,Duration,MaxAds,IsAgent")] Subscription subscription)
        {
            if (id != subscription.SubscriptionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _subscription.UpdateSubscriptionAsync(subscription);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubscriptionExists(subscription.SubscriptionId))
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
            ViewBag.IsAgent = new SelectList(new[]
            {
                new { Value = true, Text = "Yes" },
                new { Value = false, Text = "No" }
            }, "Value", "Text");

            return View(subscription);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var user = _context.Users.FirstOrDefault(x => x.UserId == id);
            var userSubscriptions = _context.UserSubscriptions.FirstOrDefault(x => x.UserSubscriptionId == id);
            if (user == null && userSubscriptions == null)
            {
                var subscriptions = _context.Subscriptions.FirstOrDefault(x => x.SubscriptionId == id);
                if (subscriptions != null)
                {
                    await _subscription.DeleteSubscriptionAsync(id);
                    ViewBag.message = "Delete subscriptions successful";
                    return RedirectToAction("Index");
                }
            }
            ViewBag.message = "Existing posts cannot be deleted.";
            return View("index");
        }

        private bool SubscriptionExists(int id)
        {
            return _context.Subscriptions.Any(e => e.SubscriptionId == id);
        }
    }
}
