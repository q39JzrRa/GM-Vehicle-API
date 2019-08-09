using JWT;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GM.Api.Tokens
{


    public class LoginRequest
    {
        [JsonProperty("client_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string ClientId { get; set; }

        [JsonProperty("device_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string DeviceId { get; set; }

        [JsonProperty("grant_type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string GrantType { get; set; }

        [JsonProperty("nonce", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Nonce { get; set; }

        [JsonProperty("password", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Password { get; set; }

        /// <summary>
        /// Scope
        /// ex: onstar gmoc commerce user_trailer msso
        /// </summary>
        [JsonProperty("scope", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Scope { get; set; }

        /// <summary>
        /// Current timestamp in UTC using "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK" format string
        /// </summary>
        [JsonProperty("timestamp", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Timestamp { get; set; }

        [JsonProperty("username", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Username { get; set; }


        /// <summary>
        /// OnStar PIN used to upgrade token
        /// </summary>
        [JsonProperty("credential", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Credential { get; set; }

        /// <summary>
        /// "PIN" for onstanr pin
        /// </summary>
        [JsonProperty("credential_type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string CredentialType { get; set; }

        /// <summary>
        /// IdToken from login payload - used for refreshing
        /// </summary>
        [JsonProperty("assertion", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Assertion { get; set; }


    }





}
