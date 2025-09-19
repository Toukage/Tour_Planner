using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.Model;

namespace BusinessLayer.Interfaces
{
    public interface ILogLogic
    {
        Task<List<TourLog>> GetLogsAsync(int tourId, CancellationToken ct = default);
        Task<TourLog> CreateLogAsync(int tourId, DateTime date, string? comment, int difficulty, float distance, float time, int rating, CancellationToken ct = default);
        Task DeleteLogAsync(TourLog log, CancellationToken ct = default);
        Task ModifyLogAsync(TourLog log, CancellationToken ct = default);
    }
}
