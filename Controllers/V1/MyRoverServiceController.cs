using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyRoverServiceAPI.Services;
using MyRoverServiceAPI.ViewModels;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;


namespace MyRoverServiceAPI.V1
{
    [Route("api/v1/rover")]
    [ApiController]
    public class MyRoverServiceController : ControllerBase
    {
        private readonly ILogger<MyRoverServiceController> _logger;
        private readonly IMyRoversServiceGuard _myRoversServiceGuard;
        private readonly IMyMarsRoverService _myMarsRoverService;
        private readonly IMapper _mapper;
        private readonly RoverApiSettings _options;
        private readonly IMyRoverPhotosCompressor _myRoverPhotosCompressor;

        public MyRoverServiceController(ILogger<MyRoverServiceController> logger, 
                                        IMyRoversServiceGuard myRoversServiceGuard,
                                        IOptions<RoverApiSettings> options,
                                        IMyMarsRoverService myMarsRoverService,
                                        IMapper mapper,
                                        IMyRoverPhotosCompressor myRoverPhotosCompressor)
        {
            _logger = logger;
            _myRoversServiceGuard = myRoversServiceGuard;
            _myMarsRoverService = myMarsRoverService;
            _mapper = mapper;
            _options = options.Value;
            _myRoverPhotosCompressor = myRoverPhotosCompressor;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<ActionResult> Get(RoverInputViewModel roverInputModel, CancellationToken cancellationToken)
        {
            _myRoversServiceGuard.GuardGetRoverImages(roverInputModel.RoverName, roverInputModel.EarthDate);
            _ = Enum.TryParse(roverInputModel.RoverName, true,out RoversEnum roverInput);
            _ = DateTime.TryParse(roverInputModel.EarthDate, out DateTime earthDateInput);
            var roverImages = await _myMarsRoverService.GetImages(roverInput, earthDateInput, cancellationToken);
            var zipstream = await _myRoverPhotosCompressor.GetImagesAsZipStream(roverImages);
            
            return File(zipstream, "application/zip", getzipName(roverInput, earthDateInput));
        }

        private string getzipName(RoversEnum roverName, DateTime earthDay)
        {

            return $"{roverName}-{earthDay.ToString(MyMarsRoverServiceConstants.DATE_FORMAT)}-{Guid.NewGuid()}.zip";
        }

        
        
    }
}
