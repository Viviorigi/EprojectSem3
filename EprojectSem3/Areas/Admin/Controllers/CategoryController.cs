﻿using BussinessLogicLayer_BLL.Services;
using DataAccessLayer_DAL.Models;
using DataAccessLayer_DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EprojectSem3.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : BaseController
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
                    Console.WriteLine(error); //  error log
                }
                return View(category);
            }
            await _service.AddCategoryAsync(category);
            TempData["msg"] = "Create successful";
            TempData["AlertType"] = "success"; // Types: success, error, warning, info
            return  RedirectToAction("Index");


        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await _service.GetCategoryByIdAsync(id);
            if (category == null)
            {
				TempData["msg"] = "category does not exist";
                TempData["AlertType"] = "error";
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
					Console.WriteLine(error); // error log
                }
				return View(category);
			}
             await _service.UpdateCategoryAsync(category);
			 TempData["msg"] = "Update category successful";
            TempData["AlertType"] = "success";
            return RedirectToAction("Index");
            
            
        }

        public async Task<IActionResult> Search(string? name)
        {
			var category = await _service.Search(name);
			if (category == null)
			{
				TempData["msg"] = "Search results do not exist";
                TempData["AlertType"] = "error";
                return RedirectToAction("Index");
			}
            ViewBag.Name = name;
			return View(category);
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
                    TempData["msg"] = "Delete Category successful";
                    TempData["AlertType"] = "success";
                    return RedirectToAction("Index");
                }
            }
            TempData["msg"] = "Existing posts cannot be deleted.";
            TempData["AlertType"] = "error";
            return RedirectToAction("index");
        }
    }
}
