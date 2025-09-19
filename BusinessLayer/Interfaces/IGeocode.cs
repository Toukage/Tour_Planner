using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IGeoCode
    {
        Task<(double lon, double lat)> GeocodeAsync(string place, (double lon, double lat)? focus = null, CancellationToken ct = default);
    }
}
