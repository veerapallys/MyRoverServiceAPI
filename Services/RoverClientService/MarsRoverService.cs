using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Net.Http.Headers;
using Marvin.StreamExtensions;
using MyRoverServiceAPI.Exceptions;
using MyRoverServiceAPI.Services.RoverClientService;

namespace MyRoverServiceAPI
{
    public class MarsRoverService : IMarsRoverService
    {
        private readonly string API_KEY;
        private readonly ILogger<MarsRoverService> _logger;
        private readonly IMarsRoverServiceValidator _marsRoverServiceValidator;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly RoverApiSettings _options;
        
        public MarsRoverService(ILogger<MarsRoverService> logger,
            IOptions<RoverApiSettings> options, 
            IHttpClientFactory httpClientFactory,
            IMarsRoverServiceValidator marsRoverServiceValidator)
        {
            _httpClientFactory = httpClientFactory;
            _options = options.Value;
            API_KEY = _options.ApiKey;
            _logger = logger;
            _marsRoverServiceValidator = marsRoverServiceValidator;
        }


        public async Task<MarsRoverPhotosManifest> GetManifest(RoversEnum RoverName, CancellationToken cancellationToken)
        {
            var httpClient = _httpClientFactory.CreateClient("RoverApiClient");
            var url = GetManifestUrl(RoverName);
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            using (var response = await httpClient.SendAsync(request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken))
            {
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity
                        || response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        var errorStream = await response.Content.ReadAsStreamAsync();
                        var validationErrors = errorStream.ReadAndDeserializeFromJson();
                        _logger.LogError(MarsRoverServiceErrorMessageHelper.GetBadRequestMessage(response.StatusCode, RoverName, "GetManifest", url, validationErrors));
                        throw new MyRoverServiceValidationException($"Bad request, Rover Name : {RoverName}");
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        _logger.LogError(MarsRoverServiceErrorMessageHelper.GetNotFoundMessage(response.StatusCode, RoverName, "GetManifest", url));
                        throw new NotFoundException("Rover", RoverName);
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    {
                        var message = MarsRoverServiceErrorMessageHelper.GetTooManyRequestMessage(response.StatusCode, RoverName, "GetManifest", url, API_KEY);
                        _logger.LogError(message);
                        throw new RoverClientThrottleException(message);
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    {
                        var message = MarsRoverServiceErrorMessageHelper.GetUnAuthorizedMessage(response.StatusCode, RoverName, "GetManifest", url, API_KEY);
                        _logger.LogError(message);
                        throw new RoverClientException(message);
                    }
                    
                    response.EnsureSuccessStatusCode();
                }
                var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

                var roverPhotosManifest = stream.ReadAndDeserializeFromJson<MarsRoverPhotosManifest>();
                return roverPhotosManifest;
            }
        }

        public async Task<MarsRoverPhotos> GetPhotos(RoversEnum RoverName, DateTime EarthDate, int PageId, CancellationToken cancellationToken)
        {
            _marsRoverServiceValidator.ValidatePage(PageId,RoverName,EarthDate);
            var httpClient = _httpClientFactory.CreateClient("RoverApiClient");
            var url = GetPhotosUrl(RoverName, EarthDate, PageId);
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            using (var response = await httpClient.SendAsync(request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken))
            {
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity
                        || response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        var errorStream = await response.Content.ReadAsStreamAsync();
                        var validationErrors = errorStream.ReadAndDeserializeFromJson();
                        _logger.LogError(MarsRoverServiceErrorMessageHelper.GetBadRequestMessage(response.StatusCode, RoverName, "GetPhotos", url, validationErrors));
                        throw new MyRoverServiceValidationException($"Bad request, Rover Name : {RoverName}");
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        _logger.LogError(MarsRoverServiceErrorMessageHelper.GetNotFoundMessage(response.StatusCode, RoverName, "GetPhotos", url));
                        throw new NotFoundException("Rover", RoverName);
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    {
                        var message = MarsRoverServiceErrorMessageHelper.GetTooManyRequestMessage(response.StatusCode, RoverName, "GetManifest", url, API_KEY);
                        _logger.LogError(message);
                        throw new RoverClientThrottleException(message);
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    {
                        var message = MarsRoverServiceErrorMessageHelper.GetUnAuthorizedMessage(response.StatusCode, RoverName, "GetPhotos", url, API_KEY);
                        _logger.LogError(message);
                        throw new RoverClientException(message);
                    }
                    response.EnsureSuccessStatusCode();
                }
                var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

                var roverPhotos = stream.ReadAndDeserializeFromJson<MarsRoverPhotos>();
                return roverPhotos;
            }
        }


        public async Task<Stream> GetRoverPhotoImage(string Url, CancellationToken cancellationToken)
        {
            var httpClient = _httpClientFactory.CreateClient("RoverApiClient");
            var request = new HttpRequestMessage(HttpMethod.Get, Url);
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            var response = await httpClient.SendAsync(request,
                 HttpCompletionOption.ResponseHeadersRead,
                 cancellationToken);
            
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    {
                        var message = MarsRoverServiceErrorMessageHelper.GetTooManyRequestMessage(response.StatusCode, Url, API_KEY);
                        _logger.LogError(message);
                        throw new RoverClientThrottleException(message);
                    }
                    else
                    {
                        var message = $"method: GetRoverPhotoImage, Error returned by api client for url : {Url}, status code : {response.StatusCode} ";
                        _logger.LogError(message);
                        throw new RoverClientException(message);
                    }
                }
            response.EnsureSuccessStatusCode();
            var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

                return stream;
       }

        private string GetPhotosUrl(RoversEnum RoverName, DateTime EarthDate, int PageId)
        {
            var earthDateString = EarthDate.ToString(MyMarsRoverServiceConstants.DATE_FORMAT);
            return $"{_options.BaseUrl}/mars-photos/api/v1/rovers/{RoverName}/photos?api_key={API_KEY}&earth_date={earthDateString}&page={PageId}";
        }

        private string GetManifestUrl(RoversEnum RoverName)
        {
            return $"{_options.BaseUrl}/mars-photos/api/v1/manifests/{RoverName}?api_key={API_KEY}";
        }
    }
}