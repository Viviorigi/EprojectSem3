using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer_DAL.Models;
using DataAccessLayer_DAL.Repositories;
using System.Reflection;

namespace EprojectSem3.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : BaseController
    {
        private readonly IUserRepository _userRepository;
        private readonly AppDbContext _context;

        public UsersController(IUserRepository userRepository, AppDbContext context)
        {
            _userRepository = userRepository;
            _context = context;
        }

        // GET: Admin/Users
        public async Task<IActionResult> Index(string keyword = "")
        {
            var users = await _userRepository.GetAllUsersAsync(keyword);
            ViewData["Keyword"] = keyword; 
            return View(users);
        }

        // GET: Admin/Users/Create
        public IActionResult Create()
        {
            ViewBag.Role = new SelectList(new[]
            {
                new { Value = 0, Text = "Private Seller" },
                new { Value = 1, Text = "Agent" },
                new { Value = 2, Text = "Admin" }
            }, "Value", "Text");

            return View();
        }

        // POST: Admin/Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,FullName,Email,Password,PhoneNumber,Address,Role,Status")] User user, IFormFile? file)
        {

            try
            {
                if (file != null && file.Length > 0)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", file.FileName);
                    user.Image = "images/" + file.FileName;

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                }

                if (user.Image ==  null)
                {
                    user.Image = "images/users.png";
                }

                if (ModelState.IsValid)
                {
                    user.CreatedAt = DateTime.Now; 
                    await _userRepository.AddUserAsync(user);

                    //add data to Statistical
                    var Statistical = await _context.Statisticals
                                    .FirstOrDefaultAsync(x => x.CreatedAt.Value.Date == user.CreatedAt.Value.Date);

                    if (Statistical != null)
                    {
                        Statistical.UserCount += 1;
                        _context.Update(Statistical);
                    }
                    else
                    {
                        int UserCount = 1;

                        Statistical = new Statistical
                        {
                            TransactionCount = 0,
                            PriceCount = 0,
                            CreatedAt = user.CreatedAt,
                            ListingCount = 0,
                            UserCount = UserCount
                        };
                        _context.Add(Statistical);
                    }

                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException?.Message.Contains("UNIQUE KEY constraint") == true)
                {
                    ModelState.AddModelError("Email", "The email address is already in use.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again.");
                }
            }
            ViewBag.Role = new SelectList(new[]
            {
                new { Value = 0, Text = "Private Seller" },
                new { Value = 1, Text = "Agent" },
                new { Value = 2, Text = "Admin" }
            }, "Value", "Text");

            return View(user);
        }

        // GET: Admin/Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewBag.Role = new SelectList(new[]
            {
                new { Value = 0, Text = "Private Seller" },
                new { Value = 1, Text = "Agent" },
                new { Value = 2, Text = "Admin" }
            }, "Value", "Text");
            ViewBag.Status = new SelectList(new[]
          {
                new { Value = 0, Text = "InActive" },
                new { Value = 1, Text = "Active" }
            }, "Value", "Text");
            return View(user);
        }

        // POST: Admin/Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user, IFormFile? file)
        {
            if (file != null && file.Length > 0)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", file.FileName);
                user.Image = "images/" + file.FileName;

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
            }

            if (id != user.UserId)
            {
                return NotFound();
            }

            // Check if the email already exists in the database (excluding the current user)
            var existingUserWithEmail = await _context.Users
                                                      .FirstOrDefaultAsync(u => u.Email == user.Email && u.UserId != id);

            // If email is already in use by another user, add an error
            if (existingUserWithEmail != null)
            {
                ModelState.AddModelError("Email", "The email address is already in use.");
            }

            // If there are no validation errors, proceed with updating the user data
            if (ModelState.IsValid)
            {
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);

                if (existingUser != null)
                {
                    existingUser.FullName = user.FullName;
                    existingUser.Email = user.Email;
                    existingUser.Address = user.Address;
                    existingUser.PhoneNumber = user.PhoneNumber;
                    existingUser.Role = user.Role;
                    existingUser.Status = user.Status;
                    existingUser.Image = user.Image;
                    existingUser.UpdatedAt = DateTime.Now;

                    // Check if password is being changed and hash it
                    if (!BCrypt.Net.BCrypt.Verify(user.Password, existingUser.Password))
                    {
                        existingUser.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                    }

                    try
                    {
                        await _userRepository.UpdateUserAsync(existingUser);

                        //add data to Statistical
                        var Statistical = await _context.Statisticals
                                        .FirstOrDefaultAsync(x => x.CreatedAt.Value.Date == user.CreatedAt.Value.Date);

                        if (Statistical != null)
                        {
                            Statistical.UserCount += 1;
                            _context.Update(Statistical);
                        }
                        else
                        {
                            int UserCount = 1;

                            Statistical = new Statistical
                            {
                                TransactionCount = 0,
                                PriceCount = 0,
                                CreatedAt = existingUser.CreatedAt,
                                ListingCount = 0,
                                UserCount = UserCount
                            };
                            _context.Add(Statistical);
                        }

                        return RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!UserExists(user.UserId))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }

            // Repopulate the ViewBag with data for the form (if there are validation errors)
            ViewBag.Role = new SelectList(new[]
                    {
                new { Value = 0, Text = "Private Seller" },
                new { Value = 1, Text = "Agent" },
                new { Value = 2, Text = "Admin" }
                }, "Value", "Text");

            ViewBag.Status = new SelectList(new[]
            {
                new { Value = 0, Text = "Inactive" },
                new { Value = 1, Text = "Active" }
            }, "Value", "Text");

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(int id)
		{
			
				// Check for related listings (posts)
				var listing = await _context.Listings.FirstOrDefaultAsync(x => x.UserId == id);
				if (listing != null)
				{
					// If listings exist, prevent deletion and set message
					TempData["Message"] = "User cannot be deleted because they have existing posts.";
					return RedirectToAction("Index");
				}

				// Check for the user and their subscriptions
				var user = await _context.Users
					.Include(u => u.Subscription)
					.FirstOrDefaultAsync(u => u.UserId == id);

				if (user == null)
				{
					// If the user doesn't exist, redirect with an error message
					TempData["Message"] = "User not found.";
					return RedirectToAction("Index");
				}

                var subscriptionsExist = _context.UserSubscriptions.Any(s => s.UserId == id);
                if (subscriptionsExist)
                {
                    // If subscriptions exist, prevent deletion and set message
                    TempData["ErrorMessage"] = "User cannot be deleted because they have active subscriptions.";
                    return RedirectToAction("Index");
                }

            var transactions = _context.Transactions.Any(s => s.UserId == id);
            if (transactions)
            {
                // If subscriptions exist, prevent deletion and set message
                TempData["ErrorMessage"] = "User cannot be deleted because they have transactions .";
                return RedirectToAction("Index");
            }

            // Delete the user
            await _userRepository.DeleteUserAsync(id);
				TempData["Message"] = "User deleted successfully.";
				return RedirectToAction("Index");
			
		
		}



		private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
