using Microsoft.EntityFrameworkCore;
using TourPlanner.Model;

namespace DataAccessLayer
{
    public class DatabaseManager : DbContext
    {
        public DbSet<Tour> Tours { get; set; }
        public DbSet<TourLog> TourLogs { get; set; }
        public DatabaseManager(DbContextOptions<DatabaseManager> options) : base(options) { }
    }
}