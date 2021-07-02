using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MyRoverServiceAPI.ExceptionHandling;
using MyRoverServiceAPI.Exceptions;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace MyRoverServiceAPI.ExceptionHandling
{
    public class MyRoverExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<MyRoverExceptionHandler> _logger;

        public MyRoverExceptionHandler(RequestDelegate next, ILogger<MyRoverExceptionHandler> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await ConvertException(context, ex);
            }
        }

        private Task ConvertException(HttpContext context, Exception exception)
        {
            HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError;

            context.Response.ContentType = "application/json";

            var result = string.Empty;

            switch (exception)
            {
                    case MyRoverServiceValidationException myRoverServiceValidationException:
                    httpStatusCode = HttpStatusCode.BadRequest;
                    result = myRoverServiceValidationException.Message;
                    break;
                case MyEarthDayPhotosNotFoundException myEarthDayPhotosNotFoundException:
                    httpStatusCode = HttpStatusCode.BadRequest;
                    result = myEarthDayPhotosNotFoundException.Message;
                    break;
                case NotFoundException notFoundException:
                    httpStatusCode = HttpStatusCode.NotFound;
                    result = notFoundException.Message;
                    break;
                case RoverClientThrottleException roverClientThrottleException:
                    httpStatusCode = HttpStatusCode.TooManyRequests;
                    result = "Sorry, too many photos downloaded in a day. Have a break and come back next day!";
                    break;
                case RoverClientException roverClientException:
                    httpStatusCode = HttpStatusCode.InternalServerError;
                    result = "Sorry, be right back!";
                    break;
                case Exception ex:
                    httpStatusCode = HttpStatusCode.InternalServerError;
                    result = "Sorry, be right back!";
                    break;
            }

            context.Response.StatusCode = (int)httpStatusCode;

            result = (result != string.Empty) ? (JsonConvert.SerializeObject(result)) : (JsonConvert.SerializeObject("Sorry, be right back!"));
            _logger.LogError(exception, result);
            return context.Response.WriteAsync(result);
        }
    }
}
