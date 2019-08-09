using System;
using System.Collections.Generic;
using System.Text;

namespace GM.Api.Models
{
    public class CommandResponseRoot
    {
        public Commandresponse commandResponse { get; set; }
    }

    
    public class Commandresponse
    {
        public DateTime requestTime { get; set; }
        public DateTime completionTime { get; set; }
        public string url { get; set; }
        public string status { get; set; } //inProgress, success
        public string type { get; set; }
        public ResponseBody body { get; set; }

    }


}
