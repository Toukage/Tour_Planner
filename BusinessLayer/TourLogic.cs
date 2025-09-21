using TourPlanner.Model;
using log4net;
using BusinessLayer.Interfaces;
using DataAccessLayer.Interfaces;

namespace BusinessLayer
{
    public class TourLogic : ITourLogic
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(TourLogic));
        private readonly ITourRepo _tourRepo;
        private readonly IRouting _routing;
        private readonly IReport _report;
        private readonly ILogLogic _logLogic;

        public TourLogic(ITourRepo tourRepo, IRouting routing, IReport report, ILogLogic logLogic)
        {
            _tourRepo = tourRepo;
            _routing = routing;
            _report = report;
            _logLogic = logLogic;
        }

        public async Task CreateTourAsync(Tour tour, CancellationToken ct = default)
        {
            try
            {
                var (km, min) = await _routing.DistAndTimeAsync(tour.TourStart, tour.TourEnd, tour.Transport, ct);
                tour.Distance = (float)km;//populates tour
                tour.EstTime = (float)min;
                await _tourRepo.InsertTourAsync(tour); // saves tour to DB
                log.Info($"Created Tour: {tour.TourName}");
            }
            catch (Exception ex)
            {
                log.Error($"Failed to Create Tour: {tour.TourName}", ex);
                throw new TourLogicException("Failed to create tour.", ex);
            }
        }

        public Task<string> GetRouteAsync(Tour tour, CancellationToken ct = default)
        {
            try
            {
                var route = _routing.RouteAsync(tour.TourStart, tour.TourEnd, tour.Transport, ct);
                log.Info($"Got Route for Tour: {tour.TourName}");
                return route;
            }
            catch (Exception ex)
            {
                log.Error($"Failed to get Route for Tour: {tour.TourName}", ex);
                throw new TourLogicException("Failed to get route for tour.", ex);
            }
        }

        public async Task<List<Tour>> GetAllToursAsync()
        {
            try
            {
                var tours = await _tourRepo.GetAllToursAsync();
                log.Info("Got all Tours");
                return tours;
            }
            catch (Exception ex)
            {
                log.Error("Failed to get all Tours", ex);
                throw new TourLogicException("Failed to get all tours.", ex);
            }
        }

        public async Task DeleteTourAsync(Tour tour)
        {
            if (tour == null) return;
            try
            {
                await _tourRepo.DropTourAsync(tour.TourID);
                log.Info($"Deleted Tour: {tour.TourName}");
            }
            catch (Exception ex)
            {
                log.Error($"Failed to delete tour: {tour.TourName}", ex);
                throw new TourLogicException("Failed to delete tour.", ex);
            }
        }

        public async Task ModifyTourAsync(Tour tour, CancellationToken ct = default)
        {
            if (tour == null) return;
            try
            {
                var (km, min) = await _routing.DistAndTimeAsync(tour.TourStart, tour.TourEnd, tour.Transport, ct);
                tour.Distance = (float)km;
                tour.EstTime = (float)min;
                await _tourRepo.EditTourAsync(tour);
                log.Info($"Modified Tour: {tour.TourName}");
            }
            catch (Exception ex)
            {
                log.Error($"Failed to modify Tour : {tour.TourName}", ex);
                throw new TourLogicException("Failed to modify tour.", ex);
            }
        }

        public async Task<string> CreateReportAsync(Tour tour, byte[]? mapPng, string? reportPath = null, CancellationToken ct = default)
        {
            string safeName = string.Join("_", (tour.TourName ?? "Tour").Split(Path.GetInvalidFileNameChars())).Trim('_');
            string defaultPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                $"Tour_{safeName}_{DateTime.Now:yyyyMMdd_HHmm}.pdf"
            );

            string path = string.IsNullOrWhiteSpace(reportPath) ? defaultPath : reportPath;
            try
            {
                var logs = await _logLogic.GetLogsAsync(tour.TourID, ct);
                await _report.ReportAsync(tour, logs, mapPng, path, ct);
                log.Info($"Successfully generated report for {tour.TourName} at {path}");
                return path;
            }
            catch (Exception ex)
            {
                log.Error($"Failed to generate report for {tour.TourName}  at {path}", ex);
                throw new TourLogicException("Failed to generate report for tour.", ex);
            }
        }
    }
}
