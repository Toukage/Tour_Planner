using TourPlanner.Model;
using log4net;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.Interfaces;


namespace DataAccessLayer
{
    public class LogRepo : ILogRepo
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(LogRepo));
        private readonly IDbContextFactory<DatabaseManager> _dbFactory;

        public LogRepo(IDbContextFactory<DatabaseManager> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        //--------------------------------SET--DATA--------------------------------
        public async Task<TourLog> InsertLogAsync(TourLog Log, CancellationToken ct = default)
        {
            try
            {
                using var db = _dbFactory.CreateDbContext();
                await db.TourLogs.AddAsync(Log, ct);
                await db.SaveChangesAsync(ct);
                log.Info($"Successfully inserted log (ID={Log.LogID}) for TourID={Log.TourID}");
                return Log;
            }
            catch (Exception ex)
            {
                log.Error("Insert failed", ex);
                throw new LogRepoException("Failed to insert log into database.", ex);
            }
        }

        public async Task EditLogAsync(TourLog Log, CancellationToken ct = default)
        {
            try
            {
                using var db = _dbFactory.CreateDbContext();
                db.TourLogs.Update(Log);
                await db.SaveChangesAsync(ct);
                log.Info($"Successfully modified log (ID={Log.LogID}) for TourID={Log.TourID}");
            }
            catch (Exception ex)
            {
                log.Error("Update failed", ex);
                throw new LogRepoException("Failed to update log from database.", ex);
            }
        }

        //--------------------------------REMOVE--DATA--------------------------------
        public async Task DropLogAsync(TourLog Log, CancellationToken ct = default)
        {

            if (Log == null)
                throw new LogRepoException("Log entity is null.");
            try
            {
                using var db = _dbFactory.CreateDbContext();
                var toDelete = await db.TourLogs.FindAsync(new object?[] { Log.LogID }, ct);

                if (toDelete == null)
                    throw new LogRepoException($"Log with ID {Log.LogID} not found.");

                db.TourLogs.Remove(toDelete);
                await db.SaveChangesAsync(ct);
                log.Info($"Successfully dropped log (ID={Log.LogID}) for TourID={Log.TourID}");
            }
            catch (Exception ex)
            {
                log.Error($"Delete failed (id={Log.LogID})", ex);
                throw new LogRepoException("Failed to drop log from Database.", ex);
            }
        }

        //--------------------------------GET--DATA--------------------------------
        public async Task<List<TourLog>> GetLogsAsync(int tourId, CancellationToken ct = default)
        {
            try
            {
                using var db = _dbFactory.CreateDbContext();
                var query = db.TourLogs
                     .AsNoTracking()
                    .Where(l => l.TourID == tourId)
                    .OrderByDescending(l => l.LogDate);
                var logs = await query.ToListAsync(ct);
                log.Info($"Successfully loaded {logs.Count} logs from the database.");
                return logs;
            }
            catch (Exception ex)
            {
                log.Error("GetLogs failed", ex);
                throw new LogRepoException("Failed to load logs from database.", ex);
            }
        }
    }
}
