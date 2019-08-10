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
        [JsonProperty("size")]
        public string Size { get; set; }

        [JsonProperty("vehicle")]
        public Vehicle[] Vehicle { get; set; }
    }

    public class Vehicle
    {
        [JsonProperty("vin")]
        public string Vin { get; set; }

        [JsonProperty("make")]
        public string Make { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("year")]
        public string Year { get; set; }

        [JsonProperty("manufacturer")]
        public string Manufacturer { get; set; }

        [JsonProperty("bodyStyle")]
        public string BodyStyle { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("unitType")]
        public string UnitType { get; set; }

        [JsonProperty("onstarStatus")]
        public string OnStarStatus { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("isInPreActivation")]
        public bool? IsInPreActivation { get; set; }

        [JsonProperty("insuranceInfo")]
        public InsuranceInfo InsuranceInfo { get; set; }

        [JsonProperty("enrolledInContinuousCoverage")]
        public bool? EnrolledInContinuousCoverage { get; set; }

        [JsonProperty("commands")]
        public Commands Commands { get; set; }

        [JsonProperty("modules")]
        public Modules Modules { get; set; }

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
        [JsonProperty("command")]
        public Command[] Command { get; set; }
    }

    public class Command
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("isPrivSessionRequired")]
        public bool? IsPrivSessionRequired { get; set; }

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
        [JsonProperty("supportedDiagnostic")]
        public string[] SupportedDiagnostic { get; set; }
    }

    public class Modules
    {
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
        [JsonProperty("entitlement")]
        public Entitlement[] Entitlement { get; set; }
    }

    public class Entitlement
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("eligible")]
        public bool? Eligible { get; set; }

        [JsonProperty("ineligibleReasonCode")]
        public string IneligibleReasonCode { get; set; }

        [JsonProperty("notificationCapable")]
        public string NotificationCapable { get; set; }
    }


}
