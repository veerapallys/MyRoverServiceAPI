using System;
using System.Collections.Generic;
using System.Text;

namespace MyRoverServiceAPI
{
    public class MarsRoverEarthDayPhotos
    {
        public RoversEnum Name { get; set; }
        public DateTime EarthDayDate { get; set; }
        public IEnumerable<string> ImageFileNames { get; set; }
    }
}
