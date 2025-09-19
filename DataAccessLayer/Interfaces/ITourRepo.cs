using TourPlanner.Model;

namespace DataAccessLayer.Interfaces
{
    public interface ITourRepo
    {
        Task<Tour> InsertTourAsync(Tour tour);
        Task EditTourAsync(Tour tour);
        Task DropTourAsync(int tourId);
        Task<List<Tour>> GetAllToursAsync();
    }
}
