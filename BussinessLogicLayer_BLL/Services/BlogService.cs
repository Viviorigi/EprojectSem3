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
    public class BlogService : IBlogRepository
    {
        private readonly AppDbContext _context;
        public BlogService(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddBlogAsync(Blog Blog)
        {
            _context.Blogs.Add(Blog);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBlogAsync(int BlogId)
        {
            var blog = _context.Blogs.FirstOrDefault(x => x.BlogId == BlogId);
            if (blog != null)
            {
                _context.Blogs.Remove(blog);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<(IEnumerable<Blog> Blogs, int TotalCount)> GetBlogsPaginatedAsync(int pageNumber, int pageSize)
        {
            var query = _context.Blogs.Where(b => b.Status == 1).AsQueryable();
            var totalCount = await query.CountAsync();
            var blogs = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (blogs, totalCount);
        }


        public async Task<Blog?> GetBlogByIdAsync(int? BlogId)
        {
            var blog = _context.Blogs.FirstOrDefault(x => x.BlogId == BlogId);
            if (blog != null)
            {
                return blog;
            }
            throw new NotImplementedException();
        }

        public async Task UpdateBlogAsync(Blog Blog)
        {
            if (Blog == null)
            {
                throw new NotImplementedException();
            }
            _context.Blogs.Update(Blog);
            await _context.SaveChangesAsync();
        }

        public async Task<(IEnumerable<Blog> Blogs, int TotalCount)> Search(string? keyword, int pageNumber, int pageSize)
        {
            var query = _context.Blogs.AsQueryable();

            // If a keyword is provided, apply the search filter
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(x => x.Title.Contains(keyword) || x.Content.Contains(keyword));
            }

            var totalCount = await query.CountAsync();

            var blogs = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (blogs, totalCount);
        }

        public async Task<IEnumerable<Blog>> SearchAdmin(string? keyword)
        {
            var blogs = _context.Blogs.OrderByDescending(b => b.CreatedAt).ToList();
            return(blogs);
        }
    }
}
