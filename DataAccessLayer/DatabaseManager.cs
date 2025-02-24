using Microsoft.EntityFrameworkCore;
using TourPlanner.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.DataAccessLayer
{


    namespace TourPlanner.DAL
    {
        public class DatabaseManager : DbContext
        {
            public DbSet<Tour> Tours { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseNpgsql("Host=localhost;Port=5433;Username=toukage;Password=tourplanner;Database=TP_DB");
            }
        }
    }

}