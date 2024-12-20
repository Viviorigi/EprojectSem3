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
        public async Task<IActionResult> Index(int? page, string? keyword, int? cateId, int? cityId, double? minPrice, double? maxPrice, int? sort)
        {
            ViewBag.categories = new SelectList(await _categoryRepository.GetCategoryAsync(), "CategoryId", "Name", cateId);
            ViewBag.city = new SelectList(await _cityRepository.GetAllCitysAsync(), "CityId", "Name" , cityId);
            
            var listings = await _listingRepository.GetAllListingAsync(page , keyword , cateId ,cityId ,minPrice, maxPrice ,sort);

            return View(listings);
        }

        public async Task<ActionResult> Create()
        {
            // Lấy UserId từ Claim
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // Lấy thông tin User và Subscription
            var user = await _context.Users
                .Include(u => u.UserSubscriptions)
                .ThenInclude(us => us.Subscription)
                .SingleOrDefaultAsync(u => u.UserId == userId);

            //check date 
            //var dateexp = new DateTime(2025, 2, 17);

            // Kiểm tra danh sách các subscription
            var expiredSubscriptions = user.UserSubscriptions
                .Where(us => us.EndDate <= DateTime.Now) // Gói đã hết hạn
                .OrderByDescending(us => us.EndDate)
                .ToList();

            var activeSubscriptions = user.UserSubscriptions
                .Where(us => us.EndDate > DateTime.Now) // Gói còn hiệu lực
                .OrderByDescending(us => us.Subscription.MaxAds) // Ưu tiên MaxAds cao nhất
                .ThenByDescending(us => us.EndDate) // Nếu MaxAds bằng nhau, ưu tiên EndDate
                .ToList();

            // Nếu không có gói còn hiệu lực và có gói hết hạn
            if (!activeSubscriptions.Any())
            {
                if (expiredSubscriptions.Any())
                {
                    TempData["err"] = "Your subscription has expired. Please renew your subscription to create a listing.";
                    return RedirectToAction("RenewSubscription", "Home"); // Điều hướng đến trang gia hạn
                }

                // Nếu không có gói nào
                TempData["err"] = "You need a valid subscription to create a listing.";
                return RedirectToAction("Pricing", "Home"); // Điều hướng đến trang mua gói
            }

            // Lấy subscription còn hiệu lực có MaxAds cao nhất
            var activeSubscription = activeSubscriptions.FirstOrDefault();
            var subscription = activeSubscription?.Subscription;

            if (subscription == null)
            {
                TempData["err"] = "Invalid subscription.";
                return RedirectToAction("Pricing", "Home");
            }

            // Kiểm tra số lượng bài viết active hiện tại
            var activeListingsCount = await _context.Listings
                .Where(l => l.UserId == userId && l.Status == 1) // Chỉ tính bài viết đang active
                .CountAsync();

            // Nếu đạt giới hạn số lượng bài viết cho gói hiện tại
            if (activeListingsCount >= subscription.MaxAds)
            {
                TempData["err"] = $"You have reached the maximum number of ads ({subscription.MaxAds}) allowed by your subscription.";
                return RedirectToAction("Pricing", "Home");
            }

            // Hiển thị số lượng bài viết còn lại
            ViewBag.RemainingAds = subscription.MaxAds - activeListingsCount;

            // Load danh mục và thành phố cho form
            ViewBag.categories = new SelectList(await _categoryRepository.GetAllCategoryAsync(), "CategoryId", "Name");
            ViewBag.city = new SelectList(await _cityRepository.GetAllCitysAsync(), "CityId", "Name");

            // Trả về form tạo bài viết
            return View();
        }


        // POST: ListingController/Create
        [HttpPost]
		[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Listing listing, IFormFile file, IFormFile[] files)
        {
            // Lấy thông tin người dùng
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var user = await _context.Users
                .Include(u => u.UserSubscriptions)
                .ThenInclude(us => us.Subscription)
                .SingleOrDefaultAsync(u => u.UserId == userId);

            // Lấy danh sách gói subscription hết hạn
            var expiredSubscriptions = user.UserSubscriptions
                .Where(us => us.EndDate <= DateTime.Now)
                .OrderByDescending(us => us.EndDate)
                .ToList();

            // Kiểm tra subscription còn hiệu lực
            var activeSubscription = user.UserSubscriptions
                .Where(us => us.EndDate > DateTime.Now) // Chỉ lấy gói còn hiệu lực
                .OrderByDescending(us => us.Subscription.MaxAds) // Ưu tiên gói MaxAds cao nhất
                .ThenByDescending(us => us.EndDate)
                .FirstOrDefault();

            // Nếu không có gói còn hiệu lực
            if (activeSubscription == null)
            {
                // Nếu có gói hết hạn, đề xuất gia hạn
                if (expiredSubscriptions.Any())
                {
                    TempData["err"] = "Your subscription has expired. Please renew your subscription.";
                    return RedirectToAction("RenewSubscription"); // Điều hướng đến trang gia hạn
                }

                // Nếu không có gói nào (chưa đăng ký)
                TempData["err"] = "You don't have an active subscription. Please purchase a subscription.";
                return RedirectToAction("PurchaseSubscription"); // Điều hướng đến trang mua gói
            }

            // Kiểm tra số lượng bài viết active
            var activeListingsCount = await _context.Listings
                .Where(l => l.UserId == userId && l.Status == 1) // Chỉ tính bài active
                .CountAsync();

            if (activeListingsCount >= activeSubscription.Subscription.MaxAds)
            {
                TempData["err"] = $"You have reached the maximum number of ads ({activeSubscription.Subscription.MaxAds}) allowed by your subscription.";
                return RedirectToAction("Create");
            }

            // Xử lý file hình ảnh chính
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

            // Kiểm tra ModelState
            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState is invalid. Errors:");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }

                // Load danh mục và thành phố cho form
                ViewBag.categories = new SelectList(await _categoryRepository.GetAllCategoryAsync(), "CategoryId", "Name");
                ViewBag.city = new SelectList(await _cityRepository.GetAllCitysAsync(), "CityId", "Name");
                return View(listing);
            }

            // Gán thêm thông tin cho bài viết
            listing.UserId = userId;
            listing.CreatedAt = DateTime.Now;

            // Thêm bài viết
            await _listingRepository.AddListingAsync(listing);

            // Xử lý file hình ảnh phụ
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
            return RedirectToAction("Index");
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
