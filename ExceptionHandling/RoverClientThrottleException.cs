using System;

namespace MyRoverServiceAPI.Exceptions
{
    public class RoverClientThrottleException : ApplicationException
    {
        public RoverClientThrottleException(string message): base(message)
        {

        }
    }
}
