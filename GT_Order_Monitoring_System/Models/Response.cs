using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GT_Order_Monitoring_System.Models
{
    public class Response
    {
        public int code { get; set; }
        public String message { get; set; }
        public String url { get; set; }
    }

    public class ActionController
    {
        public String controller { get; set; }
        public String action { get; set; }
        public String argument { get; set; }
    }
}