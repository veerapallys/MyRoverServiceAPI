using System;
using System.Collections.Generic;
using System.Text;

namespace MyRoverServiceAPI
{
    public class MyRoverEarthDayViewModel
    {
        public string Name { get; set; }
        public string EarthDayDate { get; set; }
        public IEnumerable<string> ImageLinks { get; set; }
    }
}
