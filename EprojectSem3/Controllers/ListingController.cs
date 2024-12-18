using DataAccessLayer_DAL.Models;
using DataAccessLayer_DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EprojectSem3.Controllers
{
    public class ListingController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IListingRepository _listingRepository;
        private readonly IImageRepository _imageRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICityRepository _cityRepository;

        public ListingController(AppDbContext context , IListingRepository listingRepository , IImageRepository imageRepository , ICategoryRepository categoryRepository , ICityRepository cityRepository)
        {
            _cityRepository = cityRepository;
            _categoryRepository = categoryRepository;
            _imageRepository = imageRepository;
            _listingRepository = listingRepository;
            _context = context;
        }
        public async Task<IActionResult> Index(int? page, string? keyword, int? cateId, int? cityId, double? minPrice, double? maxPrice, int? sort)
        {
            ViewBag.categories = new SelectList(await _categoryRepository.GetAllCategoryAsync(), "CategoryId", "Name", cateId);
            ViewBag.city = new SelectList(await _cityRepository.GetAllCitysAsync(), "CityId", "Name" , cityId);
            
            var listings = await _listingRepository.GetAllListingAsync(page , keyword , cateId ,cityId ,minPrice, maxPrice ,sort);

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
