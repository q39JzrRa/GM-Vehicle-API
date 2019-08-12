using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GM.Api.Models
{

    public class TbtDestination
    {
        [JsonProperty("additionalDestinationInfo")]
        public AdditionalDestinationInfo AdditionalDestinationInfo { get; set; }

        [JsonProperty("destinationLocation")]
        public Location DestinationLocation { get; set; }
    }

    public class AdditionalDestinationInfo
    {
        [JsonProperty("destinationAddress")]
        public DestinationAddress DestinationAddress { get; set; }

        [JsonProperty("destinationType")]
        public string DestinationType { get; set; }
    }

    public class DestinationAddress
    {
        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("street")]
        public string Street { get; set; }

        [JsonProperty("streetNo")]
        public string StreetNo { get; set; }

        [JsonProperty("zipCode")]
        public string ZipCode { get; set; }
    }

    public class DestinationLocation
    {
        public void SetLatitude(float value)
        {
            Latitude = value.ToString("###.#####");
        }

        public void SetLongitude(float value)
        {
            Longitude = value.ToString("###.#####");
        }

        [JsonProperty("lat")]
        public string Latitude { get; set; }

        [JsonProperty("long")]
        public string Longitude { get; set; }
    }


}
