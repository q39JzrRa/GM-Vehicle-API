using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace GM.Api
{



    public class DiagnosticReader
    {

        IEnumerable<Diagnosticresponse> _dr;

        public DiagnosticReader(IEnumerable<Diagnosticresponse> elements)
        {
            _dr = elements;
        }

        public float AmbientAirTempCelcius => float.Parse((from f in _dr
                                                           where f.name == "AMBIENT AIR TEMPERATURE"
                                                           from r in f.diagnosticElement
                                                           where r.name == "AMBIENT AIR TEMPERATURE"
                                                           select r.value).FirstOrDefault());
        public string ChargerPowerLevel => (from f in _dr
                                            where f.name == "CHARGER POWER LEVEL"
                                            from r in f.diagnosticElement
                                            where r.name == "CHARGER POWER LEVEL"
                                            select r.value).FirstOrDefault();

        public float EvBatteryLevelPercent => float.Parse((from f in _dr
                                                           where f.name == "EV BATTERY LEVEL"
                                                           from r in f.diagnosticElement
                                                           where r.name == "EV BATTERY LEVEL"
                                                           select r.value).FirstOrDefault());


    }


    public static class DiagnosticHelper
    {
        public static float GetElectricEconomyKwh(this IEnumerable<Diagnosticresponse> elements)
        {
            var itm = float.Parse((from f in elements
                       where f.name == "ENERGY EFFICIENCY"
                       from r in f.diagnosticElement
                       where r.name == "ELECTRIC ECONOMY"
                       select r.value).FirstOrDefault());

            return itm;
        }

        public static float GetLifetimeEfficiencyKwh(this IEnumerable<Diagnosticresponse> elements)
        {
            var itm = float.Parse((from f in elements
                                   where f.name == "ENERGY EFFICIENCY"
                                   from r in f.diagnosticElement
                                   where r.name == "LIFETIME EFFICIENCY"
                                   select r.value).FirstOrDefault());

            return itm;
        }

        public static float GetLifetimeMpgE(this IEnumerable<Diagnosticresponse> elements)
        {
            var itm = float.Parse((from f in elements
                                   where f.name == "ENERGY EFFICIENCY"
                                   from r in f.diagnosticElement
                                   where r.name == "LIFETIME MPGE"
                                   select r.value).FirstOrDefault());

            return itm;
        }

        public static float GetOdometerKm(this IEnumerable<Diagnosticresponse> elements)
        {
            var itm = float.Parse((from f in elements
                                   where f.name == "ENERGY EFFICIENCY"
                                   from r in f.diagnosticElement
                                   where r.name == "ODOMETER"
                                   select r.value).FirstOrDefault());

            return itm;
        }

        public static float GetEvBatteryLevelPercent(this IEnumerable<Diagnosticresponse> elements)
        {
            var itm = float.Parse((from f in elements
                                   where f.name == "EV BATTERY LEVEL"
                                   from r in f.diagnosticElement
                                   where r.name == "EV BATTERY LEVEL"
                                   select r.value).FirstOrDefault());

            return itm;
        }

    }


    public class DiagnosticRequestRoot
    {
        public static readonly string[] DefaultItems = new string[]
        {
         "ENGINE COOLANT TEMP",
         "ENGINE RPM",
         "HV BATTERY ESTIMATED CAPACITY",
         "LAST TRIP FUEL ECONOMY",
         "ENERGY EFFICIENCY",
         "HYBRID BATTERY MINIMUM TEMPERATURE",
         "EV ESTIMATED CHARGE END",
         "EV BATTERY LEVEL",
         "EV PLUG VOLTAGE",
         "ODOMETER",
         "CHARGER POWER LEVEL",
         "LIFETIME EV ODOMETER",
         "EV PLUG STATE",
         "EV CHARGE STATE",
         "TIRE PRESSURE",
         "AMBIENT AIR TEMPERATURE",
         "LAST TRIP DISTANCE",
         "INTERM VOLT BATT VOLT",
         "GET COMMUTE SCHEDULE",
         "GET CHARGE MODE",
         "EV SCHEDULED CHARGE START",
         "VEHICLE RANGE"
        };


        //public Diagnosticsrequest diagnosticsRequest { get; set; }
    }

    //public class Diagnosticsrequest
    //{
    //    public string[] diagnosticItem { get; set; }
    //}








    public class ResponseBody
    {
        public Diagnosticresponse[] diagnosticResponse { get; set; }
    }

    public class Diagnosticresponse
    {
        public string name { get; set; }
        public Diagnosticelement[] diagnosticElement { get; set; }
    }

    public class Diagnosticelement
    {
        public string name { get; set; }
        public string status { get; set; }
        public string message { get; set; }
        public string value { get; set; }
        public string unit { get; set; }
    }

}
