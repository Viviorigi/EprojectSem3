using DataAccessLayer_DAL.Models;
using EprojectSem3.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace EprojectSem3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Listings()
        {
            return View();
        }

        public IActionResult Plan()
        {
            return View();
        }

        public IActionResult AboutUs()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult FAQ()
        {
            return View();
        }
        public IActionResult SignUp()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var acc = await _context.Users.FirstOrDefaultAsync(a => a.Email == model.Email);

                if (acc == null)
                {
                    ModelState.AddModelError(string.Empty, "Your account does not exist");
                    return View(model); 
                }

                bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(model.Password, acc.Password);

                if (!isPasswordCorrect)
                {
                    ModelState.AddModelError(string.Empty, "Invalid password");
                    return View(model);
                }

                var identity = new ClaimsIdentity(
                     new List<Claim>
                     {
                        new Claim(ClaimTypes.NameIdentifier, acc.UserId.ToString() ?? string.Empty),
                        new Claim(ClaimTypes.Name, acc.FullName ?? string.Empty),
                        new Claim(ClaimTypes.Role,acc.Role.ToString())
                     },
                     "MyAuthenticationSchema" 
                 );

                var claimsPrincipal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("MyAuthenticationSchema", claimsPrincipal);              

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync("MyAuthenticationSchema");
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult TestConnection()
        {
            try
            {
                // Run a simple query to check the connection
                var regionsCount = _context.Regions.Count();
                return Content($"Database is connected! Regions count: {regionsCount}");
            }
            catch (Exception ex)
            {
                return Content($"Database connection failed: {ex.Message}");
            }
        }
    }
}
