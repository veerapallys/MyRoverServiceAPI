# MyRoverServiceAPI
This is an API that fetches rovers images from Nasa API for a given earth day.

### Target framework

This is based on asp.net core 5 framework.

#### Dependencies that can be added through Nuget
```
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
```
#### End point Details:

This API exposes a **GET** end point  **(/api/v1/rover/name/{RoverName}?earthdate={EarthDate})**. This can be verified using the added swagger page **(/swagger/index.html)**

**Input**: Any valid rover and date and in a valid format can be used.
```
 /api/v1/rover/name/curiosity?earthdate=2021-06-30
 
```
**Rover Names**

These are the rovers that  NASA API has currently. If any new rover is added, it can be added to RoversEnum.cs  file.
 - Perseverance
 - Curiosity
 - Opportunity
 - Spirit 

**Sample Dates**

- 02/27/17
- June 2, 2018
- Jul-13-2016
- April 30, 2018
- 2021-06-30

Not only the above, other dates can be used as well. The above samples illustrates various date formats.

**Output** 

1. The success response would be the list of images in a zip file that can be downloaded.
2. If a given rover is not found, it would return 400 status code with a message.
3. If images are not available for a given date, it would return 400 status code with a message.
4. If too many queries happen within the window time limited by NASA API, then 429 status code would be returned.

#### Application Settings

The appsettings.json file has following default values for rover that can be modified
```
"Rovers": {
    "ApiKey": "DEMO_KEY", // There are limited queries that can be done against NASA API per day using this key. Generate your own key @ https://api.nasa.gov/
    "BaseUrl": "https://api.nasa.gov",
    "ApiTimeOut": 60, // Default timeout time
    "ApiCacheSlidingExpirationInMinutes": 15, // The data will be cached based on this sliding time value in minutes.
    "ApiCacheAbsoluteExpirationInHours": 24, // The data will be cached based on this absolute value in hours.
    "ImagesDirectoryPath": "./Images"  // By default , the downloaded images from NASA API will be saved in this folder on the server.
  }
```
### Notes
 1. The API can be extended to save the images to any cloud storage as it is pluggable by implementing IMyRoverPhotosStorage. Currently it is saved locally.
2. The data is cached in memory for performance and can be extended to use distributed cache. Also, response caching is used.
3. Polly Library has been used for retries and circuit breakers.
4. Asynchronous programming is used for performance reasons.
5. Serilog is used for logging currently but can be replaced with any logging framework that targets asp.net core 5.
6. The project structure is organized into relevant folders. To keep it simple, they are kept in one project. These can be divided into multiple projects like one for Core (that calls NASA API) and another for API that exposes the endpoints.
7. As there are simple validations, implemented on my own instead of adding one more dependency. If there is more need,I think one can use the FluentValidation library. 
8. Global Exception handling is used for better presentation of messages.
9. Filter is used to handle the date encoding issue in url path. 
10. Static Analysis is completed with Visual Studio built-in code analyzer.
11. Added unit tests in a seperate project. XUnit,Moq and Shouldly packages have been used. 
12. Added jmeter script for performance testing the API. Also added the VS profiler snapshot.

#### Next Coming up
 
 1. UI Application to display the images in a web browser
 2. Have the application run in a Docker container
 
 If you come across any issues in setting up, please let me know.

