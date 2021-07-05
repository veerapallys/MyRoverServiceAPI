using MyRoverServiceAPI.Exceptions;
using System;

namespace MyRoverServiceAPI.Validators
{
    public class MyRoversServiceGuard : IMyRoversServiceGuard
    {
        public void GuardGetRoverImages(string RoverName, string earthDay)
        {
            if (string.IsNullOrWhiteSpace(RoverName))
                throw new MyRoverServiceValidationException("Rover Name is required");
            if (string.IsNullOrWhiteSpace(earthDay))
                throw new MyRoverServiceValidationException("Earth date is required");
            if (!Enum.TryParse(RoverName, true, out RoversEnum _))
                throw new NotFoundException("Rover", RoverName);
            if (!DateTime.TryParse(earthDay, out DateTime _))
                throw new MyRoverServiceValidationException($"The given earth date, {earthDay} is invalid.");
        }
    }
}
