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
    
    public partial class tbl_job_product
    {
        public int id { get; set; }
        public string code { get; set; }
        public string description { get; set; }
        public Nullable<decimal> unit_price { get; set; }
        public long customer_id { get; set; }
        public string status { get; set; }
        public string category { get; set; }
        public System.DateTime datelast_order { get; set; }
        public long noofdays_inactive { get; set; }
        public Nullable<decimal> unit_priceth { get; set; }
        public Nullable<decimal> unit_pricepck { get; set; }
        public Nullable<decimal> unit_pricekg { get; set; }
        public Nullable<decimal> unit_priceroll { get; set; }
        public string plate_no { get; set; }
        public string revision_code { get; set; }
        public string revision_desc { get; set; }
        public string prod_code { get; set; }
        public Nullable<int> revision_id { get; set; }
        public string revision_status { get; set; }
        public string plastic_category { get; set; }
    }
}
