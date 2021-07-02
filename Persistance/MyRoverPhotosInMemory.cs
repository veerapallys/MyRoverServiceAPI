using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyRoverServiceAPI.Persistance
{
    public class MyRoverPhotosInMemory
    {
        public RoversEnum RoverName { get; set; }
        public DateTime EarthDayDate { get; set; }
        public List<MyRoverPhotoInMemory> Photos { get; set; }
    }
}
