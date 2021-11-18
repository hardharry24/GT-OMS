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
    public class JobController : BaseController
    {
        public GT_Billing_SystemEntities dbBilling;
        // GET: Job
        public ActionResult JobProduct()
        {
            //Checking Session
            if (Session["code"] == null) return RedirectToAction("Login", "Home", new { ReturnUrl = System.Web.HttpContext.Current.Request.Url.PathAndQuery });
            
            ViewBag.a_criteria1 = "";
            ViewBag.a_criteria2 = "";
            ViewBag.a_criteria3 = "";

            var listJob = db2.vw_job_product_customer.ToList();
            g_mymodel.ListJobProduct = listJob;
            g_mymodel.ListCustomer = db2.tbl_customer.ToList();

            return View(g_mymodel);
        }
        //Search
        [HttpPost]
        public ActionResult JobProduct(String a_criteria1, String a_criteria2, String a_criteria3)
        {
            ViewBag.a_criteria1 = a_criteria1;
            ViewBag.a_criteria2 = a_criteria2;
            ViewBag.a_criteria3 = a_criteria3;

            try
            {
                var mListCustomer = db2.sp_billing_search_job_product(a_criteria1, a_criteria2, a_criteria3);
                g_mymodel.ListJobProduct = JsonConvert.DeserializeObject<List<vw_job_product_customer>>(JsonConvert.SerializeObject(mListCustomer));
            }
            catch (Exception ex)
            {
                g_mymodel.ListJobProduct = new List<vw_job_product_customer>();
            }

            g_mymodel.ListCustomer = db2.tbl_customer.ToList();

            return View(g_mymodel);
        }

        public ActionResult Create()
        {
            //Checking Session
            if (Session["code"] == null) return RedirectToAction("Login", "Home", new { ReturnUrl = System.Web.HttpContext.Current.Request.Url.PathAndQuery });

            var listCust = db2.tbl_customer.ToList();
            ViewBag.SelectedCustomer = "";

            ViewBag.errorCode = "0";
            ViewBag.errorMessage = "";
            Session["ListCustomer"] = listCust;

            return View();
        }
        [HttpPost]
        public ActionResult Create(tbl_job_product a_jobprod, String customer_name)
        {
            var response = new Response();
            try
            {
                var codeExist = db2.tbl_job_product.Where(m=>m.code.ToLower() == a_jobprod.code.ToLower()).ToList();
                if (codeExist.Count <= 0)
                {
                    DateTime dtPick = new DateTime();
                    dtPick = a_jobprod.datelast_order;
                    ViewBag.SelectedCustomer = customer_name;


                    a_jobprod.category = "Job";
                   // a_jobprod.datelast_order = null;//dtPick.ToString("dddd, dd MMMM yyyy HH:mm:ss");
                    db2.tbl_job_product.Add(a_jobprod);
                    db2.SaveChanges();
                    
                    ViewBag.errorCode = "1";
                    ViewBag.errorMessage = "Successfully Created!";

                    a_jobprod = new tbl_job_product();
                }
                else
                {
                    ViewBag.errorCode = "0";
                    ViewBag.errorMessage = "Please Enter Valid Code!";
                }
            }
            catch (Exception ex)
            {
                ViewBag.errorCode = "0";
                ViewBag.errorMessage = ex.ToString();
            }
            
            return View(a_jobprod);
        }
        public ActionResult Update(int? id)
        {
            var itemJobProduct = db2.vw_job_product_customer.Where( m => m.id == id).FirstOrDefault();

            ViewBag.errorCode = "0";
            ViewBag.errorMessage = "";

            return View(itemJobProduct);
        }
        [HttpPost]
        public ActionResult Update(vw_job_product_customer a_model)
        {
            using (dbBilling = new GT_Billing_SystemEntities())
            {
                try
                {
                    var itemJobProduct = db2.tbl_job_product.Where(m => m.id == a_model.id).FirstOrDefault();

                    itemJobProduct.customer_id = a_model.customer_id;
                    itemJobProduct.description = a_model.description;
                    itemJobProduct.unit_price = a_model.unit_price;
                    itemJobProduct.unit_pricekg = a_model.unit_pricekg;
                    itemJobProduct.unit_pricepck = a_model.unit_pricepck;
                    itemJobProduct.unit_priceroll = a_model.unit_priceroll;
                    itemJobProduct.unit_priceth = a_model.unit_priceth;
                    itemJobProduct.status = a_model.status;
                    itemJobProduct.plastic_category = a_model.plastic_category;

                    db2.SaveChanges();

                    ViewBag.errorCode = "1";
                    ViewBag.errorMessage = "Successfully Updated!";

                }
                catch (Exception ex)
                {
                    ViewBag.errorCode = "0";
                    ViewBag.errorMessage = ex.ToString();
                }
            }
            // g_mymodel.CommercialProduct = commJobprod;

            return View(a_model);
        }
        public ActionResult Delete(int? id)
        {
            var response = new Response();

            try
            {
                var itemObj = db2.tbl_job_product.Where(m => m.id == id).FirstOrDefault();
                dbBilling.tbl_job_product.Remove(itemObj);
                dbBilling.SaveChanges();

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