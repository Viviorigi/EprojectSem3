using BussinessLogicLayer_BLL.Services;
using DataAccessLayer_DAL.Models;
using DataAccessLayer_DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EprojectSem3.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _service;
        private readonly AppDbContext _context;

        public CategoryController(ICategoryRepository service , AppDbContext context)
        {
            _service = service;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var categories =await _service.GetAllCategoryAsync();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                   .Select(e => e.ErrorMessage);
                foreach (var error in errors)
                {
                    Console.WriteLine(error); // Hoặc log lỗi
                }
                return View(category);
            }
            await _service.AddCategoryAsync(category);
            ViewBag.message = "Create successful";
            return  RedirectToAction("Index");


        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await _service.GetCategoryByIdAsync(id);
            if (category == null)
            {
                ViewBag.message = "category does not exist";
                return RedirectToAction("Index");
            }
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit( Category category)
        {
			if (!ModelState.IsValid)
			{
				var errors = ModelState.Values.SelectMany(v => v.Errors)
								   .Select(e => e.ErrorMessage);
				foreach (var error in errors)
				{
					Console.WriteLine(error); // Hoặc log lỗi
				}
				return View(category);
			}
             await _service.UpdateCategoryAsync(category);
			 ViewBag.message = "Update category successful";
             return RedirectToAction("Index");
            
            
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var listing = _context.Listings.FirstOrDefault(x => x.CategoryId == id);
            if (listing == null)
            {
                var category = await _service.GetCategoryByIdAsync(id);
                if (category != null)
                {
                   await _service.DeleteCategoryAsync(id);
                    ViewBag.message = "Delete Category successful";
                    return RedirectToAction("Index");
                }
            }
            ViewBag.message = "Existing posts cannot be deleted.";
            return View("index");
        }
    }
}
