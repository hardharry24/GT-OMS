using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GT_Order_Monitoring_System.Entity;
using GT_Order_Monitoring_System.Models;
using Newtonsoft.Json;

namespace GT_Order_Monitoring_System.Controllers
{
    public class AgentController : BaseController
    {
        // GET: Agent
        public ActionResult Index()
        {
            //Checking Session
            if (Session["code"] == null) return RedirectToAction("Login", "Home", new { ReturnUrl = System.Web.HttpContext.Current.Request.Url.PathAndQuery });

            ViewBag.a_criteria1 = "";
            ViewBag.a_criteria2 = "";
            ViewBag.a_criteria3 = "";

            g_mymodel.ListAgent = db2.tbl_agent.ToList();

            return View(g_mymodel);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Index(String a_criteria1, String a_criteria2, String a_criteria3)
        {
            ViewBag.a_criteria1 = a_criteria1;
            ViewBag.a_criteria2 = a_criteria2;
            ViewBag.a_criteria3 = a_criteria3;


            try
            {
                var mListAgent = db2.sp_billing_search_agent(a_criteria1, a_criteria2, a_criteria3);
                g_mymodel.ListAgent = JsonConvert.DeserializeObject<List<tbl_agent>>(JsonConvert.SerializeObject(mListAgent));
            }
            catch (Exception ex)
            {
                g_mymodel.ListAgent = new List<tbl_agent>();
            }
            

            return View(g_mymodel);
        }

        public ActionResult Create()
        {
            //Checking Session
            if (Session["code"] == null) return RedirectToAction("Login", "Home", new { ReturnUrl = System.Web.HttpContext.Current.Request.Url.PathAndQuery });

            ViewBag.errorCode = "0";
            ViewBag.errorMessage = "";

            return View();
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(tbl_agent a_tblAgent)
        {
            try
            {
                var agentCode = db2.tbl_agent.Where(m=>m.agent_code.ToLower() == a_tblAgent.agent_code.ToLower()).ToList();
                var agentName = db2.tbl_agent.Where(m => m.agent_name.ToLower() == a_tblAgent.agent_name.ToLower()).ToList();

                if (agentCode.Count >= 1 )
                {
                    ViewBag.errorCode = "0";
                    ViewBag.errorMessage = "Error (Agent Code Already Exist!)";

                    return View(a_tblAgent);
                }
                if (agentName.Count >= 1)
                {
                    ViewBag.errorCode = "0";
                    ViewBag.errorMessage = "Error (Agent Name Already Exist!)";

                    return View(a_tblAgent);
                }
                // a_tblAgent.id = db_billing.tbl_agent.Max(m => m.id) + 1;
                db2.tbl_agent.Add(a_tblAgent);
                db2.SaveChanges();

                ViewBag.errorCode = "1";
                ViewBag.errorMessage = "Successfully Created!";
            }
            catch (Exception ex)
            {
                ViewBag.errorCode = "0";
                ViewBag.errorMessage = ex.ToString();
            }
            return View(a_tblAgent);
        }

        public ActionResult Update(int? id)
        {
            //Checking Session
            if (Session["code"] == null) return RedirectToAction("Login", "Home", new { ReturnUrl = System.Web.HttpContext.Current.Request.Url.PathAndQuery });
            
            ViewBag.errorCode = "0";
            ViewBag.errorMessage = "";
            var agentDetail = db2.tbl_agent.Where(m => m.id == id).FirstOrDefault();


            return View(agentDetail);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Update(tbl_agent a_tblAgent)
        {
            try
            {
                var currAgent = db2.tbl_agent.Where(m => m.id == a_tblAgent.id).FirstOrDefault();
                currAgent.agent_name = a_tblAgent.agent_name;
                currAgent.phone_no = a_tblAgent.phone_no;
                currAgent.address = a_tblAgent.address;
                currAgent.email_add = a_tblAgent.email_add;
                currAgent.status = a_tblAgent.status;
                currAgent.commission = a_tblAgent.commission;

                db2.SaveChanges();
                
                ViewBag.errorCode = "1";
                ViewBag.errorMessage = "Successfully Updated!";
            }
            catch (Exception ex)
            {
                ViewBag.errorCode = "0";
                ViewBag.errorMessage = ex.ToString();

            }

            return View();
        }

        public ActionResult Delete(int? id)
        {
            var response = new Response();

            try
            {
                var itemObj = db2.tbl_agent.Where(m => m.id == id).FirstOrDefault();
                db2.tbl_agent.Remove(itemObj);
                db2.SaveChanges();

                response.code = 1;
                response.message = "OK";

            }
            catch (Exception ex)
            {
                response.code = 0;
                response.message = ex.ToString();
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
    }
}