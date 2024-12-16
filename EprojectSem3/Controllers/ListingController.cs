using DataAccessLayer_DAL.Models;
using DataAccessLayer_DAL.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EprojectSem3.Controllers
{
    public class ListingController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IListingRepository _listingRepository;
        private readonly IImageRepository _imageRepository;

        public ListingController(AppDbContext context , IListingRepository listingRepository , IImageRepository imageRepository)
        {
            _imageRepository = imageRepository;
            _listingRepository = listingRepository;
            _context = context;
        }
        public async Task<IActionResult> Index(int? page)
        {
            var listings = await _listingRepository.GetAllListingAsync(page);

            return View(listings);
        }

        public IActionResult Create()
        {
            return View();
        }
        public async Task<IActionResult> Detail(int id)
        {
            var listing = await _listingRepository.GetListingByIdAsync(id);
            var image = await _imageRepository.GetImageByListingIdAsync(id);
            ViewBag.image = image;

			Console.WriteLine(ViewBag.image);

            return View(listing);
        }
    }
}
