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
    
    public partial class tbl_po_detail
    {
        public long item_id { get; set; }
        public decimal qty { get; set; }
        public string unit { get; set; }
        public string code { get; set; }
        public string description { get; set; }
        public decimal price { get; set; }
        public decimal disc { get; set; }
        public decimal amount { get; set; }
        public long po_id { get; set; }
        public Nullable<long> pcsperpack { get; set; }
        public decimal remqty { get; set; }
        public decimal overqty { get; set; }
        public string status { get; set; }
        public string overlimit { get; set; }
        public string control_type { get; set; }
        public string order_no { get; set; }
        public string product_category { get; set; }
        public string order_status { get; set; }
        public string plate_no { get; set; }
        public string revision_code { get; set; }
        public string revision_desc { get; set; }
        public string prod_code { get; set; }
    }
}