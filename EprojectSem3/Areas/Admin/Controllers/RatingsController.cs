using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer_DAL.Models;
using EprojectSem3.Areas.Admin.Controllers;

namespace Realtors_Portal.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RatingsController : BaseController
    {
        private readonly AppDbContext _context;

        public RatingsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Ratings
        public async Task<IActionResult> Index(string keyword="")
        {
            var appDbContext = _context.Ratings.Include(r => r.Listing).Include(r => r.User).Where(r=> r.User.FullName.Contains(keyword) 
            || r.Listing.Title.Contains(keyword) || r.Listing.Description.Contains(keyword) || r.User.Email.Contains(keyword));
            ViewBag.keyword = keyword;
            return View(await appDbContext.ToListAsync());
        }

        // GET: Admin/Ratings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rating = await _context.Ratings
                .Include(r => r.Listing)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.RatingId == id);
            if (rating == null)
            {
                return NotFound();
            }

            return View(rating);
        }

       
       
        // GET: Admin/Ratings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rating = await _context.Ratings
                .Include(r => r.Listing)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.RatingId == id);
            if (rating == null)
            {
                return NotFound();
            }

            return View(rating);
        }

        // POST: Admin/Ratings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rating = await _context.Ratings.FindAsync(id);
            if (rating != null)
            {
                _context.Ratings.Remove(rating);
            }

            await _context.SaveChangesAsync();
            TempData["msg"] = "Delete Rating successful.";
            TempData["AlertType"] = "success";
            return RedirectToAction(nameof(Index));
        }

        private bool RatingExists(int id)
        {
            return _context.Ratings.Any(e => e.RatingId == id);
        }
    }
}
