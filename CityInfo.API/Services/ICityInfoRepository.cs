using CityInfo.API.Entities;
using System.Threading.Tasks;

namespace CityInfo.API.Services
{
    public interface ICityInfoRepository
    {
        Task<IEnumerable<City>> GetCtiesAsync();
        Task<IEnumerable<City>> GetCtiesAsync(string? name);
        Task<City?> GetCityAsync(int CityId, bool IncludePointOfInterest); // City? Means the result may be null
        Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int CityId);
        Task<PointOfInterest?> GetPointOfInterestForCityAsync(int CityId, int PointOfInterestId);
        Task<bool> CityExistsAsync(int CityId);
        Task AddPointOfInterestForCityAsync(int CityId, PointOfInterest pointOfInterest);
        Task<bool> SaveChangesAsync();
        void DeletePointOfInterest(PointOfInterest pointOfInterest);
    }
}
