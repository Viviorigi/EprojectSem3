using DataAccessLayer_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer_DAL.Repositories
{
	public interface ICategoryRepository
	{
		Task<IEnumerable<Category>> GetAllCategoryAsync();
		Task<Category?> GetCategoryByIdAsync(int cateId);
		Task AddCategoryAsync(Category category);
		Task UpdateCategoryAsync(Category category);
		Task DeleteCategoryAsync(int cateId);

        Task<IEnumerable<Listing>> Search(string? keyword);
    }
}
