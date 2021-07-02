using System;

namespace MyRoverServiceAPI.Services.RoverClientService
{
    public interface IMarsRoverServiceValidator
    {
        bool ValidatePage(int pageId, RoversEnum RoverName, DateTime EarthDate);
    }
}
