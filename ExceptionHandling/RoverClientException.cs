using System;

namespace MyRoverServiceAPI.Exceptions
{
    public class RoverClientException : ApplicationException
    {
        public RoverClientException(string message) : base(message)
        {

        }
    }
}
