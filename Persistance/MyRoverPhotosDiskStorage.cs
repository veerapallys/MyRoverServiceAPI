using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MyRoverServiceAPI.Persistance
{
    public class MyRoverPhotosDiskStorage : IMyRoverPhotosStorage
    {
        private readonly ILogger<MyMarsRoverService> _logger;
        private readonly RoverApiSettings _options;

        public MyRoverPhotosDiskStorage(IOptions<RoverApiSettings> options, ILogger<MyMarsRoverService> logger)
        {
            _logger = logger;
            _options = options.Value;
        }
        public async Task<bool> SavePhotos(MyRoverPhotosInMemory photos, System.Threading.CancellationToken cancellationToken = default)
        {
            if (photos == null)
                return false;

            var folderPath = $"{_options.ImagesDirectoryPath}/{photos.RoverName}/{photos.EarthDayDate.ToString(MyMarsRoverServiceConstants.DATE_FORMAT)}";
            Directory.CreateDirectory(folderPath);
            
            List<Task> tasks = new List<Task>();
            foreach (var item in photos.Photos)
            {
               tasks.Add(SaveImageToDisc($"{folderPath}/{item.FileName}", item.Contents, cancellationToken));
            }
              await Task.WhenAll(tasks);
            return true; 
        }

        private async Task SaveImageToDisc(string filePath, byte[] ImageData, System.Threading.CancellationToken cancellationToken = default)
        {
            await File.WriteAllBytesAsync(filePath, ImageData, cancellationToken);
        }
    }
}
