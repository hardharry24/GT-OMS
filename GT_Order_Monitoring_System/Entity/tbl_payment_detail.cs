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
    
    public partial class tbl_payment_detail
    {
        public long invoice_id { get; set; }
        public long payment_id { get; set; }
        public long si_no { get; set; }
        public string si_type { get; set; }
        public System.DateTime si_date { get; set; }
        public decimal si_amount { get; set; }
        public decimal adjustment { get; set; }
        public decimal payment { get; set; }
        public string status { get; set; }
        public string notes { get; set; }
        public string adjustment_type { get; set; }
    }
}
