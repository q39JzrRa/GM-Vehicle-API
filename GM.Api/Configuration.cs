using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GM.Api
{

    public class GmConfiguration
    {
        public Dictionary<string, BrandClientInfo> brand_client_info { get; set; }
        public ApiConfig[] configs { get; set; }
        public Telenav_Config telenav_config { get; set; }
        public string equip_key { get; set; }
        public string key_store_password { get; set; }
        public string key_password { get; set; }
        public Dictionary<string, RegionCert> certs { get; set; }
    }

    public class BrandClientInfo
    {
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string debug_client_id { get; set; }
        public string debug_client_secret { get; set; }
    }


    public class ApiConfig
    {
        public string name { get; set; }
        public string url { get; set; }
        public string required_client_scope { get; set; }
        public string optional_client_scope { get; set; }
        /// <summary>
        /// do not use this
        /// </summary>
        public string client_id { get; set; }
        /// <summary>
        /// do not use this
        /// </summary>
        public string client_secret { get; set; }
    }



    public class Telenav_Config
    {
        public string name { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
    }

    public class RegionCert
    {
        public string pattern { get; set; }
        public string[] certificate_pins { get; set; }
    }
}
