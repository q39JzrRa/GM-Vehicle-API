using GM.Api.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GM.Api
{
    /// <summary>
    /// Generic implementation of GM Client supporting a limited set of commands or manually defined commands
    /// </summary>
    public class GenericGMClient : GMClientBase
    {
        public GenericGMClient(string clientId, string deviceId, string clientSecret, string apiUrl) : base(clientId, deviceId, clientSecret, apiUrl)
        {
        }


        public async Task<Diagnosticresponse[]> GetDiagnostics()
        {
            var cmdInfo = ActiveVehicle.GetCommand("diagnostics");

            var reqObj = new JObject()
            {
                ["diagnosticItem"] = new JArray(cmdInfo.CommandData.SupportedDiagnostics.SupportedDiagnostic)
            };

            var result = await InitiateCommandAndWait("diagnostics", reqObj);
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



        public async Task<Commandresponse> IssueCommand(string commandName, JObject parameters = null)
        {
            return await InitiateCommandAndWait(commandName, parameters);
        }


        public async Task<bool> LockDoor(string pin)
        {
            var reqObj = new JObject()
            {
                ["delay"] = 0
            };


            return await InitiateCommandAndWaitForSuccess("lockDoor", reqObj);
        }

        public async Task<bool> UnlockDoor(string pin)
        {

            var reqObj = new JObject()
            {
                ["delay"] = 0
            };

            return await InitiateCommandAndWaitForSuccess("unlockDoor", reqObj);
        }

        public async Task<bool> Start(string pin)
        {
            return await InitiateCommandAndWaitForSuccess("start", null);
        }

        public async Task<bool> CancelStart(string pin)
        {
            return await InitiateCommandAndWaitForSuccess("cancelStart", null);
        }


        public async Task<bool> Alert(string pin)
        {
            var reqObj = new JObject()
            {
                ["action"] = new JArray() { "Honk", "Flash" },
                ["delay"] = 0,
                ["duration"] = 1,
                ["override"] = new JArray() { "DoorOpen", "IgnitionOn" }
            };


            return await InitiateCommandAndWaitForSuccess("alert", reqObj);
        }


        public async Task<bool> CancelAlert(string pin)
        {
            return await InitiateCommandAndWaitForSuccess("cancelAlert", null);
        }




    }
}
