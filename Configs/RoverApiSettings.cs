using System;
using System.Collections.Generic;
using System.Text;

namespace MyRoverServiceAPI
{
    public class RoverApiSettings
    {
        public string ApiKey { get; set; }
        public string BaseUrl { get; set; }
        public int ApiTimeOut { get; set; }

        public int ApiCacheSlidingExpirationInMinutes { get; set; }
        public int ApiCacheAbsoluteExpirationInHours { get; set; }

        public string ImagesDirectoryPath { get; set; }
        
    }
}
