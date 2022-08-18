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
            return await _context.Cities.OrderBy(c => c.Name).ToListAsync();
        }
        public async Task<(IEnumerable<City>, PaginationMetaData)> GetCtiesAsync(string? name, string? searchQuery, int pageNumber, int pageSize)
        {
            // Both Empty (Since we have Paging implemented there is no need to check if both are empty)
            //if (string.IsNullOrEmpty(name) && string.IsNullOrWhiteSpace(searchQuery))
            //{
            //    return await GetCtiesAsync();
            //}
            // Collection is used to filter&Search data at the database level and not after that
            //This means that you do not need to get all data then filter&search instead you
            ////build Query with filter and send it to the database
            var collection = _context.Cities as IQueryable<City>;

            if (!string.IsNullOrWhiteSpace(name))
            {
                name = name.Trim();
                collection = collection.Where(c => c.Name == name);
            }

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                collection = collection.Where(a=>a.Name.Contains(searchQuery) ||
                (a.Description != null && a.Description.Contains(searchQuery)));
            }
            var totalItemCount = await collection.CountAsync(); // Gets total amount of items in the database
            var pagingtonMetaData = new PaginationMetaData(totalItemCount, pageSize, pageNumber);
            var collectionToReturn = await collection.OrderBy(a => a.Name).Skip(pageSize*(pageNumber-1)).Take(pageSize).ToListAsync();
            return (collectionToReturn, pagingtonMetaData);
        }
        public async Task<PointOfInterest?> GetPointOfInterestForCityAsync(int CityId, int PointOfInterestId)
        {
            return await _context.PointOfInterests.Where(c => c.CityId == CityId && c.Id == PointOfInterestId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int CityId)
        {
            return await _context.PointOfInterests.Where(c => c.CityId == CityId).ToListAsync();
        }

        public async Task<bool> CityExistsAsync(int CityId)
        {
            return await _context.Cities.AnyAsync(c => c.Id == CityId);
        }

        public async Task AddPointOfInterestForCityAsync(int CityId, PointOfInterest pointOfInterest)
        {
            var city = await GetCityAsync(CityId, false);
            if (city != null)
            {
                city.PointsOfInterest.Add(pointOfInterest); // This add does not added it to the database
            }
        }
        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }
        public void DeletePointOfInterest(PointOfInterest pointOfInterest)
        {
            _context.PointOfInterests.Remove(pointOfInterest);
        }
    }
}
