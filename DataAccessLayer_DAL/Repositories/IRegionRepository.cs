using DataAccessLayer_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 

namespace DataAccessLayer_DAL.Repositories
{
    public interface IRegionRepository
    {
        Task<IEnumerable<Region>> GetAllRegionsAsync();
        Task<Region?> GetRegionByIdAsync(int? regionId);
        Task AddRegionAsync(Region region);
        Task UpdateRegionAsync(Region region);
        Task DeleteRegionAsync(int regionId);
    }
}
