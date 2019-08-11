using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Newtonsoft.Json;

namespace GM.Api.Models
{
    /// <summary>
    /// Response Body
    /// Note: this only contains a diagnostic response. there are likely others.
    /// </summary>
    public class ResponseBody
    {
        [JsonProperty("diagnosticResponse")]
        public DiagnosticResponse[] DiagnosticResponse { get; set; }
    }

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
