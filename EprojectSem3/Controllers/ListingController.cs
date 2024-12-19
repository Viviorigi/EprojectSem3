using DataAccessLayer_DAL.Models;
using DataAccessLayer_DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
			ViewBag.categories = new SelectList(await _categoryRepository.GetCategoryAsync(), "CategoryId", "Name");
			ViewBag.city = new SelectList(await _cityRepository.GetAllCitysAsync(), "CityId", "Name");
			return View();
        }

		// POST: ListingController/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Create(Listing listing, IFormFile file, IFormFile[] files)
		{
			if (file != null && file.Length > 0)
			{

				var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", file.FileName);
				listing.Image = "images/" + file.FileName;

				using (var fileStream = new FileStream(filePath, FileMode.Create))
				{
					await file.CopyToAsync(fileStream);
				}
			}
			else
			{
				TempData["err"] = "Image is not required";
				return View(listing);
			}
			if (!ModelState.IsValid)
			{
				var errors = ModelState.Values.SelectMany(v => v.Errors)
								   .Select(e => e.ErrorMessage);
				foreach (var error in errors)
				{
					Console.WriteLine(error); // Hoặc log lỗi
				}

				ViewBag.categories = new SelectList(await _categoryRepository.GetCategoryAsync(), "CategoryId", "Name");
				ViewBag.city = new SelectList(await _cityRepository.GetAllCitysAsync(), "CityId", "Name");
				return View(listing);
			}

			listing.UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
			listing.CreatedAt = DateTime.Now;

			await _listingRepository.AddListingAsync(listing);

			if (files != null && files.Length > 0)
			{
				foreach (IFormFile i in files)
				{

					var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", i.FileName);
					Image img = new Image();
					img.ImagePath = "images/" + i.FileName;
					img.ListingId = listing.ListingId;
					await _imageRepository.AddImageAsync(img);
					using (var fileStream = new FileStream(filePath, FileMode.Create))
					{
						await i.CopyToAsync(fileStream);
					}
				}
			}

			TempData["msg"] = "Create listing succsessful";

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
