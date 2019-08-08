using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GM.Api
{

    public class LoginData
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string scope { get; set; }
        public Onstar_Account_Info onstar_account_info { get; set; }
        public User_Info user_info { get; set; }
        public string id_token { get; set; }

        [JsonIgnore]
        public DateTime IssuedUtc { get; set; } = DateTime.UtcNow;
        [JsonIgnore]
        public DateTime ExpiresAtUtc => (IssuedUtc + TimeSpan.FromSeconds(expires_in));


    }

    public class Onstar_Account_Info
    {
        public string country_code { get; set; }
        public string account_no { get; set; }
    }

    public class User_Info
    {
        public string RemoteUserId { get; set; }
        public string country { get; set; }
    }

}
