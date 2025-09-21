using BusinessLayer.Interfaces;
using log4net;
using System.Globalization;
using System.Text.Json;

namespace BusinessLayer
{
    public sealed class GeoCode : IGeoCode
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(GeoCode));
        private readonly HttpClient _http;
        private readonly string _apiKey;
        
        public GeoCode(HttpClient http, string apiKey)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        }
        public async Task<(double lon, double lat)> GeocodeAsync(string place, (double lon, double lat)? focus = null, CancellationToken ct = default)
        {
            log.Debug($"[Geocode] Request: place='{place}' focus={focus?.lon},{focus?.lat}");
            try
            {
                var url = $"https://api.openrouteservice.org/geocode/search?text={Uri.EscapeDataString(place)}&size=1";
                if (focus.HasValue)
                    url += $"&focus.point.lon={focus.Value.lon.ToString(CultureInfo.InvariantCulture)}" +
                            $"&focus.point.lat={focus.Value.lat.ToString(CultureInfo.InvariantCulture)}";

                using var req = new HttpRequestMessage(HttpMethod.Get, url);//creates HTTP request
                req.Headers.TryAddWithoutValidation("Authorization", _apiKey);//adds api key to the authorization header

                log.Debug($"Place= {place} | Url= {url}");
                using var res = await _http.SendAsync(req, ct);//sends request and waits
                var body = await res.Content.ReadAsStringAsync(ct);//reads response body (ROUTE) and creates a string based on it 

                log.Debug($" status={(int)res.StatusCode}");
                res.EnsureSuccessStatusCode();//throws exception when needed

                using var doc = JsonDocument.Parse(body);//parsed body aus dem response
                var features = doc.RootElement.GetProperty("features");
                if (features.GetArrayLength() == 0)
                {
                    log.Warn($"No geocode result for {place}");
                    throw new InvalidOperationException($"No geocode result for {place}");
                }
                   

                var coords = features[0].GetProperty("geometry").GetProperty("coordinates");//findet die coordinaten im body
                var lon = coords[0].GetDouble();
                var lat = coords[1].GetDouble();
                log.Info($"Success: '{place}' -> lon={lon}, lat={lat}");
                return (lon, lat);//gibt die coordinaten zrk
            }
            catch (Exception ex)
            {
                log.Error($"Geocoding failed for {place}", ex);
                throw new GeocodeException("Geocoding failed.", ex);
            }
        }
    }
}
