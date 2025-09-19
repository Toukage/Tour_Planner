using TourPlanner.Model;
using log4net;
using Microsoft.EntityFrameworkCore;


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
        public async Task<TourLog> InsertLogAsync(TourLog logEntity, CancellationToken ct = default)
        {
            try
            {
                using var db = _dbFactory.CreateDbContext();
                await db.TourLogs.AddAsync(logEntity, ct);
                await db.SaveChangesAsync(ct);
                return logEntity;
            }
            catch (Exception ex)
            {
                log.Error("[TourLogRepo] Insert failed", ex);
                throw new LogRepoException("Failed to insert log into database.", ex);
            }
        }

        public async Task EditLogAsync(TourLog logEntity, CancellationToken ct = default)
        {
            try
            {
                using var db = _dbFactory.CreateDbContext();
                db.TourLogs.Update(logEntity);
                await db.SaveChangesAsync(ct);
            }
            catch (Exception ex)
            {
                log.Error("[TourLogRepo] Update failed", ex);
                throw new LogRepoException("Failed to update log from database.", ex);
            }
        }

        //--------------------------------REMOVE--DATA--------------------------------
        public async Task DropLogAsync(TourLog logEntity, CancellationToken ct = default)
        {

            if (logEntity == null)
                throw new LogRepoException("Log entity is null.");
            try
            {
                using var db = _dbFactory.CreateDbContext();
                var toDelete = await db.TourLogs.FindAsync(new object?[] { logEntity.LogID }, ct);

                if (toDelete == null)
                    throw new LogRepoException($"Log with ID {logEntity.LogID} not found.");

                db.TourLogs.Remove(toDelete);
                await db.SaveChangesAsync(ct);
                
            }
            catch (Exception ex)
            {
                log.Error($"[TourLogRepo] Delete failed (id={logEntity.LogID})", ex);
                throw new LogRepoException("Failed to drop log from Database.", ex);
            }
        }

        //--------------------------------GET--DATA--------------------------------
        public async Task<List<TourLog>> GetLogsAsync(int tourId, CancellationToken ct = default)
        {
            using var db = _dbFactory.CreateDbContext();
            var query = db.TourLogs
                 .AsNoTracking()
                .Where(l => l.TourID == tourId)
                .OrderByDescending(l => l.LogDate);
            var logs = await query.ToListAsync(ct);

            return logs;
               
        }
    }
}
