using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IDirections
    {
        Task<(string geoJson, double meters, double seconds)> GetRouteAsync((double lon, double lat) start, (double lon, double lat) end, string profile, CancellationToken ct = default);
    }
}
