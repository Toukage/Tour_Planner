using BusinessLayer;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;

namespace Tests
{

    [TestFixture]
    public class DirectionsTests
    {
        private const string FakeApiKey = "FAKE_API_KEY";

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
        public void GetRouteBadRequestTest()//tests a failed request
        {
            //Arrange
            var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("Bad Request")
            };
            var httpClient = MockHttpClient(response);
            var directions = new Directions(httpClient, FakeApiKey);

            //Act & Assert
            Assert.ThrowsAsync<DirectionsException>(async () =>
                await directions.GetRouteAsync(
                    (0,0), (0,0), "driving-car", CancellationToken.None)
            );
        }

        [Test]
        public async Task GetRouteWrongJSONTest()//tests a request with invalid json response
        {
            //Arrange
            string invalidJson = @"{""notfeatures"": []}";
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(invalidJson)
            };
            var httpClient = MockHttpClient(response);
            var directions = new Directions(httpClient, FakeApiKey);

            //Act
            var result = await directions.GetRouteAsync(
                (16.3738, 48.2082), (14.2858, 48.3069), "driving-car", CancellationToken.None);

            //Assert
            Assert.That(result.meters, Is.EqualTo(0));
            Assert.That(result.seconds, Is.EqualTo(0));
        }       
    }
}
