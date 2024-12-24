using DataAccessLayer_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer_DAL.Repositories
{
    public interface IBlogRepository
    {
        Task<(IEnumerable<Blog> Blogs, int TotalCount)> GetBlogsPaginatedAsync(int pageNumber, int pageSize);
        Task<Blog?> GetBlogByIdAsync(int? BlogId);
        Task AddBlogAsync(Blog Blog);
        Task UpdateBlogAsync(Blog Blog);
        Task DeleteBlogAsync(int BlogId);
        Task<IEnumerable<Blog>> SearchAdmin(string? keyword);
        Task<(IEnumerable<Blog> Blogs, int TotalCount)> Search(string? keyword, int pageNumber, int pageSize);
    }
}
