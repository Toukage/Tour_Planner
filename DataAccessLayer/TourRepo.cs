using TourPlanner.Model;
using log4net;

namespace DataAccessLayer
{
    public class TourRepo
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(TourRepo));

        //--------------------------------SET--DATA--------------------------------
        public void InsertTour(Tour tour)
        {
            try
            {
                log.Info($"Attempting to insert tour: {tour.TourName}, {tour.TourStart}, {tour.TourEnd}");

                using (var db = new DatabaseManager())
                {
                    db.Tours.Add(tour);
                    db.SaveChanges();
                }

                log.Info($"Successfully inserted tour: {tour.TourName}");
            }
            catch (Exception ex)
            {
                log.Error("Error inserting tour into database", ex);
                throw;
            }
        }

        //--------------------------------REMOVE--DATA--------------------------------
        public void DropTour(Tour tour)
        {
            try
            {
                log.Info($"Attempting to drop tour: {tour.TourName}");

                using (var db = new DatabaseManager())
                {
                    var tourToDelete = db.Tours.FirstOrDefault(t => t.TourName == tour.TourName);

                    if (tourToDelete != null)
                    {
                        db.Tours.Remove(tourToDelete);
                        db.SaveChanges();
                    }
                }
                log.Info($"Successfully removed tour: {tour.TourName}");
            }
            catch (Exception ex)
            {
                log.Error("Error removing tour from database", ex);
                throw;
            }
        }

    }


    //--------------------------------GET--DATA--------------------------------

}
