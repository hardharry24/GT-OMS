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
    
    public partial class tbl_commission_detail
    {
        public long comm_id { get; set; }
        public long comm_no { get; set; }
        public long si_no { get; set; }
        public string si_type { get; set; }
    
        public virtual tbl_commission_master tbl_commission_master { get; set; }
    }
}
