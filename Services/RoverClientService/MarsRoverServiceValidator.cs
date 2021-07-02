using System;
using MyRoverServiceAPI.Exceptions;

namespace MyRoverServiceAPI.Services.RoverClientService
{
    public class MarsRoverServiceValidator : IMarsRoverServiceValidator
    {
        public bool ValidatePage(int pageId,RoversEnum RoverName, DateTime EarthDate)
        {
            if (pageId <= 0)
                throw new RoverClientException($"Page id, {pageId} is out of range for Rover {RoverName} and earth date {EarthDate.ToString(MyMarsRoverServiceConstants.DATE_FORMAT)}");
            return true;
        }
    }
}
