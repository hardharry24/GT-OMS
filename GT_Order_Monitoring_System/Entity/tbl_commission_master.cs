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
    
    public partial class tbl_commission_master
    {
        public tbl_commission_master()
        {
            this.tbl_commission_detail = new HashSet<tbl_commission_detail>();
        }
    
        public long comm_no { get; set; }
        public System.DateTime comm_date { get; set; }
        public string agent_code { get; set; }
        public string status { get; set; }
    
        public virtual ICollection<tbl_commission_detail> tbl_commission_detail { get; set; }
    }
}
