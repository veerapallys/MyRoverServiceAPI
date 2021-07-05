using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using MyRoverServiceAPI;
using MyRoverServiceAPI.Services.RoverClientService;
using Newtonsoft.Json;
using Shouldly;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MyRoversServiceAPI.UnitTests.Services.RoverClientService
{
    public class MarsRoverServiceUnitTests
    {
        private readonly Mock<ILogger<MarsRoverService>> _logger;
        private readonly IMarsRoverServiceValidator _marsRoverServiceValidator;
        private readonly Mock<IHttpClientFactory> _httpClientFactory;
        private readonly Mock<IOptions<RoverApiSettings>> _options;
        private readonly RoversEnum _roversName;
        private readonly DateTime _earthDayDate;
        private readonly CancellationToken _cancellationToken;
        private readonly MarsRoverPhotosManifest _marsRoverPhotosManifest;
        private readonly MarsRoverPhotos _marsRoverPhotosPageOne;
        

        public MarsRoverServiceUnitTests(IMarsRoverServiceValidator marsRoverServiceValidator)
        {
            _roversName = RoversEnum.Curiosity;
            _earthDayDate = DateTime.Parse("2021-06-30");
            _cancellationToken = new CancellationToken();
            _logger = new Mock<ILogger<MarsRoverService>>();
            _httpClientFactory = new Mock<IHttpClientFactory>();
            _options = new Mock<IOptions<RoverApiSettings>>();
            _marsRoverServiceValidator = marsRoverServiceValidator;
            _marsRoverPhotosManifest = ServiceTestData.GetManifest();
            _marsRoverPhotosPageOne = ServiceTestData.GetMarsRoverPhotosPageOne();
       }

        [Fact]
        public async Task GetManifestShouldReturnManifestForValidRoverAndDate()
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(_marsRoverPhotosManifest))
                });

            var client = new HttpClient(mockHttpMessageHandler.Object);
            _httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            _options.Setup(op => op.Value).Returns(new RoverApiSettings
            { ApiKey = "DemoKey", BaseUrl = "http://mynasa.gov", ImagesDirectoryPath = "./Images", ApiTimeOut = 60, ApiCacheAbsoluteExpirationInHours = 24, ApiCacheSlidingExpirationInMinutes = 15 });

            var marsRoverService = new MarsRoverService(_logger.Object, _options.Object,
                _httpClientFactory.Object, _marsRoverServiceValidator);

            var result = await marsRoverService.GetManifest(_roversName, _cancellationToken);
            result.PhotosManifest.LandingDate.ToString("yyyy-MM-dd").ShouldBe("2017-06-30");
        }

        [Fact]
        public async Task GetPhotosShouldReturnPhotosForValidRoverAndDate()
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(_marsRoverPhotosPageOne))
                });

            var client = new HttpClient(mockHttpMessageHandler.Object);
            _httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            _options.Setup(op => op.Value).Returns(new RoverApiSettings
            { ApiKey = "DemoKey", BaseUrl = "http://mynasa.gov", ImagesDirectoryPath = "./Images", ApiTimeOut = 60, ApiCacheAbsoluteExpirationInHours = 24, ApiCacheSlidingExpirationInMinutes = 15 });

            var marsRoverService = new MarsRoverService(_logger.Object, _options.Object,
                _httpClientFactory.Object, _marsRoverServiceValidator);

            var result = await marsRoverService.GetPhotos(_roversName, _earthDayDate, 1, _cancellationToken);
            result.Photos.Length.ShouldBe(3);
        }

        [Fact]
        public async Task GetRoverPhotoImageShouldReturnStreamForValidRoverAndDate()
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("This is stream")
                });

            var client = new HttpClient(mockHttpMessageHandler.Object);
            _httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            _options.Setup(op => op.Value).Returns(new RoverApiSettings
            { ApiKey = "DemoKey", BaseUrl = "http://mynasa.gov", ImagesDirectoryPath = "./Images", ApiTimeOut = 60, ApiCacheAbsoluteExpirationInHours = 24, ApiCacheSlidingExpirationInMinutes = 15 });

            var marsRoverService = new MarsRoverService(_logger.Object, _options.Object,
                _httpClientFactory.Object, _marsRoverServiceValidator);

            var result = await marsRoverService.GetRoverPhotoImage("http://mynasa.gov/1.jpg", _cancellationToken);
            result.ShouldBeOfType<MemoryStream>();
        }
    }
}
