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
    public class CityService : ICityRepository
    {
        private readonly AppDbContext _context;

        public CityService(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddCityAsync(City city)
        {
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCityAsync(int cityId)
        {
            var city = await _context.Cities.FindAsync(cityId);
            if (city != null)
            {
                _context.Cities.Remove(city);
                _context.SaveChanges();
            }
        }

        public async Task<IEnumerable<City>> GetAllCitysAsync()
        {
            return await _context.Cities.Include(c => c.Region).ToListAsync();
        }

        public async Task<City?> GetCityByIdAsync(int? cityId)
        {
            return await _context.Cities.Include(c => c.Region).FirstOrDefaultAsync(x => x.CityId == cityId);
        }

        public async Task UpdateCityAsync(City city)
        {
            _context.Cities.Update(city);
            await _context.SaveChangesAsync();
        }
    }
}
