using DataAccessLayer_DAL.Models;
using DataAccessLayer_DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogicLayer_BLL.Services
{
	public class ImageService : IImageRepository
	{
		private readonly AppDbContext _context;
		public ImageService(AppDbContext context) {
			_context = context;
		}
		public async Task AddImageAsync(Image image)
		{
			_context.Images.Add(image);
			await _context.SaveChangesAsync();
			
		}

		public async Task DeleteImageAsync(int imageId)
		{
			var image = _context.Images.FirstOrDefault(x => x.ImageId == imageId);
			if (image != null)
			{
				 _context.Images.Remove(image);
				await _context.SaveChangesAsync();
			}
		}

		public async Task<IEnumerable<Image>> GetAllImageAsync()
		{
			return await _context.Images.ToListAsync();
		}

		public async  Task<IEnumerable<Image>> GetImageByListingIdAsync(int listingId)
		{
			var image = _context.Images.Where(x => x.ListingId == listingId);
			if (image != null)
			{
				return image ;
			}
			throw new NotImplementedException();
		}
	}
}
