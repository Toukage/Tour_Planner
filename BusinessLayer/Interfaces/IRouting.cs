using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IRouting
    {
        Task<(double km, double minutes)> DistAndTimeAsync(string startPlace, string endPlace, string transport, CancellationToken ct = default);
        Task<string> RouteAsync(string startPlace, string endPlace, string transport, CancellationToken ct = default);
    }
}
