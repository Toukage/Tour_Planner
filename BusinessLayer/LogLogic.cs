using BusinessLayer.Interfaces;
using DataAccessLayer.Interfaces;
using log4net;
using TourPlanner.Model;

namespace BusinessLayer
{
    public sealed class LogLogic : ILogLogic
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(LogLogic));

        private readonly ILogRepo _repo;
        public LogLogic(ILogRepo repo) => _repo = repo;


        public async Task<TourLog> CreateLogAsync(TourLog Log,CancellationToken ct = default)
        {
            try
            {
                var savedLog = await _repo.InsertLogAsync(Log, ct);
         
                log.Info($"Created Log for Tour: {Log.TourID}");
                return Log;
            }
            catch (Exception ex)
            {
                log.Error($"Failed to Create Log for Tour: {Log.TourID}", ex);
                throw new TourLogicException("Failed to create log.", ex);
            }
        }

        public async Task DeleteLogAsync(TourLog Log, CancellationToken ct = default)
        {
            try
            {
                await _repo.DropLogAsync(Log, ct);
                log.Info($"Deleted Log for Tour: {Log.TourID}");
            }
            catch (Exception ex)
            {
                log.Error($"Failed to Delete Log for Tour: {Log.TourID}", ex);
                throw new LogLogicException("Failed to delete log.", ex);
            }
        }

        public async Task ModifyLogAsync(TourLog Log, CancellationToken ct = default)
        {
            try
            {
                await _repo.EditLogAsync(Log, ct);
                log.Info($"Modified Log for Tour: {Log.TourID}");
            }
            catch (Exception ex)
            {
                log.Error($"Failed to Modify Log for Tour: {Log.TourID}", ex);
                throw new LogLogicException("Failed to modify log.", ex);
            }
        }

        public async Task<List<TourLog>> GetLogsAsync(int tourId, CancellationToken ct = default)
        {
            try
            {
                var logs = await _repo.GetLogsAsync(tourId, ct);
                log.Info($"Got all Logs for Tour: {tourId}");
                return logs;
            }
            catch (Exception ex)
            {
                log.Error($"Got all Logs for Tour: {tourId}");
                throw new LogLogicException("Failed to get logs for tour.", ex);
            }
        }
    }
}
