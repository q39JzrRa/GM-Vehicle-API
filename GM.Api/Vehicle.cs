using System;
using System.Collections.Generic;
using System.Text;

namespace GM.Api
{


    public class VehiclesResponse
    {
        public Vehicles vehicles { get; set; }
    }

    public class Vehicles
    {
        public string size { get; set; }
        public Vehicle[] vehicle { get; set; }
    }

    public class Vehicle
    {
        public string vin { get; set; }
        public string make { get; set; }
        public string model { get; set; }
        public string year { get; set; }
        public string manufacturer { get; set; }
        public string bodyStyle { get; set; }
        public string phone { get; set; }
        public string unitType { get; set; }
        public string onstarStatus { get; set; }
        public string url { get; set; }
        public string isInPreActivation { get; set; }
        public Insuranceinfo insuranceInfo { get; set; }
        public string enrolledInContinuousCoverage { get; set; }
        public Commands commands { get; set; }
        public Modules modules { get; set; }
        public Entitlements entitlements { get; set; }
        public string propulsionType { get; set; }
    }

    public class Insuranceinfo
    {
        public InsuranceAgent insuranceAgent { get; set; }
    }

    public class InsuranceAgent
    {
        public Phone phone { get; set; }
    }

    public class Phone
    {
    }

    public class Commands
    {
        public Command[] command { get; set; }
    }

    public class Command
    {
        public string name { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public string isPrivSessionRequired { get; set; }
        public CommandData commandData { get; set; }
    }

    public class CommandData
    {
        public SupportedDiagnostics supportedDiagnostics { get; set; }
    }

    public class SupportedDiagnostics
    {
        public string[] supportedDiagnostic { get; set; }
    }

    public class Modules
    {
        public Module[] module { get; set; }
    }

    public class Module
    {
        public string moduleType { get; set; }
        public string moduleCapability { get; set; }
    }

    public class Entitlements
    {
        public Entitlement[] entitlement { get; set; }
    }

    public class Entitlement
    {
        public string id { get; set; }
        public string eligible { get; set; }
        public string ineligibleReasonCode { get; set; }
        public string notificationCapable { get; set; }
    }


}
