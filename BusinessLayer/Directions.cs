using System.Text;
using System.Text.Json;
using log4net;

namespace BusinessLayer
{

    public sealed class Directions
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Directions));
        private readonly HttpClient _http;
        private readonly string _apiKey;

        public Directions(HttpClient http, string apiKey)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        }

        public async Task<(string geoJson, double meters, double seconds)> GetRouteAsync((double lon, double lat) start, (double lon, double lat) end, string profile, CancellationToken ct = default)
        {
            try
            {
                var url = $"https://api.openrouteservice.org/v2/directions/{profile}/geojson";
                var payload = new
                {
                    coordinates = new[]
                    {
                        new[] {start.lon, start.lat},
                        new[] {end.lon, end.lat}
                    }
                };
                var json = JsonSerializer.Serialize(payload); //changes payload into json string

                using var request = new HttpRequestMessage(HttpMethod.Post, url); //creates HTTP request
                request.Headers.TryAddWithoutValidation("Authorization", _apiKey); //adds api key to the authorization header
                request.Content = new StringContent(json, Encoding.UTF8, "application/json"); //adds the serialized body

                using var response = await _http.SendAsync(request, ct); //sends request and waits
                var body = await response.Content.ReadAsStringAsync(ct); //reads response body (ROUTE) and creates a string based on it 
                response.EnsureSuccessStatusCode();//throws exception when needed

                var (m, s) = TryReadSummary(body);//parsed distance & duration aus dem response
                return (body, m, s);
            }
            catch (Exception ex) 
            {
                throw new DirectionsException("Failed to get route from OpenRouteService.", ex);
            }
        }

        private static (double meters, double seconds) TryReadSummary(string json)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);//writes the response body into a .js file so we can access the route with the document.
                var props = doc.RootElement.GetProperty("features")[0];

                if (props.TryGetProperty("summary", out var sum))
                {
                    return (sum.GetProperty("distance").GetDouble(),
                            sum.GetProperty("duration").GetDouble());
                }

                var s0 = props.GetProperty("segments")[0];
                return (s0.GetProperty("distance").GetDouble(),
                        s0.GetProperty("duration").GetDouble());
            }
            catch
            {
                return (0, 0);
            }
        }
    }
}

