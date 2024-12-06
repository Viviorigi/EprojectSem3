using EprojectSem3.Models;
using Microsoft.AspNetCore.Mvc;

namespace EprojectSem3.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var categories = _context.Categories.ToList();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (category != null)
            {
                _context.Categories.Add(category);
                _context.SaveChanges();
                ViewBag.message = "Create successful";
                return RedirectToAction("Index");
            }

            return View(category);
        }

        public IActionResult Edit(int id)
        {
            var category = _context.Categories.SingleOrDefault(x => x.CategoryId == id);
            if (category == null)
            {
                ViewBag.message = "category does not exist";
                return RedirectToAction("Index");
            }
            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(int id, Category category)
        {
            var c = _context.Categories.SingleOrDefault(x => x.CategoryId == id);
            if (c != null)
            {
                c.Name = category.Name;
                c.Description = category.Description;
                _context.SaveChanges();
                ViewBag.message = "Update category successful";
                return RedirectToAction("Index");
            }
            return View(category);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var listing = _context.Listings.FirstOrDefault(x => x.CategoryId == id);
            if (listing == null)
            {
                var category = _context.Categories.FirstOrDefault(x => x.CategoryId == id);
                if (category != null)
                {
                    _context.Categories.Remove(category);
                    _context.SaveChanges();
                    ViewBag.message = "Delete Category successful";
                    return RedirectToAction("Index");
                }
            }
            ViewBag.message = "Existing posts cannot be deleted.";
            return View("index");
        }
    }
}
