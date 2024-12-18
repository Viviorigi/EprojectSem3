using DataAccessLayer_DAL.Models;
using DataAccessLayer_DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace EprojectSem3.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var u = await _context.Users
                .Include(u => u.Subscription)
                .Include(u => u.Listings)
                .FirstOrDefaultAsync(m => m.UserId == id);

            if (u == null)
            {
                return NotFound();
            }
            return View(u);
        }

        //Update&Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(int id, User user,
            IFormFile? file)
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
                    return RedirectToAction("Index");
                }
            }
            return View(user);
        }

        public IActionResult Listing()
        {
            return View();
        }

        public IActionResult Password()
        {
            return View();
        }
    }
}
