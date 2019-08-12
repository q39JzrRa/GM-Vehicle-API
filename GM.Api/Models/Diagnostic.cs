using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Newtonsoft.Json;

namespace GM.Api.Models
{
    public class DiagnosticResponse
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("diagnosticElement")]
        public DiagnosticElement[] DiagnosticElement { get; set; }
    }

    public class DiagnosticElement
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("unit")]
        public string Unit { get; set; }
    }

}
