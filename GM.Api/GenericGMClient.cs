using GM.Api.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GM.Api
{
    /// <summary>
    /// Generic implementation of GM Client supporting a limited set of commands
    /// </summary>
    public class GenericGMClient : GMClientBase
    {
        public GenericGMClient(string clientId, string deviceId, string clientSecret, string apiUrl) : base(clientId, deviceId, clientSecret, apiUrl)
        {
        }

        /// <summary>
        /// Retrieve Diagnostic data for the active vehicle
        /// </summary>
        /// <returns></returns>
        public async Task<DiagnosticResponse[]> GetDiagnostics()
        {
            var cmdInfo = ActiveVehicle.GetCommand("diagnostics");

            var reqObj = new JObject()
            {
                ["diagnosticItem"] = new JArray(cmdInfo.CommandData.SupportedDiagnostics.SupportedDiagnostic)
            };

            var result = await InitiateCommandAndWait("diagnostics", reqObj);
            if (result == null) return null;
            if ("success".Equals(result.Status, StringComparison.OrdinalIgnoreCase))
            {
                return result.Body.DiagnosticResponse;
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// Issue an arbitrary command
        /// </summary>
        /// <param name="commandName">Name of the command. Must exists in the vehicle's configuration</param>
        /// <param name="parameters">JSON parameters for the command</param>
        /// <returns></returns>
        public async Task<CommandResponse> IssueCommand(string commandName, JObject parameters = null)
        {
            return await InitiateCommandAndWait(commandName, parameters);
        }

        /// <summary>
        /// Lock the active vehicles's doors and wait for completion
        /// Privileged Command
        /// </summary>
        /// <returns>True or false for success</returns>
        public async Task<bool> LockDoor()
        {

            var reqObj = new JObject()
            {
                ["lockDoorRequest"] = new JObject()
                {
                    ["delay"] = 0
                }
            };

            return await InitiateCommandAndWaitForSuccess("lockDoor", reqObj);
        }


        /// <summary>
        /// Fails when the hotspot is off...
        /// Note: the app uses diagnotics that also fail when the hotpot is off
        /// </summary>
        /// <returns></returns>
        public async Task<HotspotInfo> GetHotspotInfo()
        {
            var resp = await InitiateCommandAndWait("getHotspotInfo", null);
            return resp.Body.HotspotInfo;
        }


        /// <summary>
        /// Send a turn-by-turn destination to the vehicle
        /// Requires both coordinates and address info
        /// Vehicle may not respond if turned off or may take a very long time to respond
        /// </summary>
        /// <param name="destination"></param>
        /// <returns></returns>
        public async Task<bool> SendTBTRoute(TbtDestination destination)
        {
            var reqObj = new JObject()
            {
                ["tbtDestination"] = new JObject(destination)
            };

            return await InitiateCommandAndWaitForSuccess("sendTBTRoute", reqObj);
        }


        /// <summary>
        /// Unlock the active vehicles's doors and wait for completion
        /// Privileged Command
        /// </summary>
        /// <returns>True or false for success</returns>
        public async Task<bool> UnlockDoor()
        {
            var reqObj = new JObject()
            {
                ["unlockDoorRequest"] = new JObject()
                {
                    ["delay"] = 0
                }
            };

            return await InitiateCommandAndWaitForSuccess("unlockDoor", reqObj);
        }

        /// <summary>
        /// Remote start the active vehicle and wait for completion
        /// Privileged Command
        /// </summary>
        /// <returns>True or false for success</returns>
        public async Task<bool> Start()
        {
            return await InitiateCommandAndWaitForSuccess("start", null);
        }

        /// <summary>
        /// Remote stop the active vehicle and wait for completion
        /// Privileged Command
        /// </summary>
        /// <returns>True or false for success</returns>
        public async Task<bool> CancelStart()
        {
            return await InitiateCommandAndWaitForSuccess("cancelStart", null);
        }


        /// <summary>
        /// Set off remote alarm on the active vehicle and wait for completion
        /// Privileged Command
        /// </summary>
        /// <returns>True or false for success</returns>
        public async Task<bool> Alert()
        {


            var reqObj = new JObject()
            {
                ["alertRequest"] = new JObject()
                {
                    ["action"] = new JArray() { "Honk", "Flash" },
                    ["delay"] = 0,
                    ["duration"] = 1,
                    ["override"] = new JArray() { "DoorOpen", "IgnitionOn" }
                }
            };

            return await InitiateCommandAndWaitForSuccess("alert", reqObj);
        }

        /// <summary>
        /// Stop remote alarm on the active vehicle and wait for completion
        /// Privileged Command
        /// </summary>
        /// <returns>True or false for success</returns>
        public async Task<bool> CancelAlert()
        {
            return await InitiateCommandAndWaitForSuccess("cancelAlert", null);
        }




    }
}
