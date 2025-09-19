using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.Model;

namespace BusinessLayer.Interfaces
{
    public interface ITourLogic
    {
        Task CreateTourAsync(Tour tour, CancellationToken ct = default);
        Task<List<Tour>> GetAllToursAsync();
        Task DeleteTourAsync(Tour tour);
        Task ModifyTourAsync(Tour tour, CancellationToken ct = default);
        Task<string> GetRouteAsync(Tour tour, CancellationToken ct = default);
        Task<string> CreateReportAsync(Tour tour, byte[]? mapPng, string? reportPath = null, CancellationToken ct = default);
    }
}
