using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GM.Api.Tokens;

namespace GM.Api
{
    /// <summary>
    /// GM Client implementation to be used when you have local access to the Client ID and Client Secret
    /// </summary>
    public class GMClient : GMClientBase
    {
        string _clientId;
        JwtTool _jwtTool;


        public GMClient(string deviceId, Brand brand, string clientId, string clientSecret) : base(deviceId, brand)
        {
            _clientId = clientId;
            _jwtTool = new JwtTool(clientSecret);
        }

        protected override async Task<string> EncodeLoginRequest(LoginRequest request)
        {
            request.ClientId = _clientId;
            return _jwtTool.EncodeToken(request);
        }
    }
}
