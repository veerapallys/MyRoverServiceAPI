using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MyRoverServiceAPI
{
    public interface IMarsRoverService
    {
        Task<MarsRoverPhotosManifest> GetManifest(RoversEnum RoverName, CancellationToken cancellationToken);
        Task<MarsRoverPhotos> GetPhotos(RoversEnum RoverName, DateTime EarthDate, int PageId, CancellationToken cancellationToken);
        Task<Stream> GetRoverPhotoImage(string Url, CancellationToken cancellationToken);
    }
}