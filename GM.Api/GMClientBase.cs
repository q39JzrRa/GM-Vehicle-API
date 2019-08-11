using GM.Api.Models;
using GM.Api.Tokens;
using JWT;
using JWT.Algorithms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GM.Api
{
    /// <summary>
    /// Base class API client for GM web services
    /// </summary>
    public abstract class GMClientBase
    {
        public static int RetryCount { get; set; } = 3;

        //TODO: consistent exception throwing

        string _clientId;
        string _deviceId;
        JwtTool _jwtTool;
        string _apiUrl;
        string _host;

        HttpClient _client;

        public bool IsUpgraded { get; private set; } = false;
        bool _isConnected = false;

        /// <summary>
        /// Contents of the received login token
        /// May be populated from a cached token
        /// Refreshed automatically
        /// </summary>
        public LoginData LoginData { get; set; } = null;

        /// <summary>
        /// Active vehicle configuration
        /// Must be populated to initiate commands against a car
        /// </summary>
        public Vehicle ActiveVehicle { get; set; }


        /// <summary>
        /// Callback called when LoginData is updated
        /// Intended to facilitate updating the stored token
        /// </summary>
        public Func<LoginData, Task> TokenUpdateCallback { get; set; }


        /// <summary>
        /// Create a new GMClient
        /// </summary>
        /// <param name="clientId">Client ID for authentication</param>
        /// <param name="deviceId">Device ID (should be in the format of a GUID)</param>
        /// <param name="clientSecret">Client Secret for authentication</param>
        /// <param name="apiUrl">Base url for the API. Usually https://api.gm.com/api </param>
        public GMClientBase(string clientId, string deviceId, string clientSecret, string apiUrl)
        {
            Setup(clientId, deviceId, clientSecret, apiUrl);
        }

        void Setup(string clientId, string deviceId, string clientSecret, string apiUrl)
        {
            _clientId = clientId;
            _deviceId = deviceId;
            _jwtTool = new JwtTool(clientSecret);
            _apiUrl = apiUrl;
            var uri = new Uri(_apiUrl);
            _host = uri.Host;
            _client = CreateClient(_host);
        }


        static HttpClient CreateClient(string host)
        {
            var client = new HttpClient(new HttpClientHandler() { AllowAutoRedirect = true, AutomaticDecompression = System.Net.DecompressionMethods.GZip });

            client.DefaultRequestHeaders.AcceptEncoding.SetValue("gzip");
            client.DefaultRequestHeaders.Accept.SetValue("application/json");
            client.DefaultRequestHeaders.AcceptLanguage.SetValue("en-US");
            client.DefaultRequestHeaders.UserAgent.ParseAdd("okhttp/3.9.0");
            client.DefaultRequestHeaders.Host = host;
            client.DefaultRequestHeaders.MaxForwards = 10;
            client.DefaultRequestHeaders.ExpectContinue = false;
            return client;
        }


        #region Client Helpers

        /// <summary>
        /// Helper wrapper for SendAsync that handles token updates and retries
        /// </summary>
        /// <param name="request"></param>
        /// <param name="noAuth"></param>
        /// <returns></returns>
        async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, bool noAuth = false)
        {
            if (!noAuth)
            {
                if (LoginData == null)
                {
                    throw new InvalidOperationException("Not Logged in");
                }
                if (LoginData.IsExpired)
                {
                    var result = await RefreshToken();
                    if (!result)
                    {
                        throw new InvalidOperationException("Token refresh failed");
                    }
                }
            }
            else
            {
                request.Headers.Authorization = null;
            }

            int attempt = 0;
            while (attempt < RetryCount)
            {
                attempt++;
                HttpResponseMessage resp = null;
                try
                {
                    resp = await _client.SendAsync(request);
                }
                catch (Exception ex)
                {
                    //todo: only catch transient errors
                    //todo: log this
                    continue;
                }

                if (!resp.IsSuccessStatusCode)
                {
                    if (resp.StatusCode == System.Net.HttpStatusCode.Unauthorized || resp.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    {
                        var result = await RefreshToken();
                        if (!result)
                        {
                            throw new InvalidOperationException("Token refresh failed");
                        }
                        continue;
                    }
                    else if (resp.StatusCode == System.Net.HttpStatusCode.BadGateway || resp.StatusCode == System.Net.HttpStatusCode.Conflict || resp.StatusCode == System.Net.HttpStatusCode.GatewayTimeout || resp.StatusCode == System.Net.HttpStatusCode.InternalServerError || resp.StatusCode == System.Net.HttpStatusCode.RequestTimeout || resp.StatusCode == System.Net.HttpStatusCode.ResetContent || resp.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
                    {
                        //possible transient errors
                        //todo: log this
                        await Task.Delay(500);
                        continue;
                    }
                    else
                    {
                        var respMessage = (await resp.Content.ReadAsStringAsync()) ?? "";
                        throw new InvalidOperationException("Request error. StatusCode: " + resp.StatusCode.ToString() + ", msg: " + respMessage);
                    }
                }
                else
                {
                    return resp;
                }
            }
            //todo: include more info
            throw new InvalidOperationException("Request failed too many times");
        }


        async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content, bool noAuth = false)
        {
            return await SendAsync(new HttpRequestMessage(HttpMethod.Post, requestUri) { Content = content }, noAuth);
        }

        async Task<HttpResponseMessage> GetAsync(string requestUri, bool noAuth = false)
        {
            return await SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri), noAuth);
        }

        #endregion


        /// <summary>
        /// Connect to vehicle. Must be called before issuing commands
        /// </summary>
        /// <param name="vin"></param>
        /// <returns></returns>
        async Task<CommandResponse> VehicleConnect()
        {
            if (ActiveVehicle == null) throw new InvalidOperationException("ActiveVehicle must be populated");
            using (var response = await PostAsync(ActiveVehicle.GetCommand("connect").Url, new StringContent("{}", Encoding.UTF8, "application/json")))
            {
                if (response.IsSuccessStatusCode)
                {
                    var respString = await response.Content.ReadAsStringAsync();
                    var respObj = JsonConvert.DeserializeObject<CommandRequestResponse>(respString);
                    if (respObj == null || respObj.CommandResponse == null) return null;
                    return respObj.CommandResponse;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return null;
                }
            }
        }

        /// <summary>
        /// Upgrade the token using OnStar PIN
        /// Allows the execution of privileged commands on the vehicle
        /// </summary>
        /// <param name="onStarPin">OnStar PIN</param>
        /// <returns>Success or not</returns>
        public async Task<bool> UpgradeLogin(string onStarPin)
        {
            var payload = new LoginRequest()
            {
                ClientId = _clientId,
                DeviceId = _deviceId,
                Credential = onStarPin,
                CredentialType = "PIN",
                Nonce = helpers.GenerateNonce(),
                Timestamp = DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK")
            };

            var token = _jwtTool.EncodeToken(payload);

            using (var response = await PostAsync($"{_apiUrl}/v1/oauth/token/upgrade", new StringContent(token, Encoding.UTF8, "text/plain")))
            {
                if (response.IsSuccessStatusCode)
                {
                    IsUpgraded = true;
                    return true;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return false;
                }
            }
        }


        /// <summary>
        /// Login to the API via Username and Password
        /// These credentials are not stored; only exchanged for a token
        /// The token is maintained by the client
        /// </summary>
        /// <param name="username">GM account username</param>
        /// <param name="password">GM Account password</param>
        /// <returns></returns>
        public async Task<bool> Login(string username, string password)
        {
            var payload = new LoginRequest()
            {
                ClientId = _clientId,
                DeviceId = _deviceId,
                GrantType = "password",
                Nonce = helpers.GenerateNonce(),
                Password = password,
                Scope = "onstar gmoc commerce user_trailer msso",
                Timestamp = DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK"),
                Username = username
            };

            var token = _jwtTool.EncodeToken(payload);

            using (var response = await PostAsync($"{_apiUrl}/v1/oauth/token", new StringContent(token, Encoding.UTF8, "text/plain"), true))
            {
                string rawResponseToken = null;

                if (response.IsSuccessStatusCode)
                {
                    rawResponseToken = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                }

                if (string.IsNullOrEmpty(rawResponseToken))
                {
                    return false;
                }

                var loginTokenData = _jwtTool.DecodeTokenToObject<LoginData>(rawResponseToken);

                LoginData = loginTokenData;
                _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", LoginData.AccessToken);

                //todo: should this be a copy rather than a reference?
                await TokenUpdateCallback?.Invoke(LoginData);
                return true;
            }
        }

        /// <summary>
        /// Manually refresh access token
        /// </summary>
        /// <returns>Success tru or false</returns>
        public async Task<bool> RefreshToken()
        {
            if (LoginData == null) return false;

            var payload = new LoginRequest()
            {
                ClientId = _clientId,
                DeviceId = _deviceId,
                GrantType = "urn:ietf:params:oauth:grant-type:jwt-bearer",
                Nonce = helpers.GenerateNonce(),
                Scope = "onstar gmoc commerce user_trailer",
                Timestamp = DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK"),
                Assertion = LoginData.IdToken
            };

            var token = _jwtTool.EncodeToken(payload);

            using (var response = await PostAsync($"{_apiUrl}/v1/oauth/token", new StringContent(token, Encoding.UTF8, "text/plain"), true))
            {

                string rawResponseToken = null;

                if (response.IsSuccessStatusCode)
                {
                    rawResponseToken = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                }

                if (string.IsNullOrEmpty(rawResponseToken))
                {
                    return false;
                }


                /*{
      "access_token": ,
      "token_type": "Bearer",
      "expires_in": 1800,
      "scope": "user_trailer onstar commerce  gmoc role_owner",
      "user_info": {
        "RemoteUserId": "",
        "country": ""
      }
    }*/
                // Not sure if the scope needs to be updated, as msso has been removed in the refresh request

                var refreshData = _jwtTool.DecodeTokenToObject<LoginData>(rawResponseToken);

                LoginData.AccessToken = refreshData.AccessToken;
                LoginData.IssuedAtUtc = refreshData.IssuedAtUtc;
                LoginData.ExpiresIn = refreshData.ExpiresIn;

                //should we assume the upgrade status is broken?


                _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", LoginData.AccessToken);

                //todo: should this be a copy rather than a reference?
                await TokenUpdateCallback?.Invoke(LoginData);

                return true;
            }
        }



        #region Commands


        /// <summary>
        /// Submit the initial call for a command
        /// NOTE: this will be changing to use the URLs defined in vehicle configuration
        /// </summary>
        /// <param name="vin">Vehicle VIN</param>
        /// <param name="pin">OnStar PIN</param>
        /// <param name="command">command name</param>
        /// <returns></returns>
        async Task<CommandResponse> InitiateCommand(string command, JObject requestParameters)
        {
            if (ActiveVehicle == null) throw new InvalidOperationException("ActiveVehicle must be populated");

            var cmdInfo = ActiveVehicle.GetCommand(command);

            if (cmdInfo == null) throw new InvalidOperationException("Unsupported command");

            if (cmdInfo.IsPrivSessionRequired.GetValueOrDefault())
            {
                if (!IsUpgraded)
                {
                    //TODO: need to determine how long an upgrade lasts - do we reset it when refreshing the token?
                    // Also if the android app saves the PIN, should we save the PIN?
                    throw new InvalidOperationException("Command requires upgraded login");
                }
            }

            if (!_isConnected)
            {
                await VehicleConnect();
                _isConnected = true;
            }


            JObject reqObj = new JObject();

            if (requestParameters != null)
            {
                reqObj[$"{command}Request"] = requestParameters;
            }



            using (var response = await PostAsync(cmdInfo.Url, new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(reqObj), Encoding.UTF8, "application/json")))
            {
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    //todo: is this needed with the fancy post?
                    return null;
                }

                var commandResult = await response.Content.ReadAsAsync<CommandRequestResponse>();

                return commandResult.CommandResponse;
            }
        }


        /// <summary>
        /// Periodically poll the status of a command, only returning after it succeeds or fails
        /// </summary>
        /// <param name="statusUrl">statusUrl returned when the command was initiated</param>
        /// <returns>Response from final poll</returns>
        async Task<CommandResponse> WaitForCommandCompletion(string statusUrl)
        {
            int nullResponseCount = 0;

            while (true)
            {
                await Task.Delay(5000);
                var result = await PollCommandStatus(statusUrl);
                if (result == null)
                {
                    nullResponseCount++;
                    if (nullResponseCount > 5) return null;
                }
                if ("inProgress".Equals(result.Status, StringComparison.OrdinalIgnoreCase)) continue;
                return result;
            }
        }


        protected async Task<CommandResponse> InitiateCommandAndWait(string command, JObject requestParameters)
        {
            var result = await InitiateCommand(command, requestParameters);
            var endStatus = await WaitForCommandCompletion(result.Url);
            return endStatus;
        }

        protected async Task<bool> InitiateCommandAndWaitForSuccess(string command, JObject requestParameters)
        {
            var result = await InitiateCommandAndWait(command, requestParameters);
            if (result == null) return false;
            if ("success".Equals(result.Status, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        async Task<CommandResponse> PollCommandStatus(string statusUrl)
        {
            var response = await GetAsync($"{statusUrl}?units=METRIC");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsAsync<CommandRequestResponse>();
                return result.CommandResponse;
            }
            else
            {
                return null;
            }
        }


        public async Task<IEnumerable<Vehicle>> GetVehicles()
        {
            //these could be parameterized, but we better stick with what the app does
            var resp = await GetAsync($"{_apiUrl}/v1/account/vehicles?offset=0&limit=10&includeCommands=true&includeEntitlements=true&includeModules=true");

            if (resp.IsSuccessStatusCode)
            {
                var outerResult = await resp.Content.ReadAsAsync<VehiclesResponse>();
                if (outerResult.Vehicles != null && outerResult.Vehicles.Vehicle != null && outerResult.Vehicles.Vehicle.Length > 0)
                {
                    return outerResult.Vehicles.Vehicle;
                }
            }

            return null;
        }





        #endregion
    }
}
