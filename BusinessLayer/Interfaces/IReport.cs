using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.Model;

namespace BusinessLayer.Interfaces
{
    public interface IReport
    {
        Task ReportAsync(
            Tour tour,
            List<TourLog> logs,
            byte[]? mapPng,
            string path,
            CancellationToken ct = default);
    }
}
