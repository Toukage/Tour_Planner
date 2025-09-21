using TourPlanner.Model;

namespace BusinessLayer.Interfaces
{
    public interface ILogLogic
    {
        Task<List<TourLog>> GetLogsAsync(int tourId, CancellationToken ct = default);
        Task<TourLog> CreateLogAsync(TourLog Log, CancellationToken ct = default);
        Task DeleteLogAsync(TourLog Log, CancellationToken ct = default);
        Task ModifyLogAsync(TourLog Log, CancellationToken ct = default);
    }
}
