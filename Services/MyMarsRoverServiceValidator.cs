using MyRoverServiceAPI.Exceptions;
using System;

namespace MyRoverServiceAPI.Services
{
    public class MyMarsRoverServiceValidator : IMyMarsRoverServiceValidator
    {

        public bool Validate(RoversEnum Rover, DateTime landingDate, DateTime earthDay, DateTime maxDate, string RoverStatus)
        {
            if (earthDay.Date < landingDate.Date)
                throw new MyEarthDayPhotosNotFoundException(Rover, earthDay, $"The landing date for rover is {landingDate.ToString(MyMarsRoverServiceConstants.DATE_FORMAT)}. Please try using a date greather than landing date upto today."); ;

            if (earthDay.Date > DateTime.Now.Date)
                throw new MyEarthDayPhotosNotFoundException(Rover, earthDay, $"The landing date for rover is {landingDate.ToString(MyMarsRoverServiceConstants.DATE_FORMAT)}. Please try using a date greather than landing date upto today.");

            if (earthDay.Date > maxDate.Date)
                throw new MyEarthDayPhotosNotFoundException(Rover, earthDay, $"The landing date for rover is {landingDate.ToString(MyMarsRoverServiceConstants.DATE_FORMAT)} and max date is {maxDate.ToString(MyMarsRoverServiceConstants.DATE_FORMAT)}. Please use a date that is in between them.");

            if (!RoverStatus.Trim().Equals("active") && earthDay.Date > maxDate.Date)
                throw new MyEarthDayPhotosNotFoundException(Rover, earthDay, $"The landing date for rover is {landingDate.ToString(MyMarsRoverServiceConstants.DATE_FORMAT)} and max date is {maxDate.ToString(MyMarsRoverServiceConstants.DATE_FORMAT)}. Please use a date that is in between them.");

            return true;
        }
    }
}
