﻿using System;
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
    public class SubscriptionsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ISubscriptionRepository _subscription;

        public SubscriptionsController(AppDbContext context, ISubscriptionRepository subscription)
        {
            _context = context;
            _subscription = subscription;
        }

        // GET: Admin/Subscriptions
        public async Task<IActionResult> Index()
        {
            var susubscription = await _subscription.GetAllSubscriptionsAsync();
            return View(susubscription);
        }

        // GET: Admin/Subscriptions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Subscriptions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SubscriptionId,Name,Price,Duration,MaxAds,IsPriorityAd")] Subscription subscription)
        {
            if (ModelState.IsValid)
            {
                await _subscription.AddSubscriptionAsync(subscription);
                return RedirectToAction(nameof(Index));
            }
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
            return View(subscription);
        }

        // POST: Admin/Subscriptions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SubscriptionId,Name,Price,Duration,MaxAds,IsPriorityAd")] Subscription subscription)
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
            return View(subscription);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var user = _context.Users.FirstOrDefault(x => x.UserId == id);
            var userSubscriptions = _context.UserSubscriptions.FirstOrDefault(x => x.UserSubscriptionId == id);
            var transactions = _context.Transactions.FirstOrDefault(x => x.TransactionId == id);
            if (user == null && userSubscriptions == null && transactions == null)
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