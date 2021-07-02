using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyRoverServiceAPI.Persistance
{
    public class RoverPhotoRepository : IRoverPhotoRepository
    {
        private readonly ILogger<MyMarsRoverService> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly RoverApiSettings _options;
        private readonly IMyRoverPhotosStorage _myRoverPhotosStorage;
        private MyRoverPhotosInMemory _photos;
        public RoverPhotoRepository(IOptions<RoverApiSettings> options, ILogger<MyMarsRoverService> logger,
            IMemoryCache memoryCache, IMyRoverPhotosStorage myRoverPhotosStorage)
        {
            _logger = logger;
            _memoryCache = memoryCache;
            _options = options.Value;
            _myRoverPhotosStorage = myRoverPhotosStorage;
        }

        public bool AddPhoto(RoversEnum Rover, DateTime EarthDay,MyRoverPhotoInMemory photo)
        {
            if (_photos == null)
            {
                _photos = new MyRoverPhotosInMemory
                {
                    EarthDayDate = EarthDay,
                    RoverName = Rover,
                    Photos = new List<MyRoverPhotoInMemory>()
                }; 
            }
            _photos.Photos.Add(photo);
            return true;
        }

        public async Task<MyRoverPhotosInMemory> SavePhotos(System.Threading.CancellationToken cancellationToken = default)
        {
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(_options.ApiCacheAbsoluteExpirationInHours),
                SlidingExpiration = TimeSpan.FromMinutes(_options.ApiCacheSlidingExpirationInMinutes)
            };
           var result = _memoryCache.Set(GetCacheKey(_photos.RoverName, _photos.EarthDayDate), _photos, cacheOptions);
            await _myRoverPhotosStorage.SavePhotos(_photos);
            return result;
        }

        public MyRoverPhotosInMemory GetPhotos(RoversEnum Rover, DateTime EarthDay)
        {
            if (_memoryCache.TryGetValue(GetCacheKey(Rover, EarthDay), out MyRoverPhotosInMemory cmarsRoverEarthDayPhotos))
            {
                return cmarsRoverEarthDayPhotos;
            }
            return null;
        }

        private string GetCacheKey(RoversEnum RoverName, DateTime EarthDay)
        {
            return $"{MyMarsRoverServiceConstants.RoverServiceCacheKey}-{RoverName}-{EarthDay.ToString(MyMarsRoverServiceConstants.DATE_FORMAT)}";
        }

        
    }
}
