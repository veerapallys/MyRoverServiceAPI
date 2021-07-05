using Microsoft.Extensions.DependencyInjection;
using MyRoverServiceAPI;
using MyRoverServiceAPI.Services;
using MyRoverServiceAPI.Services.RoverClientService;
using MyRoverServiceAPI.Validators;

namespace MyRoversServiceAPI.UnitTests
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IMyRoversServiceGuard, MyRoversServiceGuard>();
            services.AddScoped<IMyMarsRoverServiceValidator, MyMarsRoverServiceValidator>();
            services.AddScoped<IMarsRoverServiceValidator, MarsRoverServiceValidator>();

        }
    }
}
