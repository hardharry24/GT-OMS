using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GT_Order_Monitoring_System.Controllers
{
    public class DocumentController : BaseController
    {
        // GET: Document

        public ActionResult PrintPolybag(int id)
        {
            if (Session["code"] == null) return RedirectToAction("Login", "Home", new { ReturnUrl = System.Web.HttpContext.Current.Request.Url.PathAndQuery });

            var polyitem = db.tbl_polybags_jo.Where(m => m.jo_no == id).FirstOrDefault();
            var customer = db2.tbl_customer.Where(m => m.customer_id == polyitem.customer_id).FirstOrDefault();

            g_mymodel.JobOrderPolybags = polyitem;
            g_mymodel.Customer = customer;

            return View(g_mymodel);
        }

        public ActionResult PrintControlLocal(int id)
        {
            var controlLocal = db.tbl_control_local_master.Where(m => m.control_no == id).FirstOrDefault();
            var controlLocalDetail = db.tbl_control_local_detail.Where(m => m.control_no == controlLocal.control_no).FirstOrDefault();
            var controlLocalPack = db.tbl_control_local_packing.Where(m => m.item_code == controlLocalDetail.item_code).ToList();
            var customerDetail = db2.tbl_customer.Where(m => m.customer_id == controlLocal.customer_id).FirstOrDefault();
            
            g_mymodel.ControlLocalMaster = controlLocal;
            g_mymodel.ControlLocalDetail = controlLocalDetail;
            g_mymodel.ListLocalPckng = controlLocalPack;
            g_mymodel.Customer = customerDetail;

            return View(g_mymodel);
        }

        public ActionResult PrintSaleOrder(int id)
        {
            var listPoDetails = db.tbl_po_detail.Where(m => m.po_id == id).ToList();
            var varPO = db.tbl_po_master.Where(m => m.po_id == id).FirstOrDefault();
            var contactP = db2.tbl_customer.Where(m => m.customer_id == varPO.customer_id).FirstOrDefault();

            g_mymodel.PoMaster = varPO;
            g_mymodel.ListSalesOrderDetails = listPoDetails;
            g_mymodel.Customer = contactP;

            return PartialView(g_mymodel);
        }
    }
}