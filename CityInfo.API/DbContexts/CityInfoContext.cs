using Microsoft.EntityFrameworkCore;
using CityInfo.API.Entities;

namespace CityInfo.API.DbContexts
{
    public class CityInfoContext : DbContext
    {
        public DbSet<City> Cities { get; set; } = null!;
        public DbSet<PointOfInterest> PointOfInterests { get; set; }= null!;
        public CityInfoContext(DbContextOptions<CityInfoContext> options):base(options)
        {

        }
    }
}
