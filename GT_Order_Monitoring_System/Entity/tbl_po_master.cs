//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GT_Order_Monitoring_System.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class tbl_po_master
    {
        public string po_no { get; set; }
        public System.DateTime po_date { get; set; }
        public long terms { get; set; }
        public string notes { get; set; }
        public string status { get; set; }
        public long customer_id { get; set; }
        public long po_id { get; set; }
        public string control { get; set; }
        public string void_by { get; set; }
        public Nullable<System.DateTime> delivery_date { get; set; }
        public string hold_type { get; set; }
        public string holdstatus { get; set; }
    }
}
