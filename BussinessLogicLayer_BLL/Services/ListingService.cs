using DataAccessLayer_DAL.Models;
using DataAccessLayer_DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace BussinessLogicLayer_BLL.Services
{
	public class ListingService : IListingRepository
	{
		private readonly AppDbContext _context;
		public ListingService(AppDbContext context) {
			_context = context;
		}
		public async Task AddListingAsync(Listing listing)
		{
			_context.Listings.Add(listing);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteListingAsync(int listingId)
		{
			var listing = _context.Listings.FirstOrDefault(x => x.ListingId == listingId);
			if (listing != null) {
				var imageOld = _context.Images.Where(x => x.ListingId == listingId).ToList();
				foreach (var i in imageOld)
				{
					_context.Images.Remove(i);
				}
				_context.Listings.Remove(listing);
				await _context.SaveChangesAsync();
			}
		}

		public async Task<IEnumerable<Listing>> GetAllListingAsync()
		{
			
			return await _context.Listings.Include(x => x.Category).Include(x => x.User).Include(x => x.City).Include(x=>x.Ratings).OrderByDescending(c => c.CreatedAt).ToListAsync(); 
		}

        public async Task<IEnumerable<Listing>> GetAllListingAsync(int? page, string? keyword, int? cateId, int? cityId, double? minPrice, double? maxPrice, string? sort ,string? role)
        {

            int pageSize = 8;
            int pageNumber = page ?? 1; // If there is no page, default is page 1
            var listings =  _context.Listings.Include(x => x.Category).Include(x => x.User).Include(x => x.Ratings).Include(x => x.City).OrderByDescending(p => p.Priority).Where(x => x.Status == 1).Where(x => x.Category.Status == 1).AsQueryable();
			if (!string.IsNullOrEmpty(keyword))
			{
				listings = listings.Where(x => EF.Functions.Like(x.Title, $"%{keyword}%"));
			}
			if (cateId.HasValue)
			{
				listings = listings.Where(x => x.CategoryId == cateId);
			}
			if (cityId.HasValue) 
			{
				listings = listings.Where(x => x.CityId == cityId);
			}
			if (minPrice.HasValue)
			{
				listings = listings.Where(x => x.Price >= minPrice);
			}
			if (maxPrice.HasValue)
			{
				listings = listings.Where(x => x.Price <= maxPrice);
			}
			listings = role switch
			{
				"private-seller" => listings.Where(x => x.User.Role == 0),
				"agent" => listings.Where(x => x.User.Role == 1),
				_ => listings

			};
			listings = sort switch
			{
				"Price_des" => listings.OrderBy(x => x.Price),
				"Price_asc" => listings.OrderByDescending(x => x.Price),
				_ => listings.OrderByDescending(x => x.Priority)
			};
            return await listings.ToPagedListAsync(pageNumber , pageSize);
        }

        public async Task<Listing?> GetListingByIdAsync(int listingId)
		{
			var listing = await _context.Listings.Include(x => x.City).Include(x => x.User).Include(x => x.Category).Include(x => x.Ratings).ThenInclude(r => r.User).FirstOrDefaultAsync(x => x.ListingId ==listingId);
			if (listing != null)
			{
				return listing;
			}
			throw new NotImplementedException();
		}

		public async Task<IEnumerable<Listing>> GetListingTop5ByPriorityAsync()
		{
			return await _context.Listings.Include(x => x.User).Include(x => x.Category).Include(x => x.City).Where(x => x.Status == 1 && x.Priority == 1 && x.Category.Status == 1).Take(5).ToListAsync();
		}

		public async Task<IEnumerable<Listing>> GetMyListingAsync(int userId, int? page, string? search)
		{
			int pageSize = 5;
			int pageNumber = page ?? 1; // If there is no page, default is page 1
            var listings = _context.Listings.Include(x => x.Category).Include(x => x.User).Include(x => x.City).OrderByDescending(p => p.CreatedAt).Where(x => x.UserId == userId).Where(x => x.Category.Status == 1).AsQueryable();
			if (!string.IsNullOrEmpty(search))
			{
				listings = listings.Where(x => EF.Functions.Like(x.Title, $"%{search}%"));
			}
			
			return await listings.ToPagedListAsync(pageNumber, pageSize);

		}

        public async Task<bool> IsTitleDuplicateAsync(string title, int? listingId = null)
        {
            return await _context.Listings
            .AnyAsync(l => l.Title == title && l.ListingId != listingId);
        }

        public async Task<IEnumerable<Listing>> Search(string? keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return Enumerable.Empty<Listing>();

            keyword = keyword.ToLower();
			var result = _context.Listings.Include(x => x.Category).Include(x => x.City).Include(x => x.User).Where(x => x.Title.ToLower().Contains(keyword) || x.Category.Name.ToLower().Contains(keyword) || x.City.Name.ToLower().Contains(keyword)).ToList();
			return result;
        }

        public async Task UpdateListingAsync(Listing listing)
		{
			_context.Listings.Update(listing);
			await _context.SaveChangesAsync();
		}
	}
}
