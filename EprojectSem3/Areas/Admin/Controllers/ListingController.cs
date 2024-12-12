
using DataAccessLayer_DAL.Models;
using DataAccessLayer_DAL.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EprojectSem3.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ListingController : BaseController
    {
        private readonly AppDbContext _context;
        private readonly IListingRepository _listingRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICityRepository _cityRepository;
        private readonly IImageRepository _imageRepository;
        public ListingController(AppDbContext context , IListingRepository service , ICategoryRepository categoryRepository , ICityRepository cityRepository , IImageRepository imageRepository)
        {
            _context = context;
            _imageRepository = imageRepository;
            _listingRepository = service;
            _categoryRepository = categoryRepository;
            _cityRepository = cityRepository;   
        }
        // GET: ListingController
        public async Task<IActionResult> Index()
        {
           var listings = await _listingRepository.GetAllListingAsync();
            return View(listings);
        }

        // GET: ListingController/Details/5
        public ActionResult Details(int id)
        {
            var listing = _listingRepository.GetListingByIdAsync(id);
            return View(listing);

        }

        // GET: ListingController/Create
        public async Task<ActionResult> Create()
        {
            ViewBag.categories = new SelectList( await _categoryRepository.GetAllCategoryAsync(),"CategoryId","Name");
            ViewBag.city = new SelectList(await _cityRepository.GetAllCitysAsync(), "CityId", "Name");
            return View();
        }

        // POST: ListingController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Listing listing, IFormFile[] files)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                   .Select(e => e.ErrorMessage);
                foreach (var error in errors)
                {
                    Console.WriteLine(error); // Hoặc log lỗi
                }

                ViewBag.categories = new SelectList(await _categoryRepository.GetAllCategoryAsync(), "CategoryId", "Name");
                ViewBag.city = new SelectList(await _cityRepository.GetAllCitysAsync(), "CityId", "Name");
                return View(listing);
            }
            listing.UserId = 1;
            listing.CreatedAt = DateTime.Now;
            await _listingRepository.AddListingAsync(listing);

            if (files != null && files.Length > 0)
            {
                foreach (IFormFile i in files)
                {

                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", i.FileName);
                    Image img = new Image();
                    img.ImagePath = "/images/" + i.FileName;
                    img.ListingId = listing.ListingId;
                    await _imageRepository.AddImageAsync(img);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await i.CopyToAsync(fileStream);
                    }
                }
            }

            ViewBag.message = "Create listing succsessful";

            return RedirectToAction("Index");
        }

        // GET: ListingController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var listing = await _listingRepository.GetListingByIdAsync(id);
            if(listing == null)
            {
				ViewBag.messsage = "Listing does not exist";
				return RedirectToAction("Index");
			}
			ViewBag.categories = new SelectList(await _categoryRepository.GetAllCategoryAsync(), "CategoryId", "Name");
			ViewBag.city = new SelectList(await _cityRepository.GetAllCitysAsync(), "CityId", "Name");
			return View(listing);

		}

        // POST: ListingController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Listing listing, IFormFile[] files)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                   .Select(e => e.ErrorMessage);
                foreach (var error in errors)
                {
                    Console.WriteLine(error); // Hoặc log lỗi
                }

                ViewBag.categories = new SelectList(await _categoryRepository.GetAllCategoryAsync(), "CategoryId", "Name");
                ViewBag.city = new SelectList(await _cityRepository.GetAllCitysAsync(), "CityId", "Name");
                return View(listing);
            }
            //var data = _context.Listings.Where(x => x.ListingId != listing.ListingId && x.Title == listing.Title);
            //if (data != null)
            //{
            //    ViewBag.message = "Title has existed";
            //    return View(listing);
            //}

            listing.UserId = 1;
            //var listingOld = _context.Listings.FirstOrDefault(x=> x.ListingId == listing.ListingId);
            //listing.CreatedAt = listingOld.CreatedAt;
            listing.UpdatedAt = DateTime.Now;
            await _listingRepository.UpdateListingAsync(listing);

            if (files != null && files.Length > 0)
            {
                var imageOld = _context.Images.Where(x => x.ListingId == listing.ListingId).ToList();
                foreach (var i in imageOld)
                {
                   await _imageRepository.AddImageAsync(i);
                }
                foreach (IFormFile i in files)
                {

                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", i.FileName);
                    Image img = new Image();
                    img.ImagePath = "/images/" + i.FileName;
                    img.ListingId = listing.ListingId;
                    await _imageRepository.AddImageAsync(img);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await i.CopyToAsync(fileStream);
                    }
                }
                

            }
			ViewBag.message = "Create listing succsessful";

			return RedirectToAction("Index");
		}

        // GET: ListingController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
			var listing = await _listingRepository.GetListingByIdAsync(id);
			if (listing != null)
			{
				await _listingRepository.DeleteListingAsync(id);
				
				ViewBag.message = "Delete Category successful";
				return RedirectToAction("Index");

			}
			ViewBag.message = "Existing posts cannot be deleted.";
			return View("index");
		}

        public async Task<IActionResult> Search(string? keyword)
        {
          var listing = await _listingRepository.Search(keyword);
          if(listing == null)
            {
                ViewBag.messsage = "Search results do not exist";
                return RedirectToAction("Index");
            }
          return View(listing);
        }

    }
}
