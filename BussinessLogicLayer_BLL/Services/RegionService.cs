using DataAccessLayer_DAL.Models;
using DataAccessLayer_DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BussinessLogicLayer_BLL.Services
{
    public class RegionService : IRegionRepository
    {
        private readonly AppDbContext _context;

        public RegionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddRegionAsync(Region region)
        {
            _context.Regions.Add(region);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRegionAsync(int regionId)
        {
            var region = await _context.Regions.FindAsync(regionId);
            if (region != null)
            {
                _context.Regions.Remove(region);
                _context.SaveChanges();
            }
        }

        public async Task<IEnumerable<Region>> GetAllRegionsAsync()
        {
            return await _context.Regions.OrderByDescending(r=> r.RegionId).ToListAsync();
        }

        public async Task<Region?> GetRegionByIdAsync(int? regionId)
        {
            return await _context.Regions.FirstOrDefaultAsync(x => x.RegionId == regionId);
        }

        public async Task UpdateRegionAsync(Region region)
        {
            _context.Regions.Update(region);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Region>> Search(string? keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return Enumerable.Empty<Region>();
            keyword = keyword.ToLower();
            var result = _context.Regions.Where(x => x.Name.ToLower().Contains(keyword)).ToList();
            return result;
        }
    }
}
