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
    public class CustomerController : BaseController
    {
        // GET: Customer
        public ActionResult Index()
        {
            //Checking Session
            if (Session["code"] == null) return RedirectToAction("Login", "Home", new { ReturnUrl = System.Web.HttpContext.Current.Request.Url.PathAndQuery });
            
            ViewBag.a_criteria1 = "";
            ViewBag.a_criteria2 = "";
            ViewBag.a_criteria3 = "";

            g_mymodel.ListCustomer = db2.tbl_customer.ToList();
            g_mymodel.ListAgent = db2.tbl_agent.ToList();
            g_mymodel.Response = new Response();

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
                var mListCustomer = db2.sp_billing_search_customer(a_criteria1, a_criteria2, a_criteria3);
                g_mymodel.ListCustomer = JsonConvert.DeserializeObject<List<tbl_customer>>(JsonConvert.SerializeObject(mListCustomer));
            }
            catch (Exception ex)
            {
                g_mymodel.ListCustomer = new List<tbl_customer>();
            }

            g_mymodel.ListAgent = db2.tbl_agent.ToList();

            return View(g_mymodel);
        }
        public ActionResult Create()
        {
            //Checking Session
            if (Session["code"] == null) return RedirectToAction("Login", "Home", new { ReturnUrl = System.Web.HttpContext.Current.Request.Url.PathAndQuery });

            ViewBag.errorCode = "0";
            ViewBag.errorMessage = "";

            ViewBag.agentCode = "";
            ViewBag.agentName = "";

            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Create(tbl_customer a_customer, String creditLimit, String balance, String over, int a_agentCode)
        {
            try
            {
                int agentId = a_agentCode;
                var agentCode = db2.tbl_agent.Where(m => m.id == agentId).FirstOrDefault();

                ViewBag.agentCode = agentCode.agent_code;
                ViewBag.agentName = agentCode.agent_name;

                a_customer.customer_id = db2.tbl_customer.Max(m => m.customer_id) + 1;
                a_customer.credit_limit = Convert.ToDecimal(creditLimit.Replace(",", ""));
                a_customer.balance = Convert.ToDecimal(balance.Replace(",", ""));
                a_customer.over_ = Convert.ToDecimal(over.Replace(",", ""));
                a_customer.agent_code = agentCode.agent_code;
                db2.tbl_customer.Add(a_customer);
                db2.SaveChanges();

                a_customer.agent_code = agentId.ToString();


                ViewBag.errorCode = "1";
                ViewBag.errorMessage = "Successfully Created!";
            }
            catch (Exception ex)
            {
                ViewBag.errorCode = "0";
                ViewBag.errorMessage = ex.ToString();
                
            }
            
            return View(a_customer);
        }

        public ActionResult Update(int? id)
        {
            //Checking Session
            if (Session["code"] == null) return RedirectToAction("Login", "Home",new { ReturnUrl = System.Web.HttpContext.Current.Request.Url.PathAndQuery });
            

            ViewBag.errorCode = "0";
            ViewBag.errorMessage = "";

            

            var customer = db2.tbl_customer.Where(m => m.customer_id == id).FirstOrDefault();
            var agentDetails = db2.tbl_agent.Where(m => m.agent_code == customer.agent_code).FirstOrDefault();

            customer.agent_code = agentDetails.id.ToString();
            ViewBag.agentCode = agentDetails.agent_code;
            ViewBag.agentName = agentDetails.agent_name;

            return View(customer);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Update(tbl_customer a_customer)
        {
            try
            {

                int agentId = Convert.ToInt32(a_customer.agent_code);
                var agentCode = db2.tbl_agent.Where(m => m.id == agentId).FirstOrDefault();

                ViewBag.agentCode = agentCode.agent_code;
                ViewBag.agentName = agentCode.agent_name;


                var custDetails = db2.tbl_customer.Where(m => m.customer_id == a_customer.customer_id).FirstOrDefault();

                a_customer.agent_code = agentCode.agent_code;
                custDetails.customer_name = a_customer.customer_name;
                custDetails.address = a_customer.address;
                custDetails.contact_person = a_customer.contact_person;
                custDetails.phone_no = a_customer.phone_no;
                custDetails.fax_no = a_customer.fax_no;
                custDetails.email_add = custDetails.email_add;
                custDetails.agent_code = agentCode.agent_code;
                custDetails.credit_limit = custDetails.credit_limit;
                custDetails.credit_terms = a_customer.credit_terms;
                custDetails.balance = a_customer.balance;
                custDetails.over_ = a_customer.over_;
                custDetails.status = a_customer.status;

                db2.SaveChanges();

                a_customer.agent_code = agentId.ToString();


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
            //Checking Session
            if (Session["code"] == null) return RedirectToAction("Login", "Home",new { ReturnUrl = System.Web.HttpContext.Current.Request.Url.PathAndQuery });
            
            var response = new Response();

            try
            {
                var itemObj = db2.tbl_customer.Where(m => m.customer_id == id).FirstOrDefault();
                db2.tbl_customer.Remove(itemObj);
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