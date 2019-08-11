using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GM.Api.Tokens
{
    /// <summary>
    /// Data contained within the login response JWT or as updated when refreshed
    /// </summary>
    public class LoginData
    {
        /// <summary>
        /// Access token used as bearer token
        /// </summary>
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// Bearer
        /// </summary>
        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        /// <summary>
        /// Token expiration in seconds
        /// (Note: I have seen the tokens expire quicker)
        /// </summary>
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        /// <summary>
        /// List of scopes
        /// </summary>
        [JsonProperty("scope")]
        public string Scope { get; set; }

        /// <summary>
        /// Information about the OnStar account
        /// </summary>
        [JsonProperty("onstar_account_info")]
        public Onstar_Account_Info OnStarAccountInfo { get; set; }

        /// <summary>
        /// Information about the user
        /// </summary>
        [JsonProperty("user_info")]
        public User_Info UserInfo { get; set; }

        /// <summary>
        /// RSA hashed itentity token (JWT) used in refresh assertion
        /// </summary>
        [JsonProperty("id_token")]
        public string IdToken { get; set; }

        /// <summary>
        /// Timestamp the token was recieved
        /// </summary>
        [JsonIgnore]
        public DateTime IssuedAtUtc { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Approximate timestamp the token expires
        /// </summary>
        [JsonIgnore]
        public DateTime ExpiresAtUtc => (IssuedAtUtc + TimeSpan.FromSeconds(ExpiresIn - 2));

        /// <summary>
        /// Check if the token is expired based on timestamp
        /// </summary>
        [JsonIgnore]
        public bool IsExpired => (DateTime.UtcNow >= ExpiresAtUtc);


    }

    public class Onstar_Account_Info
    {
        [JsonProperty("country_code")]
        public string CountryCode { get; set; }

        [JsonProperty("account_no")]
        public string AccountNo { get; set; }
    }

    public class User_Info
    {
        [JsonProperty("RemoteUserId")]
        public string RemoteUserId { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }
    }

}
