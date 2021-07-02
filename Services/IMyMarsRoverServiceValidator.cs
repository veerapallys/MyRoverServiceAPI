using MyRoverServiceAPI.Exceptions;
using System;

namespace MyRoverServiceAPI.Services
{
    public interface IMyMarsRoverServiceValidator
    {
        bool Validate(RoversEnum Rover, DateTime landingDate, DateTime earthDay, DateTime maxDate, string RoverStatus);
        
    }
}
