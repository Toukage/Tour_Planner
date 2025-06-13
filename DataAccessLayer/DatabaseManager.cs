using Microsoft.EntityFrameworkCore;
using TourPlanner.Model;

namespace DataAccessLayer
{
    public class DatabaseManager : DbContext
    {
        public DbSet<Tour> Tours { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=localhost;Port=5433;Username=toukage;Password=tourplanner;Database=TP_DB");
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tour>().ToTable("tour");
        }
    }
}