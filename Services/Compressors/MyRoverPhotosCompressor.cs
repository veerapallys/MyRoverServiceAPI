using Microsoft.Extensions.Options;
using MyRoverServiceAPI.Persistance;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace MyRoverServiceAPI.Services
{
    public class MyRoverPhotosCompressor : IMyRoverPhotosCompressor
    {
        private readonly RoverApiSettings _options;
        public MyRoverPhotosCompressor(IOptions<RoverApiSettings> options)
        {
            _options = options.Value;
        }

        public async Task<byte[]> GetImagesAsZipStream(MyRoverPhotosInMemory roverPhotos)
        {
            var Imagespath = $"{Environment.CurrentDirectory}/{_options.ImagesDirectoryPath}/{roverPhotos.RoverName}/{roverPhotos.EarthDayDate.ToString(MyMarsRoverServiceConstants.DATE_FORMAT)}/";

            using var memoryStream = new MemoryStream();
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var photo in roverPhotos.Photos)
                {
                    var zipArchiveEntry = archive.CreateEntry(photo.FileName, CompressionLevel.Fastest);
                    using var zipStream = zipArchiveEntry.Open();
                    var fullpath = $"{ Imagespath}/{photo.FileName}";
                    using MemoryStream fs = new MemoryStream(photo.Contents);
                    await fs.CopyToAsync(zipStream);
                }
            }
            memoryStream.Position = 0;
            return memoryStream.ToArray();
        }

    }
}
