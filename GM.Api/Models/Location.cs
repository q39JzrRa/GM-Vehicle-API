using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GM.Api.Models
{
    public class Location
    {
        [JsonProperty("lat")]
        public float? Latitude { get; set; }

        [JsonProperty("long")]
        public float? Longitude { get; set; }
    }
}
