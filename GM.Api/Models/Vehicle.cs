using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GM.Api.Models
{


    public class VehiclesResponse
    {
        [JsonProperty("vehicles")]
        public Vehicles Vehicles { get; set; }
    }

    public class Vehicles
    {
        /// <summary>
        /// Size of the Vehicle array, or full size. One would need to have more than 10 cars to find out...
        /// </summary>
        [JsonProperty("size")]
        public string Size { get; set; }

        /// <summary>
        /// List of vehicles associated with the account
        /// Note that there is paging and by default the page size is 10
        /// </summary>
        [JsonProperty("vehicle")]
        public Vehicle[] Vehicle { get; set; }
    }

    /// <summary>
    /// Vehicle description
    /// </summary>
    public class Vehicle
    {
        /// <summary>
        /// Vehicle VIN
        /// </summary>
        [JsonProperty("vin")]
        public string Vin { get; set; }

        /// <summary>
        /// Vehicle Make
        /// </summary>
        [JsonProperty("make")]
        public string Make { get; set; }

        /// <summary>
        /// Vehicle Model
        /// </summary>
        [JsonProperty("model")]
        public string Model { get; set; }

        /// <summary>
        /// Vehicle Year
        /// </summary>
        [JsonProperty("year")]
        public string Year { get; set; }

        /// <summary>
        /// Vehicle Manufacturer - not sure why this is required...
        /// </summary>
        [JsonProperty("manufacturer")]
        public string Manufacturer { get; set; }

        /// <summary>
        /// (e.g. car, maybe truck)
        /// </summary>
        [JsonProperty("bodyStyle")]
        public string BodyStyle { get; set; }

        /// <summary>
        /// Vehicle cellular / OnStar phone number
        /// </summary>
        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("unitType")]
        public string UnitType { get; set; }

        [JsonProperty("onstarStatus")]
        public string OnStarStatus { get; set; }

        /// <summary>
        /// Base URL for API calls regarding this vehicle
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("isInPreActivation")]
        public bool? IsInPreActivation { get; set; }

        [JsonProperty("insuranceInfo")]
        public InsuranceInfo InsuranceInfo { get; set; }

        [JsonProperty("enrolledInContinuousCoverage")]
        public bool? EnrolledInContinuousCoverage { get; set; }

        /// <summary>
        /// Details on supported commands
        /// </summary>
        [JsonProperty("commands")]
        public Commands Commands { get; set; }

        /// <summary>
        /// Details on available modules
        /// </summary>
        [JsonProperty("modules")]
        public Modules Modules { get; set; }

        /// <summary>
        /// Details on available entitlements
        /// </summary>
        [JsonProperty("entitlements")]
        public Entitlements Entitlements { get; set; }

        [JsonProperty("propulsionType")]
        public string PropulsionType { get; set; }


        public Command GetCommand(string name)
        {
            return (from f in Commands.Command where f.Name.Equals(name, StringComparison.Ordinal) select f).FirstOrDefault();
        }

    }

    public class InsuranceInfo
    {
        [JsonProperty("insuranceAgent")]
        public InsuranceAgent InsuranceAgent { get; set; }
    }

    public class InsuranceAgent
    {
        [JsonProperty("phone")]
        public Phone Phone { get; set; }
    }

    public class Phone
    {
    }

    public class Commands
    {
        /// <summary>
        /// List of commands supported by the vehicle
        /// </summary>
        [JsonProperty("command")]
        public Command[] Command { get; set; }
    }

    /// <summary>
    /// Details about a supported command
    /// </summary>
    public class Command
    {
        /// <summary>
        /// Command name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Description of what the command does
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// API URL to be used for issuing the command
        /// This SDK uses this url rather than constructing it
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        /// True or False if the command requires the token to be upgraded with an OnStar PIN
        /// </summary>
        [JsonProperty("isPrivSessionRequired")]
        public bool? IsPrivSessionRequired { get; set; }

        /// <summary>
        /// For commands with additional data such as diagnostics
        /// </summary>
        [JsonProperty("commandData")]
        public CommandData CommandData { get; set; }
    }

    public class CommandData
    {
        [JsonProperty("supportedDiagnostics")]
        public SupportedDiagnostics SupportedDiagnostics { get; set; }
    }

    public class SupportedDiagnostics
    {
        /// <summary>
        /// List of the diagnostic elements that may be requsted for the vehicle
        /// </summary>
        [JsonProperty("supportedDiagnostic")]
        public string[] SupportedDiagnostic { get; set; }
    }

    public class Modules
    {
        /// <summary>
        /// List of modules - not much here
        /// </summary>
        [JsonProperty("module")]
        public Module[] Module { get; set; }
    }

    public class Module
    {
        [JsonProperty("moduleType")]
        public string ModuleType { get; set; }

        [JsonProperty("moduleCapability")]
        public string ModuleCapability { get; set; }
    }

    public class Entitlements
    {
        /// <summary>
        /// List of entitlements - features and activities vehicles are capable of
        /// List contains things the vehicle or account may or may not support
        /// Check the Elligible flag
        /// </summary>
        [JsonProperty("entitlement")]
        public Entitlement[] Entitlement { get; set; }
    }

    /// <summary>
    /// Details about an Entitlement
    /// </summary>
    public class Entitlement
    {
        /// <summary>
        /// ID or name of entitlement
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// True or false if the entitlement is available on this vehicle or account
        /// </summary>
        [JsonProperty("eligible")]
        public bool? Eligible { get; set; }

        /// <summary>
        /// Reason for inelligibility (whether the car is incapable or the owner isn't subscribed)
        /// </summary>
        [JsonProperty("ineligibleReasonCode")]
        public string IneligibleReasonCode { get; set; }

        /// <summary>
        /// True or false if the entitlement can send notifications
        /// </summary>
        [JsonProperty("notificationCapable")]
        public bool? NotificationCapable { get; set; }
    }


}
