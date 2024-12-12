using Microsoft.AspNetCore.Mvc;

namespace EprojectSem3.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detail()
        {
            return View();
        }

        public IActionResult Listing()
        {
            return View();
        }
    }
}
