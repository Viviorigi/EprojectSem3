using DataAccessLayer_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer_DAL.Repositories
{
	public interface IImageRepository
	{
		Task<IEnumerable<Image>> GetAllImageAsync();
		Task<Image> GetImageByListingIdAsync(int listingId);
		Task AddImageAsync(Image image);
		Task DeleteImageAsync(int imageId);
	}
}
