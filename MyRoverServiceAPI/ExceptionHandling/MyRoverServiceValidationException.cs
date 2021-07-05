using System;

namespace MyRoverServiceAPI.Exceptions
{
    public class MyRoverServiceValidationException : ApplicationException
    {
        public MyRoverServiceValidationException(string message) : base(message)
        {

        }
    }
}
