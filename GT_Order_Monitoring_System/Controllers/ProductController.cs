using GT_Order_Monitoring_System.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GT_Order_Monitoring_System.Models;
using Newtonsoft.Json;

namespace GT_Order_Monitoring_System.Controllers
{
    public class ProductController : BaseController
    {
        public GT_Billing_SystemEntities dbBilling;

        public ActionResult CommercialProducts()
        {
            //Checking Session
            if (Session["code"] == null) return RedirectToAction("Logout", "Home");

            //g_mymodel
            ViewBag.a_criteria1 = "";
            ViewBag.a_criteria2 = "";
            ViewBag.a_criteria3 = "";

            dbBilling = new GT_Billing_SystemEntities();

            g_mymodel.ListCommercialProducts = dbBilling.tbl_commercial_product.ToList();
            g_mymodel.ListCustomer = dbBilling.tbl_customer.ToList();



            return View(g_mymodel);
        }
        [HttpPost]
        public ActionResult CommercialProducts(String a_criteria1, String a_criteria2, String a_criteria3)
        {
            dbBilling = new GT_Billing_SystemEntities();
            //g_mymodel
            ViewBag.a_criteria1 = a_criteria1;
            ViewBag.a_criteria2 = a_criteria2;
            ViewBag.a_criteria3 = a_criteria3;

            try
            {
                var mListCommercialProducts = db2.sp_billing_search_commercial_product(a_criteria1, a_criteria2, a_criteria3);
                g_mymodel.ListCommercialProducts = JsonConvert.DeserializeObject<List<tbl_commercial_product>>(JsonConvert.SerializeObject(mListCommercialProducts));
            }
            catch (Exception ex)
            {
                g_mymodel.ListCommercialProducts = new List<tbl_commercial_product>();
            }

            g_mymodel.ListCustomer = dbBilling.tbl_customer.ToList();


            return View(g_mymodel);
        }

        [HttpPost]
        public ActionResult Create(tbl_commercial_product commercialProduct)
        {
            var response = new Response();
            try
            {
                commercialProduct.category = "Commercial";

                db2.tbl_commercial_product.Add(commercialProduct);
                db2.SaveChanges();


                response.code = 1;
                response.message = "Success";
            }
            catch (Exception ex)
            {
                response.code = 0;
                response.message = ex.ToString();

               // g_mymodel. = Customer;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Update(int? id)
        {
            //Checking Session
            if (Session["code"] == null) return RedirectToAction("Logout", "Home");

            dbBilling = new GT_Billing_SystemEntities();

            var commJobprod = dbBilling.tbl_commercial_product.Where(m => m.id == id).FirstOrDefault();

            ViewBag.errorCode = "0";
            ViewBag.errorMessage = "";

            return View(commJobprod);
        }

        [HttpPost]
        public ActionResult Update(tbl_commercial_product model)
        {
            try
            {
                using (dbBilling = new GT_Billing_SystemEntities())
                {
                    // db_billing.sp_billing_commercial_product_update(a_cmp.code, a_cmp.description, a_cmp.unit_price, a_cmp.unit_pricekg, a_cmp.unit_pricepck, a_cmp.unit_priceth);
                    var mObjItem = dbBilling.tbl_commercial_product.Where( m=>m.id == model.id).FirstOrDefault();

                    
                    mObjItem.description = model.description;
                    mObjItem.unit_price = model.unit_price;
                    mObjItem.unit_pricekg = model.unit_pricekg;
                    mObjItem.unit_pricepck = model.unit_pricepck;
                    mObjItem.unit_priceroll = model.unit_priceroll;
                    mObjItem.unit_priceth = model.unit_priceth;

                    dbBilling.SaveChanges();

                    model = mObjItem;

                    ViewBag.errorCode = "1";
                    ViewBag.errorMessage = "Successfully Updated!";
                }
                
            }
            catch (Exception ex)
            {
                ViewBag.errorCode = "0";
                ViewBag.errorMessage = ex.ToString();

                // g_mymodel. = Customer;
            }

            

            return View(model);
        }

        [HttpPost]
        public ActionResult ValidateCode(MyModel.Code param)
        {
            var response = new Response();

            if (param.code != null)
            {
                if (param.code.Trim() != "")
                {
                    var result = db2.tbl_commercial_product.Where(m => m.code.ToLower() == param.code.ToLower()).ToList();

                    if (result.Count == 0)
                    {
                        response.code = 1;
                        response.message = "OK";
                    }
                    else
                    {
                        response.code = 0;
                        response.message = "ERROR";
                    }
                }
                else
                {
                    response.code = 0;
                    response.message = "ERROR";
                }
            }
            else
            {
                response.code = 0;
                response.message = "ERROR";
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Delete(int? id)
        {
            dbBilling = new GT_Billing_SystemEntities();
            var response = new Response();

            try
            {
                var itemObj = dbBilling.tbl_commercial_product.Where(m => m.id == id).FirstOrDefault();
                dbBilling.tbl_commercial_product.Remove(itemObj);
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