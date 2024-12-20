
using DataAccessLayer_DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace EprojectSem3.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var count_listing = _context.Listings.Where(l => l.Status == 1).Count();
            var count_user = _context.Users.Count();
            var count_user_saler = _context.Users.Where(u => u.Role == 0).Count();
            var count_user_agent = _context.Users.Where(u => u.Role == 1).Count();
            var count_usersubscription = _context.UserSubscriptions.Count();
            var count_transaction = _context.Transactions.Where(t => t.IsPaid == true).Count();
            ViewBag.CountListing = count_listing;
            ViewBag.CountUser = count_user;
            ViewBag.CountUser0 = count_user_saler;
            ViewBag.CountUser1 = count_user_agent;
            ViewBag.CountUserSubscription = count_usersubscription;
            ViewBag.CountTransaction = count_transaction;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetChartData()
        {
            var data = _context.Transactions
            .Select(x => new
            {
                date = x.PaymentDate.Value.ToString("yyyy-MM-dd"),
                price = x.Amount
            })
            .ToList();

            return Json(data);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var u = await _context.Users.FirstOrDefaultAsync(m => m.UserId == id);

            if (u == null)
            {
                return NotFound();
            }
            return View(u);
        }

        //Update&Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Detail(int id, User user, IFormFile? file)
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
                    existingUser.Password = user.Password;
                    existingUser.Image = user.Image;
                    _context.Users.Update(existingUser);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Detail");
                }
            }
            return View(user);
        }
    }
}
