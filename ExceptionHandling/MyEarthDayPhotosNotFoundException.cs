using System;

namespace MyRoverServiceAPI.Exceptions
{
    public class MyEarthDayPhotosNotFoundException : ApplicationException
    {
        public MyEarthDayPhotosNotFoundException(RoversEnum Rover, DateTime EarthDay, string additionalInformation = "")
            : base($"Photos for {Rover} are not found for earth day, {EarthDay.ToString("yyyy-MM-dd")}. {additionalInformation}")
        {
        }
    }
}
