using Microsoft.AspNetCore.Builder;

namespace MyRoverServiceAPI.ExceptionHandling
{
    public static class MyRoverExceptionMiddleware
    {
        public static IApplicationBuilder UseMyCustomExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<MyRoverExceptionHandler>();
        }
    }
}
