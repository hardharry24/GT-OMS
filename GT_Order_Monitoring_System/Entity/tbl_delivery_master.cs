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
    
    public partial class tbl_delivery_master
    {
        public tbl_delivery_master()
        {
            this.tbl_delivery_detail = new HashSet<tbl_delivery_detail>();
        }
    
        public long delivery_id { get; set; }
        public System.DateTime delivery_date { get; set; }
        public string truck_no { get; set; }
        public string trip { get; set; }
        public decimal gross { get; set; }
        public decimal tare { get; set; }
        public string notes { get; set; }
    
        public virtual ICollection<tbl_delivery_detail> tbl_delivery_detail { get; set; }
    }
}
