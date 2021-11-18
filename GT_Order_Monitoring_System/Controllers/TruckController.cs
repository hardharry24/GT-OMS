using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GT_Order_Monitoring_System.Entity;
using GT_Order_Monitoring_System.Models;

namespace GT_Order_Monitoring_System.Controllers
{
    public class TruckController : BaseController
    {
        // GET: Truck
        public ActionResult Index()
        {
            //Checking Session
            if (Session["code"] == null) return RedirectToAction("Logout", "Home");
            
            ViewBag.a_criteria1 = "";
            ViewBag.a_criteria2 = "";
            ViewBag.a_criteria3 = "";

            g_mymodel.ListTruck = db.tbl_truck.ToList();

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
                var mListTruck = db.sp_oms_search_truck(a_criteria1, a_criteria2, a_criteria3);
                g_mymodel.ListTruck = JsonConvert.DeserializeObject<List<tbl_truck>>(JsonConvert.SerializeObject(mListTruck));
            }
            catch (Exception ex)
            {
                g_mymodel.ListTruck = new List<tbl_truck>();
            }


            return View(g_mymodel);
        }

        public ActionResult Create()
        {
            //Checking Session
            if (Session["code"] == null) return RedirectToAction("Logout", "Home");
            
            ViewBag.errorCode = "0";
            ViewBag.errorMessage = "";

            return View();
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(tbl_truck a_tbltruck)
        {
            ViewBag.errorCode = "0";
            ViewBag.errorMessage = "";

            try
            {

                var truckExist = db.tbl_truck.Where(m => m.truck_no.ToLower() == a_tbltruck.truck_no.ToLower()).ToList();

                if (truckExist.Count >= 1)
                {
                    ViewBag.errorCode = "0";
                    ViewBag.errorMessage = "Error (Agent Code Already Exist!)";

                    return View(a_tbltruck);
                }

                db.tbl_truck.Add(a_tbltruck);
                db.SaveChanges();

                ViewBag.errorCode = "1";
                ViewBag.errorMessage = "Successfully Created!";

            }
            catch (Exception ex)
            {
                ViewBag.errorCode = "0";
                ViewBag.errorMessage = ex.ToString();
            }

            return View(a_tbltruck);
        }
        public ActionResult Update(int? id)
        {
            //Checking Session
            if (Session["code"] == null) return RedirectToAction("Logout", "Home");
            
            ViewBag.errorCode = "0";
            ViewBag.errorMessage = "";

            var truckDetail = db.tbl_truck.Where(m => m.id == id).FirstOrDefault();

            return View(truckDetail);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Update(tbl_truck a_tbltruck)
        {
            try
            {

                var truckExist = db.tbl_truck.Where(m=>m.truck_no.ToLower() == a_tbltruck.truck_no.ToLower()).ToList();

                if (truckExist.Count >= 1)
                {
                    ViewBag.errorCode = "0";
                    ViewBag.errorMessage = "Error (Agent Code Already Exist!)";

                    return View(a_tbltruck);
                }

                var truckDetail = db.tbl_truck.Where(m => m.id == a_tbltruck.id).FirstOrDefault();
                truckDetail.truck_no = a_tbltruck.truck_no;
                db.SaveChanges();

                ViewBag.errorCode = "1";
                ViewBag.errorMessage = "Successfully Updated";

            }
            catch (Exception ex)
            {
                ViewBag.errorCode = "0";
                ViewBag.errorMessage = ex.ToString();
            }

            return View(a_tbltruck);
        }

        public ActionResult Delete(int? id)
        {
            var response = new Response();

            try
            {
                var itemObj = db.tbl_truck.Where(m => m.id == id).FirstOrDefault();
                db.tbl_truck.Remove(itemObj);
                db.SaveChanges();

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