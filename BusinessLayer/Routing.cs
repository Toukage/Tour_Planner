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
            log.Debug($"Distance/Time requested: start={startPlace}, end={endPlace}, transport={transport}");
            try
            {
                var (key, profile) = BuildKey(startPlace, endPlace, transport);
                var result = await _cache.GetOrAdd(key, _ => ComputeCoreAsync(startPlace, endPlace, profile, ct));
                log.Info($"Distance and time computed: {result.Km:F2} km, {result.Minutes:F1} min, transport={profile}");
                return (result.Km, result.Minutes);
            }
            catch (Exception ex)
            {
                log.Error($"Failed to compute distance and time: start={startPlace}, end={endPlace}, transport={transport}", ex);
                throw new RoutingException("Failed to compute distance and time for route.", ex);
            }
        }

        public async Task<string> RouteAsync(string startPlace, string endPlace, string transport, CancellationToken ct = default)
        {
            log.Debug($"Route requested: start={startPlace}, end={endPlace}, transport={transport}");
            try
            {
                var (key, profile) = BuildKey(startPlace, endPlace, transport);
                var result = await _cache.GetOrAdd(key, _ => ComputeCoreAsync(startPlace, endPlace, profile, ct));
                log.Info($"Route computed for transport={profile}. Length: {result.Km:F2} km, Duration: {result.Minutes:F1} min");
                return result.Route;
            }
            catch (Exception ex)
            {
                log.Error($"Failed to compute route: start={startPlace}, end={endPlace}, transport={transport}", ex);
                throw new RoutingException("Failed to compute route.", ex);
            }
        }

        // das ist von einem AI Prompt!
        private async Task<RouteResult> ComputeCoreAsync(string startPlace, string endPlace, string profile, CancellationToken ct)
        {
            try
            {
                var start = await _geo.GeocodeAsync(startPlace, focus: null, ct);
                log.Debug($"Geocoded start: {startPlace} -> {start.lon},{start.lat}");
                var end = await _geo.GeocodeAsync(endPlace, focus: start, ct);
                log.Debug($"Geocoded end: {endPlace} -> {end.lon},{end.lat}");

                var (geo, meters, seconds) = await _dir.GetRouteAsync(start, end, profile, ct);
                log.Info($"Route received. Meters: {meters}, Seconds: {seconds}");

                return new RouteResult
                {
                    Route = geo,
                    Km = meters / 1000.0,
                    Minutes = seconds / 60.0
                };
            }
            catch (Exception ex)
            {
                log.Error($"Failed to compute core route result: start={startPlace}, end={endPlace}, profile={profile}", ex);
                throw new RoutingException("Failed to compute core route result.", ex);
            }
        }
    }
}
