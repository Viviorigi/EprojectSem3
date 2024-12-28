using DataAccessLayer_DAL.Models;
using DataAccessLayer_DAL.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Realtors_Portal.Models;
using System.Reflection;
using System.Security.Claims;

namespace EprojectSem3.Controllers
{
	[Authorize(AuthenticationSchemes = "MyAuthenticationSchema")]
	public class UserController : Controller
    {
        private readonly AppDbContext _context;
		private readonly IListingRepository _listingRepository;

		public UserController(AppDbContext context,IListingRepository listingRepository)
        {
            _context = context;
            _listingRepository = listingRepository;
        }

        public async Task<IActionResult> Index()
        {
            var id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (id == null)
            {
                return NotFound();
            }

            var count_listing = _context.Listings.Where(x => x.Status == 1).Where(x => x.UserId == id).Count();
            ViewBag.CountListing = count_listing;

            var user_sub = _context.UserSubscriptions.Where(x =>x.UserId == id).Count();
            ViewBag.UserSub = user_sub;

            var bookmark= _context.BookMarks.Where(x => x.UserId == id).Count();
			ViewBag.countBookMark = bookmark;

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

        [HttpPost]
        [Route("GetChartData3")]
        public async Task<IActionResult> GetChartData3()
        {
            var id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var data = _context.Listings
            .Where(x => x.Status == 1 && x.UserId == id && x.CreatedAt.HasValue)
            .GroupBy(x => new { Year = x.CreatedAt.Value.Year, Month = x.CreatedAt.Value.Month })
            .Select(x => new
            {
                date = x.Key.Year + "-" + x.Key.Month.ToString("D2"),
                listing = x.Count()

            })
            .ToList();

            return Json(data);
        }

        

        public async Task<IActionResult> Detail()
        {
            var id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
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
        public async Task<IActionResult> Detail(User user,
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

            var id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

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
                    existingUser.UpdatedAt = DateTime.Now;
                    _context.Users.Update(existingUser);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Detail");
                }
            }
            return View(user);
        }

        public async Task<IActionResult> Listing(int? page , string? search)
        {
			var id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
			if (id == null)
			{
				return NotFound();
			}
            var userListings = await _listingRepository.GetMyListingAsync(id , page , search);

             
			return View(userListings);
        }

        public async Task<IActionResult> Subscription()
        {
            var id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (id == null)
            {
                return NotFound();
            }
            var subscription = await _context.UserSubscriptions.Include(u=>u.User).Include(u => u.Subscription).Where(u=> u.UserId == id).OrderByDescending(u=>u.Subscription.Price).ToListAsync();


            return View(subscription);
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "MyAuthenticationSchema")]
        public async Task<IActionResult> ChangePassword(ChangePassword model)
        {
            if (ModelState.IsValid) {
                var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
                var user = await _context.Users.SingleOrDefaultAsync(u=> u.UserId==userId);

                if (user == null)
                {
                    // Handle case when user is not found
                    ModelState.AddModelError(string.Empty, "User not found.");
                    return View(model);
                }

                if (!BCrypt.Net.BCrypt.Verify(model.OldPassword, user.Password)) 
                {
                    ModelState.AddModelError(string.Empty, "The old password is incorrect.");
                    return View(model);
                }

                if (model.NewPassword != model.ConfirmPassword)
                {
                    ModelState.AddModelError("PasswordMismatch", "Passwords do not match.");
                    return View(model);
                }

                user.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                await HttpContext.SignOutAsync("MyAuthenticationSchema");
                return RedirectToAction("Login","Home");
            }
            return View(model);
        }

		[Authorize(AuthenticationSchemes = "MyAuthenticationSchema")]
		public async Task<IActionResult> Delete(int id)
		{
			var listing = await _listingRepository.GetListingByIdAsync(id);
			if (listing != null)
			{
				await _listingRepository.DeleteListingAsync(id);

				TempData["msg"] = "Delete Category successful";
				return RedirectToAction("Listing", "User");

			}
			TempData["err"] = "Existing posts cannot be deleted.";
			return View("Listing");
		}

        [Authorize(AuthenticationSchemes = "MyAuthenticationSchema")]
        public async Task<IActionResult> BookMark()
        {
            var id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (id == null)
            {
                return NotFound();
            }
            var bookMarks = await _context.BookMarks.Include(u => u.User).Include(u => u.Listing).Where(u => u.UserId == id).ToListAsync();


            return View(bookMarks);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = "MyAuthenticationSchema")]
        public async Task<IActionResult> DeleteToBookmark(int listingId)
        {

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // Look for an existing bookmark
            var existingBookmark = await _context.BookMarks
                .FirstOrDefaultAsync(b => b.UserId == userId && b.ListingId == listingId);

            if (existingBookmark != null)
            {
                // If the bookmark exists, remove it (delete it)
                _context.BookMarks.Remove(existingBookmark);
                await _context.SaveChangesAsync();
                TempData["msg"] = "Listing removed from bookmarks successful.";
                TempData["AlertType"] = "success";
            }
            else
            {
                // If the bookmark does not exist, show a message
                TempData["Message"] = "Listing not found in bookmarks.";
            }

            // Redirect to the details page of the listing
            return RedirectToAction("BookMark");
        }

    }
	
}
