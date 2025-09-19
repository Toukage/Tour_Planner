using System.Collections.Concurrent;
using BusinessLayer.Interfaces;
using log4net;

namespace BusinessLayer
{
    public sealed class Routing : IRouting
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Routing));

        private readonly IGeoCode _geo;
        private readonly IDirections _dir;

        private readonly ConcurrentDictionary<string, Task<RouteResult>> _cache = new();
        private sealed class RouteResult //zwischen speicher
        {
            public string Route = "";
            public double Km;
            public double Minutes;
        }
        public Routing(IGeoCode geo, IDirections dir)
        {
            _geo = geo;
            _dir = dir;
        }
        private static (string key, string profile) BuildKey(string start, string end, string? transport)//methode dieeinfach schaut das die eingabe richitg formatiert ist
        {
            string option = (transport ?? "").Trim().ToLowerInvariant();
            string profile = option switch
            {
                "car" => "driving-car",
                "bike" => "cycling-regular",
                "walking" => "foot-walking",
                "public transport" => "foot-walking",
                _ => "foot-walking"
            };

            var key = $"{start}|{end}|{profile}".Trim().ToLowerInvariant();
            return (key, profile);
        }
        
        //cancelletion token ins protocoll schreiben! dafuer da das man HTTP calls noch abbrechen kann !
        public async Task<(double km, double minutes)> DistAndTimeAsync(string startPlace, string endPlace, string transport, CancellationToken ct = default)
        {
            try
            {
                var (key, profile) = BuildKey(startPlace, endPlace, transport);
                var result = await _cache.GetOrAdd(key, _ => ComputeCoreAsync(startPlace, endPlace, profile, ct));
                return (result.Km, result.Minutes);
            }
            catch (Exception ex)
            {
                throw new RoutingException("Failed to compute distance and time for route.", ex);
            }
        }

        public async Task<string> RouteAsync(string startPlace, string endPlace, string transport, CancellationToken ct = default)
        {
            try
            {
                var (key, profile) = BuildKey(startPlace, endPlace, transport);
                var result = await _cache.GetOrAdd(key, _ => ComputeCoreAsync(startPlace, endPlace, profile, ct));
                return result.Route;
            }
            catch (Exception ex)
            {
                throw new RoutingException("Failed to compute route.", ex);
            }
        }

        // das ist von einem AI Prompt!
        private async Task<RouteResult> ComputeCoreAsync(string startPlace, string endPlace, string profile, CancellationToken ct)
        {
            try
            {
                var start = await _geo.GeocodeAsync(startPlace, focus: null, ct);
                var end = await _geo.GeocodeAsync(endPlace, focus: start, ct);

                var (geo, meters, seconds) = await _dir.GetRouteAsync(start, end, profile, ct);

                return new RouteResult
                {
                    Route = geo,
                    Km = meters / 1000.0,
                    Minutes = seconds / 60.0
                };
            }
            catch (Exception ex)
            {
                throw new RoutingException("Failed to compute core route result.", ex);
            }
        }
    }
}
