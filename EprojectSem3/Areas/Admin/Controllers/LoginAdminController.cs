using DataAccessLayer_DAL.Models;
using EprojectSem3.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Realtors_Portal.Areas.Admin.Models;
using System.Security.Claims;

namespace EprojectSem3.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class LoginAdminController : Controller
	{
		private readonly AppDbContext _context;
		public LoginAdminController(AppDbContext context)
		{
			_context = context;
		}
		public IActionResult Index()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Index(LoginModel model)
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
				if (acc.Role != 2)
				{
					ModelState.AddModelError(string.Empty, "You don't have permission");
					return View(model);
				}
				if (acc.Status == 0) {
                    ModelState.AddModelError(string.Empty, "Your Account don't active");
                    return View(model);
                }

				var identity = new ClaimsIdentity(new[]
					{
						new Claim(ClaimTypes.NameIdentifier,acc.UserId.ToString()),
						new Claim(ClaimTypes.Name, acc.FullName),
						new Claim(ClaimTypes.Email, acc.Email),
						new Claim(ClaimTypes.Role, acc.Role.ToString()),
                        new Claim("ProfileImage",acc.Image ?? string.Empty)
                    }, 
					"MyAppAuthenticationAdmin");

				var parincipal = new ClaimsPrincipal(identity);
				await HttpContext.SignInAsync("MyAppAuthenticationAdmin", parincipal);
				return RedirectToAction("Index", "Home");
			}
			return View();
		}
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync("MyAppAuthenticationAdmin");
            return RedirectToAction("Index", "LoginAdmin");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [Authorize(AuthenticationSchemes = "MyAppAuthenticationAdmin")]
        public IActionResult ChangePassword()
		{
			return View();
		}

        [Authorize(AuthenticationSchemes = "MyAppAuthenticationAdmin")]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordAdmin model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (ModelState.IsValid)
			{
				var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
				if (userId == null)
				{
					return RedirectToAction("Index");
				}
				var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
				if (user == null)
				{
					return RedirectToAction("Index");
				}
                user.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

				Logout();
				return RedirectToAction("Index");
            }
            return View();
        }

    }
}
