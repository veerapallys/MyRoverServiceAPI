using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace MyRoverServiceAPI
{
    public static class MarsRoverServiceErrorMessageHelper
    {
        public static string GetBadRequestMessage(HttpStatusCode StatusCode, RoversEnum RoverName, string methodName, string url, object validationErrors)
        {
            return $"Statuscode:{StatusCode} : Bad request for the requested Rover : {RoverName}, method : {methodName} , URL: {url} , Message : {validationErrors}";
        }

        public static string GetNotFoundMessage(HttpStatusCode StatusCode, RoversEnum RoverName, string methodName, string url)
        {
            return $"method: {methodName}, Statuscode:{StatusCode} : The requested Rover : {RoverName} manifest is not found at {url}";
        }

        public static string GetUnAuthorizedMessage(HttpStatusCode StatusCode, RoversEnum RoverName, string methodName, string url, string ApiKey)
        {
            return $"method: {methodName}, Statuscode:{StatusCode}, RoverName: {RoverName} , URL : {url} returned Access denied for access key : {ApiKey}";
        }

        public static string GetTooManyRequestMessage(HttpStatusCode StatusCode, RoversEnum RoverName, string methodName, string url, string ApiKey)
        {
            return $"method: {methodName}, Statuscode:{StatusCode}, RoverName: {RoverName} , URL : {url} returned, Too many requests for access key : {ApiKey}";
        }

        public static string GetTooManyRequestMessage(HttpStatusCode StatusCode, string url, string ApiKey)
        {
            return $"Statuscode:{StatusCode},  URL : {url} returned, Too many requests for access key : {ApiKey}";
        }
    }
}
