using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GT_Order_Monitoring_System.Controllers
{
    public class ReportController : BaseController
    {
        // GET: Report
        public ActionResult AllCustomer()
        {
            g_mymodel.ListCustomer = db.tbl_customer.ToList();
            return View(g_mymodel);
        }
    }
}