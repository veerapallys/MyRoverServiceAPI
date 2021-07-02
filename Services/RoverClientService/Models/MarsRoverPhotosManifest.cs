using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace MyRoverServiceAPI
{
    public class MarsRoverPhotosManifest
    {
        [JsonProperty("photo_manifest")]
        public RoverPhotosManifest PhotosManifest { get; set; }
    }
    public class RoverPhotosManifest
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("landing_date")]
        public DateTime LandingDate { get; set; }
        [JsonProperty("launch_date")]
        public DateTime LaunchingDate { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("max_sol")]
        public long MaxSol { get; set; }
        [JsonProperty("max_date")]
        public DateTime MaxDate { get; set; }
        [JsonProperty("total_photos")]
        public long TotalPhotos { get; set; }
        [JsonProperty("photos")]
        public List<Photo> Photos { get; set; }
    }

    public class Photo
    {
        [JsonProperty("sol")]
        public long Sol { get; set; }
        [JsonProperty("earth_date")]
        public DateTime EarthDate { get; set; }
        [JsonProperty("total_photos")]
        public long TotalPhotos { get; set; }
        [JsonProperty("cameras")]
        //This has to be replaced with enum
        public string[] cameras { get; set; }
    }
}
