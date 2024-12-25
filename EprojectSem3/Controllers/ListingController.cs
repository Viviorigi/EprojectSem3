using DataAccessLayer_DAL.Models;
using DataAccessLayer_DAL.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EprojectSem3.Controllers
{
	
	public class ListingController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IListingRepository _listingRepository;
        private readonly IImageRepository _imageRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICityRepository _cityRepository;

        public ListingController(AppDbContext context , IListingRepository listingRepository , IImageRepository imageRepository , ICategoryRepository categoryRepository , ICityRepository cityRepository)
        {
            _cityRepository = cityRepository;
            _categoryRepository = categoryRepository;
            _imageRepository = imageRepository;
            _listingRepository = listingRepository;
            _context = context;
        }
        public async Task<IActionResult> Index(int? page, string? keyword, int? cateId, int? cityId, double? minPrice, double? maxPrice, string? sort, string? role)
        {
            ViewBag.categories = new SelectList(await _categoryRepository.GetCategoryAsync(), "CategoryId", "Name", cateId);
            ViewBag.city = new SelectList(await _cityRepository.GetAllCitysAsync(), "CityId", "Name" , cityId);
            ViewBag.sort = sort;
            
            var listings = await _listingRepository.GetAllListingAsync(page , keyword , cateId ,cityId ,minPrice, maxPrice ,sort , role);

            return View(listings);
        }

		[Authorize(AuthenticationSchemes = "MyAuthenticationSchema")]
		public async Task<ActionResult> Create()
		{
			// Get UserId from Claim
			var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

			// Get User and Subscription information
			var user = await _context.Users
				.Include(u => u.UserSubscriptions)
				.ThenInclude(us => us.Subscription)
				.SingleOrDefaultAsync(u => u.UserId == userId);

			// Check the list of subscriptions
			var expiredSubscriptions = user.UserSubscriptions
				.Where(us => us.EndDate <= DateTime.Now) // Package has expired
				.OrderByDescending(us => us.EndDate)
				.ToList();

			var activeSubscriptions = user.UserSubscriptions
				.Where(us => us.EndDate > DateTime.Now) // Package is still valid
				.OrderByDescending(us => us.Subscription.MaxAds) // MaxAds highest priority
				.ThenByDescending(us => us.EndDate) // If MaxAds are equal, EndDate takes precedence
				.ToList();

			// If there is no Package is still valid and there is an expired package
			if (!activeSubscriptions.Any())
			{
				if (expiredSubscriptions.Any())
				{
					TempData["err"] = "Your subscription has expired. Please renew your subscription to create a listing.";
					return RedirectToAction("RenewSubscription", "Home"); // Navigate to the renewal page
				}

				// If there is no package
				TempData["err"] = "You need a valid subscription to create a listing.";
				return RedirectToAction("Pricing", "Home"); // Navigate to the package purchase page
			}

			// Get the active subscription with the highest MaxAds
			var activeSubscription = activeSubscriptions.FirstOrDefault();
			var subscription = activeSubscription?.Subscription;

			if (subscription == null)
			{
				TempData["err"] = "Invalid subscription.";
				return RedirectToAction("Pricing", "Home");
			}

			// Calculate the number of days remaining
			var remainingDays = (activeSubscription.EndDate - DateTime.Now).Days;

			// Check current active post count
			var activeListingsCount = await _context.Listings
				.Where(l => l.UserId == userId && l.Status == 1) // Only active posts are counted.
				.CountAsync();

			// If the article limit for the current package is reached
			if (activeListingsCount >= subscription.MaxAds)
			{
				TempData["err"] = $"You have reached the maximum number of ads ({subscription.MaxAds}) allowed by your subscription.";
				return RedirectToAction("Pricing", "Home");
			}

			// Show number of remaining posts and days remaining
			ViewBag.RemainingAds = subscription.MaxAds - activeListingsCount;
			ViewBag.RemainingDays = remainingDays; // Add remaining days
			ViewBag.User = user;

			// Load category and city for form
			ViewBag.categories = new SelectList(await _categoryRepository.GetCategoryAsync(), "CategoryId", "Name");
			ViewBag.city = new SelectList(await _cityRepository.GetAllCitysAsync(), "CityId", "Name");
			ViewBag.showContact = new SelectList(new[]
			{
		    new { Value = 0, Text = "Hide" },
		    new { Value = 1, Text = "Show" },
	    }, "Value", "Text");

			// Returns the post creation form
			return View();
		}

		[Authorize(AuthenticationSchemes = "MyAuthenticationSchema")]
		// POST: ListingController/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Listing listing, IFormFile file, IFormFile[] files)
        {
            // Get user information
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var user = await _context.Users
                .Include(u => u.UserSubscriptions)
                .ThenInclude(us => us.Subscription)
                .SingleOrDefaultAsync(u => u.UserId == userId);

            // Get a list of expired subscription packages
            var expiredSubscriptions = user.UserSubscriptions
                .Where(us => us.EndDate <= DateTime.Now)
                .OrderByDescending(us => us.EndDate)
                .ToList();

            // Check if subscription is valid
            var activeSubscription = user.UserSubscriptions
                .Where(us => us.EndDate > DateTime.Now) // Only get Package is still valid
                .OrderByDescending(us => us.Subscription.MaxAds) // Prioritize the highest MaxAds package
                .ThenByDescending(us => us.EndDate)
                .FirstOrDefault();

            // If no Package is still valid
            if (activeSubscription == null)
            {
                // If any package expires, renewal is recommended.
                if (expiredSubscriptions.Any())
                {
                    TempData["err"] = "Your subscription has expired. Please renew your subscription.";
                    return RedirectToAction("RenewSubscription"); // Navigate to the renewal page
                }

                // If there is no package (not registered)
                TempData["err"] = "You don't have an active subscription. Please purchase a subscription.";
                return RedirectToAction("PurchaseSubscription"); // Navigate to the package purchase page
            }

            // Check the number of active posts
            var activeListingsCount = await _context.Listings
                .Where(l => l.UserId == userId && l.Status == 1) // Only active posts count
                .CountAsync();

            if (activeListingsCount >= activeSubscription.Subscription.MaxAds)
            {
                TempData["err"] = $"You have reached the maximum number of ads ({activeSubscription.Subscription.MaxAds}) allowed by your subscription.";
                return RedirectToAction("Create");
            }

            // Processing the main image file
            if (file != null && file.Length > 0)
            {
                var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", uniqueFileName);
                listing.Image = "images/" + uniqueFileName;
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
            }

            // Check ModelState
            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState is invalid. Errors:");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }

                // Load category and city for form
                ViewBag.categories = new SelectList(await _categoryRepository.GetCategoryAsync(), "CategoryId", "Name");
                ViewBag.city = new SelectList(await _cityRepository.GetAllCitysAsync(), "CityId", "Name");
                return View(listing);
            }

            // Add more information to the article
            listing.UserId = userId;
            listing.CreatedAt = DateTime.Now;
			// Add article
			await _listingRepository.AddListingAsync(listing);

            //add data to Statistical
            var Statistical = await _context.Statisticals
                            .FirstOrDefaultAsync(x => x.CreatedAt.Value.Date == listing.CreatedAt.Value.Date);

            if (Statistical != null)
            {
                Statistical.ListingCount += 1;
                _context.Update(Statistical);
            }
            else
            {
                int ListingCount = 1;

                Statistical = new Statistical
                {
                    TransactionCount = 0,
                    PriceCount = 0,
                    CreatedAt = listing.CreatedAt,
                    ListingCount = ListingCount,
                    UserCount = 0
                };
                _context.Add(Statistical);
                _context.SaveChanges();
            }

            // Processing of secondary image files
            if (files != null && files.Length > 0)
            {
                foreach (var i in files)
                {
                    if (i != null && i.Length > 0)
                    {
                        var uniqueFileName = $"{Guid.NewGuid()}_{i.FileName}";
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", uniqueFileName);
                        var image = new Image
                        {
                            ImagePath = "images/" + uniqueFileName,
                            ListingId = listing.ListingId
                        };

                        await _imageRepository.AddImageAsync(image);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await i.CopyToAsync(fileStream);
                        }
                    }
                }
            }

            TempData["msg"] = "Create listing successful.";
            TempData["AlertType"] = "success"; // Types: success, error, warning, info
            return RedirectToAction("Listing","User");
        }



        public async Task<ActionResult> Edit(int id) 
        {
            // Get UserId from Claim
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // Get User and Subscription information
            var user = await _context.Users
                .Include(u => u.UserSubscriptions)
                .ThenInclude(us => us.Subscription)
                .SingleOrDefaultAsync(u => u.UserId == userId);

            // Check the list of subscriptions
            var expiredSubscriptions = user.UserSubscriptions
                .Where(us => us.EndDate <= DateTime.Now) // Package has expired
                .OrderByDescending(us => us.EndDate)
                .ToList();

            var activeSubscriptions = user.UserSubscriptions
                .Where(us => us.EndDate > DateTime.Now) // Package is still valid
                .OrderByDescending(us => us.Subscription.MaxAds) // MaxAds highest priority
                .ThenByDescending(us => us.EndDate) // If MaxAds are equal, EndDate takes precedence
                .ToList();

            // If there is no Package is still valid and there is an expired package
            if (!activeSubscriptions.Any())
            {
                if (expiredSubscriptions.Any())
                {
                    TempData["err"] = "Your subscription has expired. Please renew your subscription to create a listing.";
                    return RedirectToAction("RenewSubscription", "Home"); // Navigate to the renewal page
                }

                // If there is no package
                TempData["err"] = "You need a valid subscription to create a listing.";
                return RedirectToAction("Pricing", "Home"); // Navigate to the package purchase page
            }

            // Get the active subscription with the highest MaxAds
            var activeSubscription = activeSubscriptions.FirstOrDefault();
            var subscription = activeSubscription?.Subscription;

            if (subscription == null)
            {
                TempData["err"] = "Invalid subscription.";
                return RedirectToAction("Pricing", "Home");
            }

            // Calculate the number of days remaining
            var remainingDays = (activeSubscription.EndDate - DateTime.Now).Days;

            // Check current active post count
            var activeListingsCount = await _context.Listings
                .Where(l => l.UserId == userId && l.Status == 1) // Only active posts are counted.
                .CountAsync();

            // If the article limit for the current package is reached
            if (activeListingsCount >= subscription.MaxAds)
            {
                TempData["err"] = $"You have reached the maximum number of ads ({subscription.MaxAds}) allowed by your subscription.";
                return RedirectToAction("Pricing", "Home");
            }

            // Show number of remaining posts and days remaining
            ViewBag.RemainingAds = subscription.MaxAds - activeListingsCount;
            ViewBag.RemainingDays = remainingDays; // Add remaining days
            ViewBag.User = user;
            

            // Load category and city for form
            ViewBag.categories = new SelectList(await _categoryRepository.GetCategoryAsync(), "CategoryId", "Name");
            ViewBag.city = new SelectList(await _cityRepository.GetAllCitysAsync(), "CityId", "Name");
            ViewBag.showContact = new SelectList(new[]
            {
            new { Value = 0, Text = "Hide" },
            new { Value = 1, Text = "Show" },
        }, "Value", "Text");
            var listing = await _listingRepository.GetListingByIdAsync(id);

            // Returns the post creation form
            return View(listing);

        }

        [HttpPost]
        public async Task<ActionResult> Edit(Listing listing , IFormFile? file , IFormFile[] files)
        {
            // Get user information
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var user = await _context.Users
                .Include(u => u.UserSubscriptions)
                .ThenInclude(us => us.Subscription)
                .SingleOrDefaultAsync(u => u.UserId == userId);

            // Get a list of expired subscription packages
            var expiredSubscriptions = user.UserSubscriptions
                .Where(us => us.EndDate <= DateTime.Now)
                .OrderByDescending(us => us.EndDate)
                .ToList();

            // Check if subscription is valid
            var activeSubscription = user.UserSubscriptions
                .Where(us => us.EndDate > DateTime.Now) // Only get Package is still valid
                .OrderByDescending(us => us.Subscription.MaxAds) // Prioritize the highest MaxAds package
                .ThenByDescending(us => us.EndDate)
                .FirstOrDefault();

            // If no Package is still valid
            if (activeSubscription == null)
            {
                // If any package expires, renewal is recommended.
                if (expiredSubscriptions.Any())
                {
                    TempData["err"] = "Your subscription has expired. Please renew your subscription.";
                    return RedirectToAction("RenewSubscription"); // Navigate to the renewal page
                }

                // If there is no package (not registered)
                TempData["err"] = "You don't have an active subscription. Please purchase a subscription.";
                return RedirectToAction("PurchaseSubscription"); // Navigate to the package purchase page
            }

            // Check the number of active posts
            var activeListingsCount = await _context.Listings
                .Where(l => l.UserId == userId && l.Status == 1) // Only active posts count
                .CountAsync();

            if (activeListingsCount >= activeSubscription.Subscription.MaxAds)
            {
                TempData["err"] = $"You have reached the maximum number of ads ({activeSubscription.Subscription.MaxAds}) allowed by your subscription.";
                return RedirectToAction("Create");
            }

            // Processing the main image file
            if (file != null && file.Length > 0)
            {
                var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", uniqueFileName);
                listing.Image = "images/" + uniqueFileName;
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
            }
            else
            {
                listing.Image = listing.Image;
            }

            // Check ModelState
            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState is invalid. Errors:");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }

                // Load category and city for form
                ViewBag.User = user;
                ViewBag.categories = new SelectList(await _categoryRepository.GetCategoryAsync(), "CategoryId", "Name");
                ViewBag.city = new SelectList(await _cityRepository.GetAllCitysAsync(), "CityId", "Name");
                return View(listing);
            }
            var isDuplicateTitle = await _listingRepository.IsTitleDuplicateAsync(listing.Title , listing.ListingId);
            if (isDuplicateTitle)
            {
                TempData["msg"] = "title already exists";
                TempData["AlertType"] = "error"; // Types: success, error, warning, info
            }

            // Add more information to the article
            listing.UserId = userId;
            listing.CreatedAt = listing.CreatedAt;
            listing.UpdatedAt = DateTime.Now;
            // Add article
            await _listingRepository.UpdateListingAsync(listing);

            // Processing of secondary image files
            if (files != null && files.Length > 0)
            {
                var imageOld = await _context.Images.Where(x => x.ListingId == listing.ListingId).ToListAsync();
                foreach (var i in imageOld)
                {
                    await _imageRepository.DeleteImageAsync(i.ImageId);
                }
                foreach (var i in files)
                {
                    if (i != null && i.Length > 0)
                    {
                        var uniqueFileName = $"{Guid.NewGuid()}_{i.FileName}";
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", uniqueFileName);
                        var image = new Image
                        {
                            ImagePath = "images/" + uniqueFileName,
                            ListingId = listing.ListingId
                        };

                        await _imageRepository.AddImageAsync(image);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await i.CopyToAsync(fileStream);
                        }
                    }
                }
            }

            TempData["msg"] = "Update listing successful.";
            TempData["AlertType"] = "success"; // Types: success, error, warning, info
            return RedirectToAction("Listing", "User");

        }
        public async Task<IActionResult> Delete(int id)
        {
            var listing = await _listingRepository.GetListingByIdAsync(id);
            if (listing != null)
            {
                await _listingRepository.DeleteListingAsync(id);

                TempData["msg"] = "Delete Listing successful";
                TempData["AlertType"] = "success"; // Types: success, error, warning, info
                return RedirectToAction("Listing", "User");

            }
            TempData["msg"] = "Existing posts cannot be deleted.";
            TempData["AlertType"] = "error"; // Types: success, error, warning, info
            return View("Listing", "User");
        }


        public async Task<IActionResult> Detail(int id)
        {
            var listing = await _listingRepository.GetListingByIdAsync(id);
            var image = await _imageRepository.GetImageByListingIdAsync(id);
            ViewBag.image = image;

			Console.WriteLine(ViewBag.image);

            return View(listing);
        }
	}
}
