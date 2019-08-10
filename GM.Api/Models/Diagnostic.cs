using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace GM.Api.Models
{
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
