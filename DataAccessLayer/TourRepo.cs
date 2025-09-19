using TourPlanner.Model;
using log4net;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.Interfaces;

namespace DataAccessLayer
{
    public class TourRepo : ITourRepo
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(TourRepo));
        private readonly IDbContextFactory<DatabaseManager> _dbFactory;

        public TourRepo(IDbContextFactory<DatabaseManager> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        //--------------------------------SET--DATA--------------------------------
        public async Task<Tour> InsertTourAsync(Tour tour)
        {
            try
            {
                log.Info($"Attempting to insert tour: {tour.TourName}, {tour.TourStart}, {tour.TourEnd}");
                using var db = _dbFactory.CreateDbContext();
                await db.Tours.AddAsync(tour);
                await db.SaveChangesAsync();

                log.Info($"Successfully inserted tour: {tour.TourName}");
                return tour;
            }
            catch (Exception ex)
            {
                log.Error("Error inserting tour into database", ex);
                throw new TourRepoException("Failed to insert Tour into database.", ex);
            }
        }
        
        public async Task EditTourAsync(Tour tour)
        {
            try
            {
                log.Info($"Attempting to modify tour: {tour.TourName}, {tour.TourStart}, {tour.TourEnd}");

                using var db = _dbFactory.CreateDbContext();
                db.Tours.Update(tour);
                await db.SaveChangesAsync();
                

                log.Info($"Successfully modified tour: {tour.TourName}");
            }
            catch (Exception ex)
            {
                log.Error("Error modifing tour", ex);
                throw new TourRepoException("Failed to Update Tour from database.", ex);
            }
        }

        //--------------------------------REMOVE--DATA--------------------------------
        public async Task DropTourAsync(int tourId)
        {
            try
            {
                log.Info($"Attempting to drop tour: {tourId}");

                using var db = _dbFactory.CreateDbContext();
                var tourToDelete = await db.Tours.FindAsync(tourId);


                if (tourToDelete == null)
                    throw new TourRepoException($"Tour with ID {tourId} not found.");
            
                 db.Tours.Remove(tourToDelete);
                    await db.SaveChangesAsync();
                log.Info($"Successfully removed tour: {tourId}");
            }
            catch (Exception ex)
            {
                log.Error("Error removing tour from database", ex);
                throw new TourRepoException("Failed to drop Tour from database.", ex);
            }
        }

        //--------------------------------GET--DATA--------------------------------
        public async Task<List<Tour>> GetAllToursAsync()
        {
            using var db = _dbFactory.CreateDbContext();
            var query = db.Tours.OrderBy(t => t.TourID);
            var tours = await query.ToListAsync();

            return tours;
        }
    }
}
