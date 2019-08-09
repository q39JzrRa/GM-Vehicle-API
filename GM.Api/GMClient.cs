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
    public class GMClient
    {
        public static int RetryCount { get; set; } = 3;

        //TODO: consistent exception throwing

        string _clientId;
        string _deviceId;
        JwtTool _jwtTool;
        string _apiUrl;
        string _host;

        HttpClient _client;

        bool _isUpgraded = false;
        bool _isConnected = false;

        public LoginData LoginData { get; set; } = null;

        public Func<LoginData, Task> TokenUpdateCallback { get; set; }
        

        public GMClient(GmConfiguration config, string brand, string deviceId)
        {
            throw new NotImplementedException();
        }

        public GMClient(string clientId, string deviceId, string clientSecret, string apiUrl)
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
                        var respMessage = (await resp.Content.ReadAsStringAsync())??"";
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




        async Task<Commandresponse> VehicleConnect(string vin)
        {
            var response = await PostAsync($"{_apiUrl}/v1/account/vehicles/{vin}/commands/connect", new StringContent("{}", Encoding.UTF8, "application/json"));

            
            if (response.IsSuccessStatusCode)
            {
                var respString = await response.Content.ReadAsStringAsync();
                var respObj = JsonConvert.DeserializeObject<CommandResponseRoot>(respString);
                if (respObj == null || respObj.commandResponse == null) return null;
                return respObj.commandResponse;
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                return null;
            }

        }


        async Task<bool> UpgradeToken(string pin)
        {
            var payload = new LoginRequest()
            {
                ClientId = _clientId,
                DeviceId = _deviceId,
                Credential = pin,
                CredentialType = "PIN",
                Nonce = helpers.GenerateNonce(),
                Timestamp = DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK")
            };

            var token = _jwtTool.EncodeToken(payload);

            var response = await PostAsync($"{_apiUrl}/v1/oauth/token/upgrade", new StringContent(token, Encoding.UTF8, "text/plain"));

            if (response.IsSuccessStatusCode)
            {
                _isUpgraded = true;
                return true;
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                return false;
            }
        }


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

            var response = await PostAsync($"{_apiUrl}/v1/oauth/token", new StringContent(token, Encoding.UTF8, "text/plain"), true);


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

            var response = await PostAsync($"{_apiUrl}/v1/oauth/token", new StringContent(token, Encoding.UTF8, "text/plain"), true);

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




        #region Commands



        public async Task<Commandresponse> InitiateCommand(string vin, string pin, string command)
        {
            if (!_isConnected)
            {
                await VehicleConnect(vin);
                _isConnected = true;
            }

            await Task.Delay(500);

            if (!_isUpgraded)
            {
                if (!await UpgradeToken(pin)) return null;
            }


            //


            JObject reqObj;

            if (command == "lockDoor" || command == "unlockDoor")
            {
                reqObj = new JObject()
                {
                    [$"{command}Request"] = new JObject()
                    {
                        ["delay"] = 0
                    }
                };
            }
            else if (command == "alert")
            {
                reqObj = new JObject()
                {
                    //TODO: these parameters may be controllable :D
                    [$"{command}Request"] = new JObject()
                    {
                        ["action"] = new JArray() { "Honk", "Flash" },
                        ["delay"] = 0,
                        ["duration"] = 1,
                        ["override"] = new JArray() { "DoorOpen", "IgnitionOn" }
                    }
                };
            }
            else if (command == "diagnostics")
            {
                reqObj = new JObject()
                {
                    [$"{command}Request"] = new JObject()
                    {
                        ["diagnosticItem"] = new JArray(DiagnosticRequestRoot.DefaultItems)
                    }
                };
            }
            else
            {
                reqObj = new JObject();
            }

            var response = await PostAsync($"{_apiUrl}/v1/account/vehicles/{vin}/commands/{command}", new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(reqObj), Encoding.UTF8, "application/json"));


            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return null;
            }

            var commandResult = await response.Content.ReadAsAsync<CommandResponseRoot>();

            return commandResult.commandResponse;
        }


        public async Task<Commandresponse> WaitForCommandCompletion(string statusUrl)
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
                if ("inProgress".Equals(result.status, StringComparison.OrdinalIgnoreCase)) continue;
                return result;
            }
        }


        async Task<Commandresponse> InitiateCommandAndWait(string vin, string pin, string command)
        {
            var result = await InitiateCommand(vin, pin, command);
            var endStatus = await WaitForCommandCompletion(result.url);
            return endStatus;
        }

        async Task<bool> InitiateCommandAndWaitForSuccess(string vin, string pin, string command)
        {
            var result = await InitiateCommandAndWait(vin, pin, command);
            if (result == null) return false;
            if ("success".Equals(result.status, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        async Task<Commandresponse> PollCommandStatus(string statusUrl)
        {
            var response = await GetAsync($"{statusUrl}?units=METRIC");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsAsync<CommandResponseRoot>();
                return result.commandResponse;
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
                if (outerResult.vehicles != null && outerResult.vehicles.vehicle != null && outerResult.vehicles.vehicle.Length > 0)
                {
                    return outerResult.vehicles.vehicle;
                }
            }

            return null;
        }



        public async Task<Diagnosticresponse[]> GetDiagnostics(string vin, string pin)
        {
            var result = await InitiateCommandAndWait(vin, pin, "diagnostics");
            if (result == null) return null;
            if ("success".Equals(result.status, StringComparison.OrdinalIgnoreCase))
            {
                return result.body.diagnosticResponse;
            }
            else
            {
                return null;
            }



        }



        public async Task<bool> LockDoor(string vin, string pin)
        {
            return await InitiateCommandAndWaitForSuccess(vin, pin, "lockDoor");
        }

        public async Task<bool> UnlockDoor(string vin, string pin)
        {
            return await InitiateCommandAndWaitForSuccess(vin, pin, "unlockDoor");
        }

        public async Task<bool> Start(string vin, string pin)
        {
            return await InitiateCommandAndWaitForSuccess(vin, pin, "start");
        }

        public async Task<bool> CancelStart(string vin, string pin)
        {
            return await InitiateCommandAndWaitForSuccess(vin, pin, "cancelStart");
        }


        public async Task<bool> Alert(string vin, string pin)
        {
            return await InitiateCommandAndWaitForSuccess(vin, pin, "alert");
        }


        public async Task<bool> CancelAlert(string vin, string pin)
        {
            return await InitiateCommandAndWaitForSuccess(vin, pin, "cancelAlert");
        }



        #endregion
    }
}
