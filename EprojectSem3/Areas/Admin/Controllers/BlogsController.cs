using BussinessLogicLayer_BLL.Services;
using DataAccessLayer_DAL.Models;
using DataAccessLayer_DAL.Repositories;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using EprojectSem3.Models;
using EprojectSem3.Areas.Admin.Controllers;
using System.Reflection;
using System.Reflection.Metadata;

namespace Realtors_Portal.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BlogsController : BaseController
    {
        private readonly AppDbContext _context;
        private readonly IBlogRepository _blogRepository;
        public BlogsController(AppDbContext context,IBlogRepository blogRepository)
        {
            _context = context;
            _blogRepository = blogRepository;
        }

        // GET: Admin/Blogs
        public async Task<IActionResult> Index()
        {
            var blogs=_context.Blogs.ToList();

            return View(blogs);
        }

        // GET: Admin/Blogs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _blogRepository.GetBlogByIdAsync(id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // GET: Admin/Blogs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Blogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Blog blog, IFormFile file, IFormFile[] files)
        {
            if (file != null && file.Length > 0)
            {

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/blogs", file.FileName);
                blog.Image = "images/blogs/" + file.FileName;

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
            }
            else
            {
                TempData["err"] = "Image is not required";

                return View(blog);
            }
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                   .Select(e => e.ErrorMessage);
                foreach (var error in errors)
                {
                    Console.WriteLine(error);
                }
                return View(blog);
            }
            blog.CreatedAt = DateTime.Now;
            await _blogRepository.AddBlogAsync(blog);
            TempData["msg"] = "Create successful";
            TempData["AlertType"] = "success";
            return RedirectToAction("Index");
        }

        // GET: Admin/Blogs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var blog = await _blogRepository.GetBlogByIdAsync(id);
            if (blog == null)
            {
                
                TempData["msg"] = "Blog does not exist";
                TempData["AlertType"] = "error"; // Các loại: success, error, warning, info
                return RedirectToAction("Index");
            }
            return View(blog);
        }

        // POST: Admin/Blogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,  Blog blog, IFormFile? file, IFormFile[] files)
        {
            if (file != null && file.Length > 0)
            {

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", file.FileName);
                blog.Image = "images/blogs/" + file.FileName;

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
                    Console.WriteLine(error);
                }
                return View(blog);
            }
            blog.BlogId = id;
            await _blogRepository.UpdateBlogAsync(blog);
            
            TempData["msg"] = "Update blog successful";
            TempData["AlertType"] = "success";
            return RedirectToAction("Index");
        }

        // GET: Admin/Blogs/Delete
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var blog = await _blogRepository.GetBlogByIdAsync(id);
            if (blog == null)
            {
                
                TempData["msg"] = "Blog does not exist";
                TempData["AlertType"] = "error"; // Các loại: success, error, warning, info
                return RedirectToAction("Index");
            }

            await _blogRepository.DeleteBlogAsync(id);
            
            TempData["AlertType"] = "success"; // Các loại: success, error, warning, info
            TempData["msg"] = "Delete blog successful";
            TempData["AlertType"] = "success";
            return RedirectToAction("Index");
        }
        // GET: Admin/Blogs/Search
        public async Task<IActionResult> SearchAdmin(string? keyword)
        {
            // Call the Search method in the repository with pagination parameters
            var blogs = _blogRepository.SearchAdmin(keyword);
            if (blogs == null)
            {
                TempData["err"] = "Coudn't find blog";
                return RedirectToAction("Index");
            }
            return View(blogs);
        }

    }
}
