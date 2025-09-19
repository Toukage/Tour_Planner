using TourPlanner.Model;

namespace DataAccessLayer.Interfaces
{
    public interface ILogRepo
    {
        Task<TourLog> InsertLogAsync(TourLog logEntity, CancellationToken ct = default);
        Task EditLogAsync(TourLog logEntity, CancellationToken ct = default);
        Task DropLogAsync(TourLog logEntity, CancellationToken ct = default);
        Task<List<TourLog>> GetLogsAsync(int tourId, CancellationToken ct = default);
    }
}
