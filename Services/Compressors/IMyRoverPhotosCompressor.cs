using MyRoverServiceAPI.Persistance;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyRoverServiceAPI.Services
{
    public interface IMyRoverPhotosCompressor
    {
        Task<byte[]> GetImagesAsZipStream(MyRoverPhotosInMemory marsRoverEarthDayPhotos);
    }
}
