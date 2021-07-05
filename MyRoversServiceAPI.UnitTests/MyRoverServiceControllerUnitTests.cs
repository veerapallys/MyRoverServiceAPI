using Microsoft.AspNetCore.Mvc;
using Moq;
using MyRoverServiceAPI;
using MyRoverServiceAPI.Exceptions;
using MyRoverServiceAPI.Persistance;
using MyRoverServiceAPI.Services;
using MyRoverServiceAPI.V1;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MyRoversServiceAPI.UnitTests
{
    public class MyRoverServiceControllerUnitTests
    {
        private readonly Mock<IMyMarsRoverService> _mockmyRoverService;
        private readonly Mock<IMyRoverPhotosCompressor> _mockmyRoverPhotosCompressor;
        private readonly IMyRoversServiceGuard _myRoversServiceGuard;
        private readonly CancellationToken _cancellationToken;
        private readonly MyRoverPhotosInMemory _myRoverPhotosInMemory;
        private readonly RoversEnum _roversName;
        private readonly DateTime _earthDayDate;


        public MyRoverServiceControllerUnitTests(IMyRoversServiceGuard myRoversServiceGuard)
        {
            _mockmyRoverService = new Mock<IMyMarsRoverService>();
            _mockmyRoverPhotosCompressor = new Mock<IMyRoverPhotosCompressor>();
            _cancellationToken = new CancellationToken();
            _myRoverPhotosInMemory = new MyRoverPhotosInMemory();
            _roversName = RoversEnum.Curiosity;
            _earthDayDate = DateTime.Parse("2021-06-30");
            _myRoversServiceGuard = myRoversServiceGuard;
            var lstPhotos = new List<MyRoverPhotoInMemory>();
            var p1 = new MyRoverPhotoInMemory
            {
                FileName = "p1.jpg",
                Contents = new byte[8]
            };
            var p2 = new MyRoverPhotoInMemory
            {
                FileName = "p2.jpg",
                Contents = new byte[8]
            };
            lstPhotos.Add(p1);
            lstPhotos.Add(p2);
            _myRoverPhotosInMemory.Photos = lstPhotos;
        }



        [Fact]
        public async Task GetImagesForValidRoverAndDateShouldReturnFileResult()
        {
            _myRoverPhotosInMemory.EarthDayDate = _earthDayDate;
            _myRoverPhotosInMemory.RoverName = _roversName;

            _mockmyRoverService.Setup(p => p.GetImages(_roversName, _earthDayDate,
                _cancellationToken)).ReturnsAsync(_myRoverPhotosInMemory);
            MyRoverServiceController _rs = new MyRoverServiceController(_myRoversServiceGuard, _mockmyRoverService.Object,
                _mockmyRoverPhotosCompressor.Object);
            var result = await _rs.Get(_roversName.ToString(),
                _earthDayDate.ToString(MyMarsRoverServiceConstants.DATE_FORMAT), _cancellationToken);
            //Assert
            result.ShouldBeOfType<FileContentResult>();

        }

        [Fact]
        public async Task GetImagesForInvalidDateShouldThrowException()
        {
            _myRoverPhotosInMemory.EarthDayDate = _earthDayDate;
            _myRoverPhotosInMemory.RoverName = _roversName;

            _mockmyRoverService.Setup(p => p.GetImages(_roversName, _earthDayDate,
                _cancellationToken)).ReturnsAsync(_myRoverPhotosInMemory);
            MyRoverServiceController _rs = new MyRoverServiceController(_myRoversServiceGuard, _mockmyRoverService.Object,
                _mockmyRoverPhotosCompressor.Object);
            Task act() => _rs.Get(_roversName.ToString(),
                "April 31, 2018", _cancellationToken);
            //Assert
            var exception = await Assert.ThrowsAsync<MyRoverServiceValidationException>(act);
            exception.Message.ShouldBe("The given earth date, April 31, 2018 is invalid.");
        }

        [Theory]
        [InlineData("curiosity")]
        [InlineData("Perseverance")]
        [InlineData("Opportunity")]
        [InlineData("Spirit")]
        public async Task GetImagesForValidRoverShouldReturnFileResult(string roverName)
        {
            _myRoverPhotosInMemory.EarthDayDate = _earthDayDate;
            _myRoverPhotosInMemory.RoverName = _roversName;

            _mockmyRoverService.Setup(p => p.GetImages(It.IsAny<RoversEnum>(), _earthDayDate,
                _cancellationToken)).ReturnsAsync(_myRoverPhotosInMemory);
            MyRoverServiceController _rs = new MyRoverServiceController(_myRoversServiceGuard, _mockmyRoverService.Object,
                _mockmyRoverPhotosCompressor.Object);
            var result = await _rs.Get(roverName,
                _earthDayDate.ToString(MyMarsRoverServiceConstants.DATE_FORMAT), _cancellationToken);
            //Assert
            result.ShouldBeOfType<FileContentResult>();
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("curiosity", "")]
        [InlineData("", "2020-06-30")]
        public async Task GetImagesForInvalidRoverOrDateShouldThrowException(string Rover, string earthDay)
        {
            _myRoverPhotosInMemory.EarthDayDate = _earthDayDate;
            _myRoverPhotosInMemory.RoverName = _roversName;

            _mockmyRoverService.Setup(p => p.GetImages(It.IsAny<RoversEnum>(), _earthDayDate,
                _cancellationToken)).ReturnsAsync(_myRoverPhotosInMemory);
            MyRoverServiceController _rs = new MyRoverServiceController(_myRoversServiceGuard, _mockmyRoverService.Object,
                _mockmyRoverPhotosCompressor.Object);
            Task act() => _rs.Get(Rover,
                earthDay, _cancellationToken);
            //Assert
            var exception = await Assert.ThrowsAsync<MyRoverServiceValidationException>(act);
        }

        [Theory]
        [InlineData("Future", "2020-06-30")]
        public async Task GetImagesForUnknownRoverOrDateShouldThrowNotFoundException(string Rover, string earthDay)
        {
            _myRoverPhotosInMemory.EarthDayDate = _earthDayDate;
            _myRoverPhotosInMemory.RoverName = _roversName;

            _mockmyRoverService.Setup(p => p.GetImages(It.IsAny<RoversEnum>(), _earthDayDate,
                _cancellationToken)).ReturnsAsync(_myRoverPhotosInMemory);
            MyRoverServiceController _rs = new MyRoverServiceController(_myRoversServiceGuard, _mockmyRoverService.Object,
                _mockmyRoverPhotosCompressor.Object);
            Task act() => _rs.Get(Rover,
                earthDay, _cancellationToken);
            //Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(act);
        }
    }
}
