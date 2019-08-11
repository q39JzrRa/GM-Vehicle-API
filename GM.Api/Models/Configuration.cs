using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GM.Api.Models
{
    /// <summary>
    /// Model of the encrypted Andorid configuration file
    /// </summary>

    public class GmConfiguration
    {
        /// <summary>
        /// Client Credentials by GM Brand
        /// </summary>
        [JsonProperty("brand_client_info")]
        public Dictionary<string, BrandClientInfo> BrandClientInfo { get; set; }

        /// <summary>
        /// Endpoint Configuration collection
        /// </summary>
        [JsonProperty("configs")]
        public ApiConfig[] Configs { get; set; }

        /// <summary>
        /// Presumably configuration used for navigation
        /// </summary>
        [JsonProperty("telenav_config")]
        public TelenavConfig TelenavConfig { get; set; }

        /// <summary>
        /// Unknown
        /// </summary>
        [JsonProperty("equip_key")]
        public string EquipKey { get; set; }

        /// <summary>
        /// Probably the key used to encrypt the saved OnStar PINs
        /// </summary>
        [JsonProperty("key_store_password")]
        public string KeyStorePassword { get; set; }

        /// <summary>
        /// Unknown
        /// </summary>
        [JsonProperty("key_password")]
        public string KeyPassword { get; set; }

        /// <summary>
        /// Certificate pinning information used to prevent SSL spoofing
        /// </summary>
        [JsonProperty("certs")]
        public Dictionary<string, RegionCert> Certs { get; set; }
    }

    /// <summary>
    /// Client Credentials for a given GM brand
    /// </summary>
    public class BrandClientInfo
    {
        /// <summary>
        /// OAuth Client ID
        /// </summary>
        [JsonProperty("client_id")]
        public string ClientId { get; set; }

        /// <summary>
        /// OAuth Client Secret
        /// </summary>
        [JsonProperty("client_secret")]
        public string ClientSecret { get; set; }

        /// <summary>
        /// Debug environment Oauth Client ID
        /// </summary>
        [JsonProperty("debug_client_id")]
        public string DebugClientId { get; set; }

        /// <summary>
        /// Debug environment Oauth Client Secret
        /// </summary>
        [JsonProperty("debug_client_secret")]
        public string DebugClientSecret { get; set; }
    }

    /// <summary>
    /// API configuration for a given GM brand
    /// </summary>
    public class ApiConfig
    {
        /// <summary>
        /// GM Brand name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Base API endpoint URL (eg "https://api.gm.com/api")
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        /// Space separated scopes required for login
        /// </summary>
        [JsonProperty("required_client_scope")]
        public string RequiredClientScope { get; set; }

        /// <summary>
        /// Space separated scopes optional for login
        /// </summary>
        [JsonProperty("optional_client_scope")]
        public string OptionalClientScope { get; set; }

        /// <summary>
        /// Use the Brand config Client ID instead
        /// </summary>
        [JsonProperty("client_id")]
        public string ClientId { get; set; }

        /// <summary>
        /// Use the Brand config Client Secret instead
        /// </summary>
        [JsonProperty("client_secret")]
        public string ClientSecret { get; set; }
    }


    /// <summary>
    /// Client credentials for Telenav system
    /// </summary>
    public class TelenavConfig
    {
        /// <summary>
        /// Name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// OAuth Client ID
        /// </summary>
        [JsonProperty("client_id")]
        public string ClientId { get; set; }

        /// <summary>
        /// OAuth Client Secret
        /// </summary>
        [JsonProperty("client_secret")]
        public string ClientSecret { get; set; }
    }

    /// <summary>
    /// Container for certificate pinning info
    /// </summary>
    public class RegionCert
    {
        /// <summary>
        /// Pattern used by the expected certificate. Should match the CN I'm guessing
        /// </summary>
        [JsonProperty("pattern")]
        public string Pattern { get; set; }

        /// <summary>
        /// A list of certificate pins. There are usually 3 and only one matches the actual SSL cert of the server
        /// The other two do not match the intermediate / root certs - not sure what they are
        /// </summary>
        [JsonProperty("certificate_pins")]
        public string[] CertificatePins { get; set; }
    }
}
