# MyRoverServiceAPI
This is an API that fetches rovers images from Nasa API for a given earth day.

Target framework

This is based on asp.net core 5 framework.

Dependencies that can be added through Nuget

"AutoMapper" Version="10.1.1"
"AutoMapper.Extensions.Microsoft.DependencyInjection" Version="8.1.1"
"Marvin.StreamExtensions" Version="1.2.0"
"Microsoft.Extensions.Http.Polly" Version="5.0.1"
"Microsoft.Extensions.Logging" Version="5.0.0"
"Newtonsoft.Json" Version="13.0.1"
"Serilog.AspNetCore" Version="4.1.0"
"Serilog.Sinks.Console" Version="3.1.1"
"Serilog.Sinks.File" Version="5.0.0"
"Swashbuckle.AspNetCore" Version="6.1.4"

End point Details:

This API exposes a post end point  (/api/v1/rover)

Input: that accepts below arguments as input(json). 

Sample Input 
{
  "roverName": "curiosity",
  "earthDate": "2021-06-30"
}

The roverName can have one of these values {Perseverance,Curiosity,Opportunity,Spirit}. 
These are the rovers that dependent NASA API has currently. If any new rover is added, it can be added to RoversEnum.cs  file.

Output : The success response would be the list of images in a zip file that can be downloaded.

The appsettings.json file has following default values for rover that can be modified

"Rovers": {
    "ApiKey": "DEMO_KEY", // There is limited queries that can be done against NASA API per day using this key. Generate your own key @ https://api.nasa.gov/
    "BaseUrl": "https://api.nasa.gov",
    "ApiTimeOut": 60, // Default time out time
    "ApiCacheSlidingExpirationInMinutes": 15, // The data will be cached based on this sliding time value in minutes.
    "ApiCacheAbsoluteExpirationInHours": 24, // The data will be cached based on this asbolute value in hours.
    "ImagesDirectoryPath": "./Images"  // By default , the downloaded images from NASA API will be saved in this folder on server.
  }

Notes
 
 1. The API can be extended to save the images to any cloud storage as it is pluggable by implementing IMyRoverPhotosStorage.
    Currently it is saved to local.
 2. The data is cached in memory and can be extended to use distributed cache.
 3. Polly Library has been used for retries and circuitbreakers.
 4. Serilog is used for logging currently but can be replaced with any logging framework that targets asp.net core 5.
 5. The project structure is organized into relevant folders. To keep it simple, they are kept in one project. 
    These can be divided into multiple projects like one for Core (that calls NASA API) and another for API that exposes the endpoints.
 6. As there are simple validations, implemented on my own instead of adding one more dependency.
    If there is more need,I think one can use FluentValidation library.
 
 Next Coming up
 
 1. unit tests, static analysis, performance tests
 2. UI Application to display the images in a web browser
 3. Have the application run in a Docker container
 
 If you come across any issues in setting up, please let me know.
