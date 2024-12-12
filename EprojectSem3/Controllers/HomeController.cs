using BussinessLogicLayer_BLL.Services;
using DataAccessLayer_DAL.Models;
using DataAccessLayer_DAL.Repositories;
using EprojectSem3.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace EprojectSem3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;
        private readonly IListingRepository _listingRepository;

        public HomeController(ILogger<HomeController> logger, AppDbContext context,EmailService emailService , IListingRepository listingRepository)
        {
            _logger = logger;
            _context = context;
            _emailService = emailService;
            _listingRepository = listingRepository;
        }

        public async Task<IActionResult> Index()
        {
            var listingTop = await _listingRepository.GetListingTop5ByPriorityAsync();
            return View(listingTop);
        }

        public IActionResult Plan()
        {
            return View();
        }

        public IActionResult AboutUs()
        {
            return View();
        }

        public IActionResult Blog()
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
			ViewBag.Role = new SelectList(new[]
			{
				new { Value = 0, Text = "Private Seller" },
				new { Value = 1, Text = "Agent" }
			}, "Value", "Text");
			return View();
        }

		[HttpPost]
		public async Task<IActionResult> SignUp(RegisterModel model)
		{
			if (ModelState.IsValid)
			{
				// Check if password and confirm password match
				if (model.Password != model.ConfirmPassword)
				{
					ModelState.AddModelError("PasswordMismatch", "Passwords do not match.");
					return View(model);
				}

				// Create new User object from RegisterModel
				var user = new DataAccessLayer_DAL.Models.User
				{
					FullName = model.FullName,
					Email = model.Email,
					Password = BCrypt.Net.BCrypt.HashPassword(model.Password), 
					PhoneNumber = model.PhoneNumber,
					Address = model.Address,
                    Role = model.Role,
                    Status=0
				};

				// Save to database
				_context.Users.Add(user);
				await _context.SaveChangesAsync();
				string verificationUrl = Url.Action("VerifyAccount", "Home", new { email = model.Email }, protocol: Request.Scheme);

				// Create the email subject and body
				string subject = "Welcome to Our Platform!";
				string body = $"<h1>Hi {model.FullName},</h1>" +
							  "<p>Thank you for registering! Please check your email to verify your account and activate it.</p>" +
							  $"<p>Click <a href=\"{verificationUrl}\">here</a> to verify your email and activate your account.</p>" +
							  "<p>Your account is now pending approval.</p>";

				// Send the email
				await _emailService.SendEmailAsync(model.Email, subject, body);
				return RedirectToAction("ConfirmationAccount");	
			}

			// If there are validation errors, redisplay the form
			ViewBag.Role = new SelectList(new[]
			{
				new { Value = 0, Text = "Private Seller" },
				new { Value = 1, Text = "Agent" }
			}, "Value", "Text");
			return View(model);
		}

        public IActionResult ConfirmationAccount()
        {
            return View();
        }

		[HttpGet]
		public async Task<IActionResult> VerifyAccount(string email)
		{
			// Perform account verification (e.g., update the user's status in the database)
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
				user.Status = 1;
				await _context.SaveChangesAsync();
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

				if (acc.Status == 0)
				{
					ModelState.AddModelError(string.Empty, "Account is not active.");
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
