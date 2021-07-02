using MyRoverServiceAPI.Persistance;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyRoverServiceAPI
{
    public interface IMyMarsRoverService
    {
        Task<MyRoverPhotosInMemory> GetImages(RoversEnum RoverName, DateTime EarthDay, CancellationToken cancellationToken);
    }
}
