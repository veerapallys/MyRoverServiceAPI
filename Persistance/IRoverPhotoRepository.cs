using System;
using System.Threading.Tasks;

namespace MyRoverServiceAPI.Persistance
{
    public interface IRoverPhotoRepository
    {
        Task<MyRoverPhotosInMemory> SavePhotos(System.Threading.CancellationToken cancellationToken = default);
        MyRoverPhotosInMemory GetPhotos(RoversEnum Rover, DateTime EarthDay);
        bool AddPhoto(RoversEnum Rover, DateTime EarthDay, MyRoverPhotoInMemory photo);
    }
}
