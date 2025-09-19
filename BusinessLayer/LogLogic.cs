using BusinessLayer.Interfaces;
using DataAccessLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.Model;

namespace BusinessLayer
{
    public sealed class LogLogic : ILogLogic
    {
        private readonly ILogRepo _repo;
        public LogLogic(ILogRepo repo) => _repo = repo;

        public async Task<List<TourLog>> GetLogsAsync(int tourId, CancellationToken ct = default)
        {
            try
            {
                return await _repo.GetLogsAsync(tourId, ct);
            }
            catch (Exception ex)
            {
                throw new LogLogicException("Failed to get logs for tour.", ex);
            }
        }

        public async Task<TourLog> CreateLogAsync(
            int tourId, DateTime date, string? comment, int difficulty, float distance, float time, int rating,
            CancellationToken ct = default)
        {
            try
            {
                return await _repo.InsertLogAsync(new TourLog
                {
                    TourID = tourId,
                    LogDate = date,
                    LogComment = comment,
                    LogDifficulty = difficulty,
                    LogDistance = distance,
                    LogTime = time,
                    Rating = rating
                }, ct);
            }
            catch (Exception ex)
            {
                throw new TourLogicException("Failed to create log.", ex);
            }
        }

        public async Task DeleteLogAsync(TourLog log, CancellationToken ct = default)
        {
            try
            {
                await _repo.DropLogAsync(log, ct);
            }
            catch (Exception ex)
            {
                throw new LogLogicException("Failed to delete log.", ex);
            }
        }

        public async Task ModifyLogAsync(TourLog log, CancellationToken ct = default)
        {
            try
            {
                await _repo.EditLogAsync(log, ct);
            }
            catch (Exception ex)
            {
                throw new LogLogicException("Failed to modify log.", ex);
            }
        }
    }
}
