using DataAccessLayer_DAL.Models;
using DataAccessLayer_DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace BussinessLogicLayer_BLL.Services
{
	public class CategoryService : ICategoryRepository
	{
		private readonly AppDbContext _context;
		public CategoryService(AppDbContext context ) {
			_context = context;
		}
		public async Task AddCategoryAsync(Category category)
		{
			 _context.Categories.Add(category);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteCategoryAsync(int cateId)
		{
			var category = _context.Categories.FirstOrDefault(x => x.CategoryId == cateId);
			if (category != null)
			{
				_context.Categories.Remove(category);
				await _context.SaveChangesAsync();
			}

		}

		public async Task<IEnumerable<Category>> GetAllCategoryAsync()
		{
			return await _context.Categories.OrderByDescending(c => c.CategoryId).ToListAsync();
		}


        public async Task<IEnumerable<Category>> GetCategoryAsync()
        {
            return await _context.Categories.Where(x => x.Status == 1).ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(int cateId)
		{
			var category = _context.Categories.FirstOrDefault(x => x.CategoryId == cateId);
			if(category != null)
			{
				return category;
			}
			throw new NotImplementedException();
		}

		
		public async Task<IEnumerable<Category>> Search(string? keyword)
		{
            if (string.IsNullOrWhiteSpace(keyword))
                return Enumerable.Empty<Category>();
            keyword = keyword.ToLower();
            var result = _context.Categories.Where(x => x.Name.ToLower().Contains(keyword)).ToList();
            return result;
        }
		public async Task UpdateCategoryAsync(Category category)
		{
			if (category == null)
			{
				throw new NotImplementedException();
			}
			_context.Categories.Update(category);
			await _context.SaveChangesAsync();
			
		}
	}
}
