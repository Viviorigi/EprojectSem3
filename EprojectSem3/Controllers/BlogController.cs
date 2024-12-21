using DataAccessLayer_DAL.Models;
using DataAccessLayer_DAL.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Realtors_Portal.Controllers
{
    public class BlogController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IBlogRepository _blogRepository;
        public BlogController(AppDbContext context, IBlogRepository blogRepository) {
            _context = context;
            _blogRepository = blogRepository;
        }
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 9)
        {
            var (blogs, totalCount) = await _blogRepository.GetBlogsPaginatedAsync(pageNumber, pageSize);

            ViewBag.CurrentPage = pageNumber; 
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize); 

            return View(blogs);
        }

        public async Task<IActionResult> Details(int id)
        {
            var blog = await _blogRepository.GetBlogByIdAsync(id);
            return View(blog);
        }

    }
}
