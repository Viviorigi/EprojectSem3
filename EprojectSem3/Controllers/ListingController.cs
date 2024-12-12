using DataAccessLayer_DAL.Models;
using Microsoft.AspNetCore.Mvc;

namespace EprojectSem3.Controllers
{
    public class ListingController : Controller
    {
        private readonly AppDbContext _context;

        public ListingController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }
    }
}
