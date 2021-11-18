using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GT_Order_Monitoring_System.Entity;

namespace GT_Order_Monitoring_System.Models
{
    public class DEFINITION
    {
        public class JO_TYPE
        {
            public const string POLYBAG = "Polybag";
            public const string LAMINATED = "Laminated";
        }
        public class CONTROL
        {
            public const string LOCAL = "local";
            public const string OUTOFTOWN = "outoftown";
        }

        public class DB
        {
            public const string BILLING = "Billing";
            public const string OMS = "Oms";
            
        }

        public class MENU
        {
            public const string APPROVE = "APPROVE";
            public const string NOTED = "NOTED";
            public const string VERIFIED = "VERIFIED";
        }

        public class CONST
        {
            public const string IMAGEPATH = @"/Uploads/";
            public const string IMG_BLANK = "blank.png";
        }

        public class QTY
        {
            public const string KGS = "kgs";
            public const string PCS = "pcs";
            public const string ROLL = "roll";
        }

        public class RESPONSE
        {
            public const int SUCCESS = 1;
            public const int ERROR = 0;
            public const int EXCEPTION = -1;
        }
    }
}