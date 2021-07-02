using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyRoverServiceAPI.ExceptionHandling;
using MyRoverServiceAPI.Persistance;
using MyRoverServiceAPI.Services;
using MyRoverServiceAPI.Services.RoverClientService;
using MyRoverServiceAPI.Validators;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace MyRoverServiceAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddResponseCaching();
            AddHttpClientAndPolly(services);

            services.AddControllers();
            services.AddMemoryCache();
            services.AddOptions<RoverApiSettings>().Bind(Configuration.GetSection("Rovers"));
            AddRoverServices(services);


            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddSwaggerGen();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAny", builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });
        }

        private void AddRoverServices(IServiceCollection services)
        {
            services.AddScoped<IMyRoversServiceGuard, MyRoversServiceGuard>();
            services.AddScoped<IMarsRoverService, MarsRoverService>();
            services.AddScoped<IMyMarsRoverService, MyMarsRoverService>();
            services.AddScoped<IMyMarsRoverServiceValidator, MyMarsRoverServiceValidator>();
            services.AddScoped<IMarsRoverServiceValidator, MarsRoverServiceValidator>();
            services.AddScoped<IMyRoverPhotosCompressor, MyRoverPhotosCompressor>();
            services.AddScoped<IMyRoverPhotosStorage, MyRoverPhotosDiskStorage>();
            services.AddScoped<IRoverPhotoRepository, RoverPhotoRepository>();
        }

        private void AddHttpClientAndPolly(IServiceCollection services)
        {
            var retryPolicy = HttpPolicyExtensions
                            .HandleTransientHttpError()
                            .Or<TimeoutRejectedException>() 
                            .WaitAndRetryAsync(new[]
                                {
                                    TimeSpan.FromSeconds(1),
                                    TimeSpan.FromSeconds(5),
                                    TimeSpan.FromSeconds(10)
                                });
            var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(10);
            services.AddHttpClient("RoverApiClient", client =>
            {
                client.BaseAddress = new Uri("https://api.nasa.gov");
                client.Timeout = new TimeSpan(0, 0, 60);
                client.DefaultRequestHeaders.Clear();
            })
             .ConfigurePrimaryHttpMessageHandler(handler =>
                        new HttpClientHandler()
                        {
                            AutomaticDecompression = System.Net.DecompressionMethods.GZip
                        })
             .AddPolicyHandler(retryPolicy)
             .AddPolicyHandler(timeoutPolicy)
             .AddTransientHttpErrorPolicy(builder => builder.CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 3,
                    durationOfBreak: TimeSpan.FromSeconds(60)
                  ));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseSwagger();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My Rovers Service V1");
            });

            app.UseRouting();
            app.UseResponseCaching();
            app.Use(async (context, next) =>
            {
                context.Response.GetTypedHeaders().CacheControl =
                    new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
                    {
                        Public = true,
                        MaxAge = TimeSpan.FromSeconds(10)
                    };
                context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Vary] =
                    new string[] { "Accept-Encoding" };

                await next();
            });

            app.UseMyCustomExceptionHandler();

            app.UseCors("AllowAny");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
