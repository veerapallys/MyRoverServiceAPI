using System.Threading.Tasks;

namespace MyRoverServiceAPI.Persistance
{
    public interface IMyRoverPhotosStorage
    {
        Task<bool> SavePhotos(MyRoverPhotosInMemory photos, System.Threading.CancellationToken cancellationToken = default);
    }
}
