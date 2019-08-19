using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using GM.Api.Tokens;

namespace GM.Api
{
    /// <summary>
    /// GM Client implementation that uses a web service to sign the JWT tokens required for authentication
    /// Use this if you do not have access to the Client ID and Client Secret
    /// </summary>
    public class GMClientNoKey : GMClientBase
    {
        string _tokenSignUrl;
        HttpClient _tokenClient = new HttpClient();

        /// <summary>
        /// Create new GM Client
        /// </summary>
        /// <param name="deviceId">deviceId = string representation of a GUID</param>
        /// <param name="brand">API is segmented by brand</param>
        /// <param name="tokenSignUrl">URL for webservice that will sign JWT tokens (e.g. "https://gmsigner.herokuapp.com/")</param>
        public GMClientNoKey(string deviceId, Brand brand, string tokenSignUrl) : base(deviceId, brand)
        {
            _tokenSignUrl = tokenSignUrl;
        }

        protected override async Task<string> EncodeLoginRequest(LoginRequest request)
        {
            var resp = await _tokenClient.PostAsJsonAsync($"{_tokenSignUrl}?brand={_brand.GetName()}", request);

            if (resp.IsSuccessStatusCode)
            {
                return await resp.Content.ReadAsStringAsync();
            }
            else
            {
                string errorText = await resp.Content.ReadAsStringAsync();
                throw new InvalidOperationException("Token sign failure: " + errorText);
            }
        }
    }
}
