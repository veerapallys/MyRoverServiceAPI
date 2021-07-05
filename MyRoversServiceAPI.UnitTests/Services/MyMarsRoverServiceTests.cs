using Moq;
using MyRoverServiceAPI;
using MyRoverServiceAPI.Exceptions;
using MyRoverServiceAPI.Persistance;
using MyRoverServiceAPI.Services;
using Shouldly;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MyRoversServiceAPI.UnitTests.Services
{
    public class MyMarsRoverServiceTests
    {
        private readonly Mock<IMarsRoverService> _marsRoverService;
        private readonly IMyMarsRoverServiceValidator _myMarsRoverServiceValidator;
        private readonly Mock<IRoverPhotoRepository> _roverPhotoRepository;
        private readonly CancellationToken _cancellationToken;
        private readonly MarsRoverPhotosManifest _marsRoverPhotosManifest;
        private readonly RoversEnum _roversName;
        private readonly DateTime _earthDayDate;
        private readonly MarsRoverPhotos _marsRoverPhotosPageOne;
        private readonly MarsRoverPhotos _marsRoverPhotosPageTwo;
        private readonly IMyMarsRoverService _myMarsRoverService;

        public MyMarsRoverServiceTests(IMyMarsRoverServiceValidator myMarsRoverServiceValidator)
        {
            _marsRoverService = new Mock<IMarsRoverService>();
            _roverPhotoRepository = new Mock<IRoverPhotoRepository>();
            _myMarsRoverServiceValidator = myMarsRoverServiceValidator;
            _roversName = RoversEnum.Curiosity;
            _earthDayDate = DateTime.Parse("2021-06-30");
            _cancellationToken = new CancellationToken();
            _marsRoverPhotosManifest = ServiceTestData.GetManifest();
            _marsRoverPhotosPageOne = ServiceTestData.GetMarsRoverPhotosPageOne();
            _marsRoverPhotosPageTwo = ServiceTestData.GetMarsRoverPhotosPageTwo();
            _myMarsRoverService = new MyMarsRoverService(_marsRoverService.Object, _myMarsRoverServiceValidator, _roverPhotoRepository.Object);
        }

        [Fact]
        public async Task GetImagesShouldReturnPhotosForValidRover()
        {
            _marsRoverService.Setup(p => p.GetManifest(_roversName, _cancellationToken)).ReturnsAsync(_marsRoverPhotosManifest);
            _roverPhotoRepository.Setup(r => r.GetPhotos(_roversName, _earthDayDate)).Returns<MyRoverPhotosInMemory>(null);
            _marsRoverService.Setup(p => p.GetPhotos(_roversName, _earthDayDate, 1, _cancellationToken)).ReturnsAsync(_marsRoverPhotosPageOne);
            _marsRoverService.Setup(p => p.GetPhotos(_roversName, _earthDayDate, 2, _cancellationToken)).ReturnsAsync(_marsRoverPhotosPageTwo);
            _marsRoverService.Setup(p => p.GetRoverPhotoImage("http://mynasa.gov/1.jpg", _cancellationToken)).ReturnsAsync(new MemoryStream(Encoding.UTF8.GetBytes("1")));
            _marsRoverService.Setup(p => p.GetRoverPhotoImage("http://mynasa.gov/2.jpg", _cancellationToken)).ReturnsAsync(new MemoryStream(Encoding.UTF8.GetBytes("2")));
            _marsRoverService.Setup(p => p.GetRoverPhotoImage("http://mynasa.gov/3.jpg", _cancellationToken)).ReturnsAsync(new MemoryStream(Encoding.UTF8.GetBytes("3")));
            _marsRoverService.Setup(p => p.GetRoverPhotoImage("http://mynasa.gov/4.jpg", _cancellationToken)).ReturnsAsync(new MemoryStream(Encoding.UTF8.GetBytes("4")));
            _marsRoverService.Setup(p => p.GetRoverPhotoImage("http://mynasa.gov/5.jpg", _cancellationToken)).ReturnsAsync(new MemoryStream(Encoding.UTF8.GetBytes("5")));
            _marsRoverService.Setup(p => p.GetRoverPhotoImage("http://mynasa.gov/6.jpg", _cancellationToken)).ReturnsAsync(new MemoryStream(Encoding.UTF8.GetBytes("6")));
            _roverPhotoRepository.Setup(r => r.AddPhoto(_roversName, _earthDayDate, It.IsAny<MyRoverPhotoInMemory>())).Returns(true);
            var result = await _myMarsRoverService.GetImages(_roversName, _earthDayDate, _cancellationToken);
            _marsRoverService.Verify(mock => mock.GetPhotos(_roversName, _earthDayDate, 1, _cancellationToken), Times.Exactly(1));
            _marsRoverService.Verify(mock => mock.GetPhotos(_roversName, _earthDayDate, 2, _cancellationToken), Times.Exactly(1));

            _marsRoverService.Verify(mock => mock.GetRoverPhotoImage("http://mynasa.gov/1.jpg", _cancellationToken), Times.Once());
            _marsRoverService.Verify(mock => mock.GetRoverPhotoImage("http://mynasa.gov/2.jpg", _cancellationToken), Times.Once());
            _marsRoverService.Verify(mock => mock.GetRoverPhotoImage("http://mynasa.gov/3.jpg", _cancellationToken), Times.Once());
            _marsRoverService.Verify(mock => mock.GetRoverPhotoImage("http://mynasa.gov/4.jpg", _cancellationToken), Times.Once());
            _marsRoverService.Verify(mock => mock.GetRoverPhotoImage("http://mynasa.gov/5.jpg", _cancellationToken), Times.Once());
            _marsRoverService.Verify(mock => mock.GetRoverPhotoImage("http://mynasa.gov/6.jpg", _cancellationToken), Times.Once());
            _roverPhotoRepository.Verify(r => r.AddPhoto(_roversName, _earthDayDate, It.IsAny<MyRoverPhotoInMemory>()), Times.Exactly(6));
        }

        [Fact]
        public async Task GetImagesShouldReturnPhotosFromCacheIfPresentForValidRover()
        {
            _roverPhotoRepository.Setup(r => r.GetPhotos(_roversName, _earthDayDate)).Returns(new MyRoverPhotosInMemory { });
            var result = await _myMarsRoverService.GetImages(_roversName, _earthDayDate, _cancellationToken);
            result.ShouldBeOfType<MyRoverPhotosInMemory>();
        }

        [Fact]
        public async Task GetImagesShouldReturnNotFoundExceptionIfDateDoesntExist()
        {
            _marsRoverService.Setup(p => p.GetManifest(_roversName, _cancellationToken)).ReturnsAsync(_marsRoverPhotosManifest);
            _roverPhotoRepository.Setup(r => r.GetPhotos(_roversName, _earthDayDate)).Returns<MyRoverPhotosInMemory>(null);
            Task act() => _myMarsRoverService.GetImages(_roversName, DateTime.Now.Date, _cancellationToken);
            var exception = await Assert.ThrowsAsync<MyEarthDayPhotosNotFoundException>(act);
        }
    }
}
