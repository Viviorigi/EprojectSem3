
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
            return View();
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
