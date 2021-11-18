using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GT_Order_Monitoring_System.Entity;
using PagedList;

namespace GT_Order_Monitoring_System.Models
{
    public class MyModel
    {
        //db
        public tbl_user CurrentUser { get; set; }
        public tbl_customer Customer { get; set; }
        public tbl_agent Agent { get; set; }
        public Response Response { get; set; }
        public List<tbl_user> AllUser { get; set; }
        public tbl_commercial_product CommercialProduct { get; set; }
        public class Code { public string code { get; set; } }
        public tbl_job_product JobProduct { get; set; }
        public tbl_po_master PoMaster { get; set; }
        public tbl_po_detail PoDetails { get; set; }
        public tbl_polybags_jo JobOrderPolybags { get; set; }
        public tbl_laminated_jo JobOrderLaminated { get; set; }
        public tbl_control_local_master ControlLocalMaster { get; set; }
        public tbl_control_local_detail ControlLocalDetail { get; set; }

        public tbl_control_outoftown_master ControlOutOfTownMaster { get; set; }
      
        public tbl_control_outoftown_detail ControlOutOfTownDetail { get; set; }


        public IPagedList<tbl_user> PagedUser { get; set; }




        //db Billing
        public List<vw_job_product_customer> ListJobProduct { get; set; }
        public List<tbl_commercial_product> ListCommercialProducts { get; set; }


        public List<tbl_ship> ListShips { get; set; }
        public List<tbl_truck> ListTruck { get; set; }
        public List<tbl_customer> ListCustomer { get; set; }
        public List<tbl_po_master> ListPurchaseOrder { get; set; }

        
        public List<vw_job_product_customer> ListVwJobProductCust { get; set; }
        public List<tbl_agent> ListAgent { get; set; }
        public List<tbl_po_detail> ListSalesOrderDetails { get; set; }
        public List<tbl_po_master> ListSalesOrderMaster { get; set; }
        public List<tbl_polybags_jo> ListJOpolybags { get; set; }
        public List<tbl_laminated_jo> ListJOlaminated { get; set; }
        public List<tbl_control_local_master> ListLocalMaster { get; set; }
        public List<tbl_control_local_detail> ListLocalDetail { get; set; }
        public List<tbl_control_local_packing> ListLocalPckng { get; set; }

    }
}