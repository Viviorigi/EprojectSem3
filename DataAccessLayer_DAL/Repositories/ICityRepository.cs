using DataAccessLayer_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer_DAL.Repositories
{
    public interface ICityRepository
    {
        Task<IEnumerable<City>> GetAllCitysAsync();
        Task<City?> GetCityByIdAsync(int? cityId);
        Task AddCityAsync(City city);
        Task UpdateCityAsync(City city);
        Task DeleteCityAsync(int cityId);
        Task<IEnumerable<City>> Search(string? keyword, int? regionId);
    }
}
