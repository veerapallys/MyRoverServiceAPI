using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace MyRoverServiceAPI
{
    public class MarsRoverPhotos
    {
        [JsonProperty("photos")]
        public RoverPhotos[] Photos { get; set; }
    }
    public class RoverPhotos
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("sol")]
        public long Sol { get; set; }

        [JsonProperty("camera")]
        public Camera Camera { get; set; }

        [JsonProperty("img_src")]
        public string ImageSourceUrl { get; set; }

        [JsonProperty("earth_date")]
        public DateTime EarthDate { get; set; }
        
        [JsonProperty("rover")]
        public Rover Rover { get; set; }
        
      }

    public class Rover
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("landing_date")]
        public DateTime LandingDate { get; set; }
        
        [JsonProperty("launch_date")]
        public DateTime LaunchingDate { get; set; }
        
        [JsonProperty("status")]
        public string Status { get; set; }
    }

    public class Camera
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("rover_id")]
        public long RoverId { get; set; }
        
        [JsonProperty("full_name")]
        public string FullName { get; set; }
    }
}
