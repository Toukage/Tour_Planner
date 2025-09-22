using BusinessLayer;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;

namespace Tests
{
    [TestFixture]
    public class GeocodeTests
    {
        private const string ApiKey = "FAKE_API_KEY";

        private HttpClient MockHttpClient(HttpResponseMessage response)
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                     .Protected()
                     .Setup<Task<HttpResponseMessage>>(
                         "SendAsync",
                         ItExpr.IsAny<HttpRequestMessage>(),
                         ItExpr.IsAny<CancellationToken>())
                     .ReturnsAsync(response);

            return new HttpClient(handlerMock.Object);
        }

        [Test]
        public async Task GeocodeTest()//tests ob die coordinaten richtig geparsed werden
        {
            //Arrange
            string validJson = @"{
                ""features"": [
                    { ""geometry"": { ""coordinates"": [16.3738, 48.2082] } }
                ]
            }";
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(validJson)
            };
            var httpClient = MockHttpClient(response);
            var geo = new GeoCode(httpClient, ApiKey);

            //Act
            var (lon, lat) = await geo.GeocodeAsync("Vienna", null, CancellationToken.None);

            //Assert
            Assert.That(lon, Is.EqualTo(16.3738));
            Assert.That(lat, Is.EqualTo(48.2082));
        }

        [Test]
        public void GeocodeBadRequestTest()//tests ob exception geworfen wird wenn http fehlercode zurueckkommt
        {
            //Arrange
            var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("Bad Request")
            };
            var httpClient = MockHttpClient(response);
            var geo = new GeoCode(httpClient, ApiKey);

            //Act & Assert
            Assert.ThrowsAsync<GeocodeException>(async () =>
                await geo.GeocodeAsync("InvalidPlace", null, CancellationToken.None)
            );
        }

        [Test]
        public void GeocodeWrongJSONTest()//tests ob exception geworfen wird wenn features leer is
        {
            //Arrange
            string emptyJson = @"{ ""features"": [] }";
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(emptyJson)
            };
            var httpClient = MockHttpClient(response);
            var geo = new GeoCode(httpClient, ApiKey);

            //Act & Assert
            Assert.ThrowsAsync<GeocodeException>(async () =>
                await geo.GeocodeAsync("Unknown", null, CancellationToken.None)
            );
        }

        [Test]
        public async Task GeocodeMalformedTest()//tests ob exception geworfen wird wenn json nicht geparst werden kann
        {
            //Arrange
            string invalidJson = @"{ ""notfeatures"": [] }";
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(invalidJson)
            };
            var httpClient = MockHttpClient(response);
            var geo = new GeoCode(httpClient, ApiKey);

            //Act & Assert
            Assert.ThrowsAsync<GeocodeException>(async () =>
                await geo.GeocodeAsync("Broken", null, CancellationToken.None)
            );
        }
    }
}
