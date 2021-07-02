using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyRoverServiceAPI.Exceptions;
using MyRoverServiceAPI.Persistance;
using MyRoverServiceAPI.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyRoverServiceAPI
{
    public class MyMarsRoverService : IMyMarsRoverService
    {
        
        private readonly IMarsRoverService _marsRoverService;
        private readonly ILogger<MyMarsRoverService> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly RoverApiSettings _options;
        private readonly IMyMarsRoverServiceValidator _myMarsRoverServiceValidator;
        private readonly IRoverPhotoRepository _roverPhotoRepository;

        public MyMarsRoverService(IMarsRoverService marsRoverService,
            IOptions<RoverApiSettings> options, ILogger<MyMarsRoverService> logger,
            IMemoryCache memoryCache,
            IMyMarsRoverServiceValidator myMarsRoverServiceValidator,
            IRoverPhotoRepository roverPhotoRepository)
        {
            //ToDo: implementation.Unit tests
            _marsRoverService = marsRoverService;
            _logger = logger;
            _memoryCache = memoryCache;
            _options = options.Value;
            _myMarsRoverServiceValidator = myMarsRoverServiceValidator;
            _roverPhotoRepository = roverPhotoRepository;
        }
        public async Task<MyRoverPhotosInMemory> GetImages(RoversEnum RoverName, DateTime EarthDay, CancellationToken cancellationToken)
        {
            var cacheData = _roverPhotoRepository.GetPhotos(RoverName, EarthDay);
            if (cacheData != null)
                return cacheData;
            
            await GetImagesForMarsRover(_marsRoverService, RoverName, EarthDay, cancellationToken);
            return _roverPhotoRepository.GetPhotos(RoverName, EarthDay);
        }

        private string GetCacheKey(RoversEnum RoverName, DateTime EarthDay)
        {
            return $"{MyMarsRoverServiceConstants.RoverServiceCacheKey}-{RoverName}-{EarthDay.ToString(MyMarsRoverServiceConstants.DATE_FORMAT)}";
        }
        /// <summary>
        /// 1. Asnyc Get Manifest 
        //  2. With parallel processing GetPhotos by page . Since these are I/O intensive and not cpu intensive, async will do. parallel will not add value.
        //  3.Save the photos
        /// </summary>
        /// <param name="marsRoverService"></param>
        /// <param name="RoverName"></param>
        /// <param name="inputEarthDay"></param>
        /// <returns></returns>
        private async Task<IEnumerable<string>> GetImagesForMarsRover(IMarsRoverService marsRoverService, RoversEnum RoverName, DateTime inputEarthDay, CancellationToken cancellationToken)
        {

            var curiosityManifest = await marsRoverService.GetManifest(RoverName, cancellationToken);
            _myMarsRoverServiceValidator.Validate(RoverName, curiosityManifest.PhotosManifest.LandingDate, inputEarthDay, curiosityManifest.PhotosManifest.MaxDate, curiosityManifest.PhotosManifest.Status);
            var photosForEarthDay = curiosityManifest.PhotosManifest.Photos.Find(x => x.EarthDate.Date == inputEarthDay.Date);
            var imageNames = new List<string>();
            if (photosForEarthDay == null)
                throw new MyEarthDayPhotosNotFoundException(RoverName, inputEarthDay);

            var totalPages = GetTotalPages(photosForEarthDay);
            var tasks = ProcessByPage(marsRoverService, RoverName, inputEarthDay, totalPages, cancellationToken);
            await Task.WhenAll(tasks);
                
            foreach (var task in tasks)
            {
                imageNames.AddRange(task.Result);
            }
            await _roverPhotoRepository.SavePhotos(cancellationToken);
            return imageNames;
        }

        private List<Task<IEnumerable<string>>> ProcessByPage(IMarsRoverService marsRoverService, RoversEnum RoverName, DateTime inputDay, long totalPages, CancellationToken cancellationToken)
        {
            var tasks = new List<Task<IEnumerable<string>>>();
            for (int i = 1; i <= totalPages; i++)
            {
                async Task<IEnumerable<string>> ProcessByPage()
                {

                    var roverPagePhotos = await marsRoverService.GetPhotos(RoverName, inputDay, i, cancellationToken);
                    var folderPath = $"{_options.ImagesDirectoryPath}/{RoverName}/{inputDay.ToString(MyMarsRoverServiceConstants.DATE_FORMAT)}";
                    Directory.CreateDirectory(folderPath);
                    var photoNames = new List<string>();
                    foreach (var photo in roverPagePhotos.Photos)
                    {
                        if (!string.IsNullOrWhiteSpace(photo.ImageSourceUrl))
                        {
                            //FileName is considered as unique from looking at the images names. 
                            //If this is not unique, then need to generate unique id and save
                            var UrlParts = photo.ImageSourceUrl.Split("/");
                            var fileName = UrlParts[^1];
                            await SaveImageAsync(RoverName, inputDay,marsRoverService, photo.ImageSourceUrl, fileName, cancellationToken);
                            photoNames.Add(fileName);
                        }
                    }
                    return photoNames;
                }
                tasks.Add(ProcessByPage());
            }
            return tasks;
        }

        private static long GetTotalPages(Photo photosForEarthDay)
        {
            return photosForEarthDay.TotalPhotos / MyMarsRoverServiceConstants.PAGE_SIZE + (photosForEarthDay.TotalPhotos % MyMarsRoverServiceConstants.PAGE_SIZE == 0 ? 0 : 1);
        }

        private async Task SaveImageAsync(RoversEnum Rover, DateTime EarthDay, IMarsRoverService marsRoverService, string Url, string fileName, CancellationToken cancellationToken)
        {
            
            using (var ImageStream = await marsRoverService.GetRoverPhotoImage(Url, cancellationToken))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                   await ImageStream.CopyToAsync(ms);

                    var photo = new MyRoverPhotoInMemory
                    {
                        FileName = fileName,
                        Contents = ms.ToArray()
                    };
                    _roverPhotoRepository.AddPhoto(Rover, EarthDay, photo);
                }
            }
        }
    }
}
