
using DataAccessLayer_DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EprojectSem3.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ListingController : Controller
    {
        private readonly AppDbContext _context;
        public ListingController(AppDbContext context)
        {
            _context = context;
        }
        // GET: ListingController
        public ActionResult Index()
        {
           var listings = _context.Listings.Include(x => x.Category).Include(x => x.User).Include(x => x.City).ToList();
            return View(listings);
        }

        // GET: ListingController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ListingController/Create
        public ActionResult Create()
        {
            ViewBag.categories = new SelectList(_context.Categories,"CategoryId","Name");
            ViewBag.city = new SelectList(_context.Cities, "CityId", "Name");
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

                ViewBag.categories = new SelectList(_context.Categories, "CategoryId", "Name");
                ViewBag.city = new SelectList(_context.Cities, "CityId", "Name");
                return View(listing);
            }
            listing.UserId = 1;
            listing.CreatedAt = DateTime.Now;
            _context.Listings.Add(listing);
            _context.SaveChanges();

            if (files != null && files.Length > 0)
            {
                foreach (IFormFile i in files)
                {

                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", i.FileName);
                    Image img = new Image();
                    img.ImagePath = "/images/" + i.FileName;
                    img.ListingId = listing.ListingId;
                    _context.Images.Add(img);
                    _context.SaveChanges();
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
        public ActionResult Edit(int id)
        {
            var listing = _context.Listings.FirstOrDefault(x => x.ListingId == id);
            if(listing == null)
            {
				ViewBag.messsage = "Listing does not exist";
				return RedirectToAction("Index");
			}
			ViewBag.categories = new SelectList(_context.Categories, "CategoryId", "Name");
			ViewBag.city = new SelectList(_context.Cities, "CityId", "Name");
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

                ViewBag.categories = new SelectList(_context.Categories, "CategoryId", "Name");
                ViewBag.city = new SelectList(_context.Cities, "CityId", "Name");
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
            _context.Listings.Update(listing);
            _context.SaveChanges();

            if (files != null && files.Length > 0)
            {
                var imageOld = _context.Images.Where(x => x.ListingId == listing.ListingId).ToList();
                foreach (var i in imageOld)
                {
                    _context.Images.Remove(i);
                }
                foreach (IFormFile i in files)
                {

                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", i.FileName);
                    Image img = new Image();
                    img.ImagePath = "/images/" + i.FileName;
                    img.ListingId = listing.ListingId;
                    _context.Images.Add(img);
                    _context.SaveChanges();
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
        public ActionResult Delete(int id)
        {
			var listing = _context.Listings.FirstOrDefault(x => x.ListingId == id);
			if (listing != null)
			{
				var imageOld = _context.Images.Where(x => x.ListingId == listing.ListingId).ToList();
				foreach (var i in imageOld)
				{
					_context.Images.Remove(i);
				}
				_context.Listings.Remove(listing);
				_context.SaveChanges();
				
				ViewBag.message = "Delete Category successful";
				return RedirectToAction("Index");

			}
			ViewBag.message = "Existing posts cannot be deleted.";
			return View("index");
		}

    }
}
