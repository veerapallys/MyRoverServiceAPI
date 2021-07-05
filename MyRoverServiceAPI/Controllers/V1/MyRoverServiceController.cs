using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyRoverServiceAPI.Filters;
using MyRoverServiceAPI.Services;
using System;
using System.Threading;
using System.Threading.Tasks;


namespace MyRoverServiceAPI.V1
{
    [Route("api/v1/rover")]
    [ApiController]
    public class MyRoverServiceController : ControllerBase
    {
        private readonly IMyRoversServiceGuard _myRoversServiceGuard;
        private readonly IMyMarsRoverService _myMarsRoverService;
        private readonly IMyRoverPhotosCompressor _myRoverPhotosCompressor;

        public MyRoverServiceController(IMyRoversServiceGuard myRoversServiceGuard,
                                        IMyMarsRoverService myMarsRoverService,
                                        IMyRoverPhotosCompressor myRoverPhotosCompressor)
        {
            _myRoversServiceGuard = myRoversServiceGuard;
            _myMarsRoverService = myMarsRoverService;
            _myRoverPhotosCompressor = myRoverPhotosCompressor;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/zip")]
        [Route("name/{RoverName}")]
        [HttpGet]
        [MyUrlDateDecoder("EarthDate")]
        public async Task<ActionResult> Get(string RoverName, string EarthDate, CancellationToken cancellationToken)
        {
            _myRoversServiceGuard.GuardGetRoverImages(RoverName, EarthDate);
            _ = Enum.TryParse(RoverName, true, out RoversEnum roverInput);
            _ = DateTime.TryParse(EarthDate, out DateTime earthDateInput);
            var roverImages = await _myMarsRoverService.GetImages(roverInput, earthDateInput, cancellationToken);
            var zipstream = await _myRoverPhotosCompressor.GetImagesAsZipStream(roverImages);

            return File(zipstream, "application/zip", GetzipName(roverInput, earthDateInput));
        }

        private static string GetzipName(RoversEnum roverName, DateTime earthDay)
        {

            return $"{roverName}-{earthDay.ToString(MyMarsRoverServiceConstants.DATE_FORMAT)}-{Guid.NewGuid()}.zip";
        }



    }
}
