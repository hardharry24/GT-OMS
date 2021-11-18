using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GT_Order_Monitoring_System.Controllers
{
    public class ControlController : BaseController
    {
        // GET: Control
        public ActionResult Local()
        {
            //Checking Session
            if (Session["code"] == null) return RedirectToAction("Login", "Home", new { ReturnUrl = System.Web.HttpContext.Current.Request.Url.PathAndQuery });

            ViewBag.a_criteria1 = "";
            ViewBag.a_criteria2 = "";
            ViewBag.a_criteria3 = "";

            var listLocalMaster = db.tbl_control_local_master.ToList();
            g_mymodel.ListLocalMaster = listLocalMaster;

            return View(g_mymodel);
        }
    }
}