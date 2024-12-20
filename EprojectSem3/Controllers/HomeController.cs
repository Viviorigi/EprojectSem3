using BussinessLogicLayer_BLL.Services;
using DataAccessLayer_DAL.Models;
using DataAccessLayer_DAL.Repositories;
using EprojectSem3.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Realtors_Portal.Models;
using Realtors_Portal.Services;
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
        private readonly PaypalClient _paypalClient;

        public HomeController(ILogger<HomeController> logger, AppDbContext context, EmailService emailService, IListingRepository listingRepository,
            PaypalClient paypalClient )
        {
            _logger = logger;
            _context = context;
            _emailService = emailService;
            _listingRepository = listingRepository;
            _paypalClient = paypalClient;
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

        [HttpPost]
		public async Task<IActionResult> Contact(Contact model)
		{
            if (ModelState.IsValid)
            {
                 await _context.Contacts.AddAsync(model);
                 await _context.SaveChangesAsync();
                return RedirectToAction("ContactSuccess");
            }
			return View(model);
		}
		public IActionResult ContactSuccess()
		{
			return View();
		}

		public IActionResult FAQ()
        {
            return View();
        }

        [Authorize(AuthenticationSchemes = "MyAuthenticationSchema")]
        public IActionResult Pricing()
        {
            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            if (userRole == "0")
            {
                ViewBag.Memberships = _context.Subscriptions.Where(s => s.IsAgent == false);
            }
            else if (userRole == "1")
            {
                ViewBag.Memberships = _context.Subscriptions.Where(s => s.IsAgent == true);
            }
            else
            {
                ViewBag.Memberships = _context.Subscriptions.ToList();
            }
            return View();
        }

        [Authorize(AuthenticationSchemes = "MyAuthenticationSchema")]
        public IActionResult RenewSubscription()
        {
            // Lấy UserId từ Claims
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

            // Lấy danh sách các Subscription mà người dùng đã mua
            var purchasedSubscriptions = _context.UserSubscriptions
                .Where(us => us.UserId == userId)
                .Select(us => us.SubscriptionId)
                .ToList();

            // Lấy danh sách các Subscription dựa trên Role và lọc các gói chưa mua
            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            if (userRole == "0") // User thông thường
            {
                ViewBag.Memberships = _context.Subscriptions
                    .Where(s => s.IsAgent == false)
                    .ToList();
            }
            else if (userRole == "1") // Agent
            {
                ViewBag.Memberships = _context.Subscriptions
                    .Where(s => s.IsAgent == true )
                    .ToList();
            }
            else // Admin hoặc không có Role cụ thể
            {
                ViewBag.Memberships = _context.Subscriptions.ToList();
            }

            // Gửi danh sách các gói đã mua (để hiển thị hoặc xử lý nếu cần)
            ViewBag.PurchasedMemberships = _context.Subscriptions
                .Where(s => purchasedSubscriptions.Contains(s.SubscriptionId))
                .ToList();

            return View();
        }



        public async Task<IActionResult> Checkout(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var subscription = await _context.Subscriptions.FirstOrDefaultAsync(x => x.SubscriptionId == id);
            if (subscription == null)
            {
                return NotFound();
            }
            ViewBag.PaypalClientId = _paypalClient.ClientId;
            return View(subscription);
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
                    Image = "/images/users.png",
                    Status = 0
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
                        new Claim(ClaimTypes.Role,acc.Role.ToString()),
                        new Claim("ProfileImage",acc.Image ?? string.Empty)
                     },
                     "MyAuthenticationSchema"
                 ); ;

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

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(ForgotPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "The email address does not exist.");
                return View(model);
            }

            var resetToken = Guid.NewGuid().ToString();
            var expirationTime = DateTime.UtcNow.AddMinutes(5);

            user.ResetPasswordToken = resetToken;
            user.ResetTokenExpiration = expirationTime;
            _context.Users.Update(user);
            _context.SaveChanges();

            var resetLink = Url.Action("ResetPassword", "Home",
                new { token = resetToken, email = model.Email }, Request.Scheme);

            var emailContent = $"Click the following link to reset your password: <a href='{resetLink}'>Reset Password</a>";
            _emailService.SendEmailAsync(model.Email, "Password Reset", emailContent);

            ViewBag.Message = "A password reset link has been sent to your email address. Please check your Email";
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Error", "Home");
            }

            var model = new ResetPasswordModel
            {
                Token = token,
                Email = email
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email && u.ResetPasswordToken == model.Token);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid token or email.");
                return View(model);
            }

            if (user.ResetTokenExpiration < DateTime.UtcNow)
            {
                ModelState.AddModelError("", "The reset token has expired.");
                return View(model);
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            user.ResetPasswordToken = null;
            user.ResetTokenExpiration = null;

            _context.Users.Update(user);
            _context.SaveChanges();

            ViewBag.Message = "Your password has been reset successfully.";
            return RedirectToAction("Login", "Home");
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

        public async Task LoginByGoogle()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action("GoogleResponse")
                });
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (!result.Succeeded)
            {
                return RedirectToAction("Login", "Home");
            }
            var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claims => new
            {
                claims.Issuer,
                claims.OriginalIssuer,
                claims.Type,
                claims.Value
            });
            //return Json(claims);
            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;
            var check_email = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (check_email == null)
            {
                // Create new User object from RegisterModel
                var user = new DataAccessLayer_DAL.Models.User
                {
                    FullName = name,
                    Email = email,
                    Password = BCrypt.Net.BCrypt.HashPassword("123456"),
                    PhoneNumber = "123456",
                    Address = "123456",
                    Role = 0,
                    Status = 1
                };
                // Save to database
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var acc = await _context.Users.FirstOrDefaultAsync(a => a.Email == email);
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
            else
            {
                var acc = await _context.Users.FirstOrDefaultAsync(a => a.Email == email);
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
        }
    }
}
