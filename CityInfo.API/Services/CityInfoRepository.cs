using CityInfo.API.DbContexts;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services
{
    public class CityInfoRepository : ICityInfoRepository
    {
        private readonly CityInfoContext _context;

        public CityInfoRepository(CityInfoContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<City?> GetCityAsync(int CityId, bool IncludePointOfInterest)
        {
            if (IncludePointOfInterest)
            {
                return await _context.Cities.Include(c => c.PointsOfInterest).Where(c => c.Id == CityId).FirstOrDefaultAsync();
            }

            return await _context.Cities.Where(c => c.Id == CityId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<City>> GetCtiesAsync()
        {
            return await _context.Cities.OrderBy(c=>c.Name).ToListAsync();
        }

        public async Task<PointOfInterest?> GetPointOfInterestForCityAsync(int CityId, int PointOfInterestId)
        {
           return await _context.PointOfInterests.Where(c=>c.CityId == CityId && c.Id == PointOfInterestId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int CityId)
        {
            return await _context.PointOfInterests.OrderBy(c => c.Name).ToListAsync();
        }
    }
}
