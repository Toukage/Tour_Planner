using TourPlanner.Model;
using DataAccessLayer;

namespace BusinessLayer
{
    public class TourLogic
    {
        readonly TourRepo _tour;

        public TourLogic()
        {
            _tour = new TourRepo();
        }
        public void CreateNewTour(Tour tour)
        {
            _tour.InsertTour(tour);
        }

        public void DeleteTour(Tour tour) 
        {
            _tour.DropTour(tour);
        }

        public void getTourData()
        {

        }

        public void getAllTours()
        {

        }
    }
}
