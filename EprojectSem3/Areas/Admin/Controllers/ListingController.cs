using DataAccessLayer_DAL.Models;
using DataAccessLayer_DAL.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        public ListingController(AppDbContext context, IListingRepository service, ICategoryRepository categoryRepository, ICityRepository cityRepository, IImageRepository imageRepository)
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
            Console.WriteLine(Request.Cookies["NameIdentifier"]);
            var listings = await _listingRepository.GetAllListingAsync();
            return View(listings);
        }

        // GET: ListingController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var listing = await _listingRepository.GetListingByIdAsync(id);
            var image = await _imageRepository.GetImageByListingIdAsync(id);
            ViewBag.image = image;

            Console.WriteLine(ViewBag.image);

            return View(listing);
        }

        // GET: ListingController/Create
        public async Task<ActionResult> Create()
        {
            ViewBag.categories = new SelectList(await _categoryRepository.GetAllCategoryAsync(), "CategoryId", "Name");
            ViewBag.city = new SelectList(await _cityRepository.GetAllCitysAsync(), "CityId", "Name");
            ViewBag.showContact = new SelectList(new[]
              {
                new { Value = 0, Text = "Hide" },
                new { Value = 1, Text = "Show" },
            }, "Value", "Text");
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
                TempData["msg"] = "Image is not required";
                TempData["AlertType"] = "error"; // Types: success, error, warning, info
                ViewBag.categories = new SelectList(await _categoryRepository.GetAllCategoryAsync(), "CategoryId", "Name");
                ViewBag.city = new SelectList(await _cityRepository.GetAllCitysAsync(), "CityId", "Name");
                ViewBag.showContact = new SelectList(new[]
                 {
                    new { Value = 0, Text = "Hide" },
                    new { Value = 1, Text = "Show" },
                }, "Value", "Text");
                return View(listing);
            }
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                   .Select(e => e.ErrorMessage);
                foreach (var error in errors)
                {
                    Console.WriteLine(error); //  error log
                }

                ViewBag.categories = new SelectList(await _categoryRepository.GetAllCategoryAsync(), "CategoryId", "Name");
                ViewBag.city = new SelectList(await _cityRepository.GetAllCitysAsync(), "CityId", "Name");
                ViewBag.showContact = new SelectList(new[]
                 {
                    new { Value = 0, Text = "Hide" },
                    new { Value = 1, Text = "Show" },
                }, "Value", "Text");
                return View(listing);
            }

            listing.UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            listing.CreatedAt = DateTime.Now;
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
            TempData["AlertType"] = "success"; // Types: success, error, warning, info

            return RedirectToAction("Index");
        }

        // GET: ListingController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var listing = await _listingRepository.GetListingByIdAsync(id);
            ViewBag.Images = await _imageRepository.GetImageByListingIdAsync(listing.ListingId);
            if (listing == null)
            {
                TempData["msg"] = "Listing does not exist";
                TempData["AlertType"] = "error"; // Types: success, error, warning, info
                return RedirectToAction("Index");
            }
            ViewBag.categories = new SelectList(await _categoryRepository.GetAllCategoryAsync(), "CategoryId", "Name");
            ViewBag.city = new SelectList(await _cityRepository.GetAllCitysAsync(), "CityId", "Name");
            ViewBag.showContact = new SelectList(new[]
             {
                new { Value = 0, Text = "Hide" },
                new { Value = 1, Text = "Show" },
            }, "Value", "Text");
            return View(listing);

        }

        // POST: ListingController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Listing listing, IFormFile? file, IFormFile[] files)
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
            
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                   .Select(e => e.ErrorMessage);
                foreach (var error in errors)
                {
                    Console.WriteLine(error); // error log
                }

                ViewBag.categories = new SelectList(await _categoryRepository.GetAllCategoryAsync(), "CategoryId", "Name");
                ViewBag.city = new SelectList(await _cityRepository.GetAllCitysAsync(), "CityId", "Name");
                ViewBag.showContact = new SelectList(new[]
             {
                new { Value = 0, Text = "Hide" },
                new { Value = 1, Text = "Show" },
            }, "Value", "Text");
                return View(listing);
            }
            var isDuplicateTitle = await _listingRepository.IsTitleDuplicateAsync(listing.Title, listing.ListingId);
            if (isDuplicateTitle)
            {
                ViewBag.categories = new SelectList(await _categoryRepository.GetAllCategoryAsync(), "CategoryId", "Name");
                ViewBag.city = new SelectList(await _cityRepository.GetAllCitysAsync(), "CityId", "Name");
                ViewBag.showContact = new SelectList(new[]
             {
                new { Value = 0, Text = "Hide" },
                new { Value = 1, Text = "Show" },
            }, "Value", "Text");
                TempData["msg"] = "title already exists";
                TempData["AlertType"] = "error"; // Types: success, error, warning, info
                return View(listing);
            }

            listing.UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            listing.UpdatedAt = DateTime.Now;
            await _listingRepository.UpdateListingAsync(listing);

            if (files != null && files.Length > 0)
            {
                var imageOld = await _context.Images.Where(x => x.ListingId == listing.ListingId).ToListAsync();
                foreach (var i in imageOld)
                {
                    await _imageRepository.DeleteImageAsync(i.ImageId);
                }
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
            TempData["AlertType"] = "success"; // Types: success, error, warning, info

            return RedirectToAction("Index");
        }

        // GET: ListingController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var listing = await _listingRepository.GetListingByIdAsync(id);
            if (listing != null)
            {
                await _listingRepository.DeleteListingAsync(id);

                TempData["msg"] = "Delete Listing successful";
                TempData["AlertType"] = "success"; // Types: success, error, warning, info
                return RedirectToAction("Index");

            }
            TempData["msg"] = "Existing posts cannot be deleted.";
            TempData["AlertType"] = "error"; // Types: success, error, warning, info
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Search(string? keyword)
        {
            var listing = await _listingRepository.Search(keyword);
            if (listing == null)
            {
                TempData["msg"] = "Search results do not exist";
                TempData["AlertType"] = "error"; // Types: success, error, warning, info
                return RedirectToAction("Index");
            }
            return View(listing);
        }

    }
}
