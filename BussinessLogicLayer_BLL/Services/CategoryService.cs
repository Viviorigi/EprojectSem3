using DataAccessLayer_DAL.Models;
using DataAccessLayer_DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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
			return await _context.Categories.ToListAsync();
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
