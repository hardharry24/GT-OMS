using GT_Order_Monitoring_System.Entity;
using GT_Order_Monitoring_System.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GT_Order_Monitoring_System.Controllers
{
    public class ShipController : BaseController
    {
        // GET: Ship
        public ActionResult Index()
        {
            //Checking Session
            if (Session["code"] == null) return RedirectToAction("Logout", "Home");
            
            ViewBag.a_criteria1 = "";
            ViewBag.a_criteria2 = "";
            ViewBag.a_criteria3 = "";

            g_mymodel.ListShips = db.tbl_ship.ToList();

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
                var mListShip= db.sp_oms_search_ship(a_criteria1, a_criteria2, a_criteria3);
                g_mymodel.ListShips = JsonConvert.DeserializeObject<List<tbl_ship>>(JsonConvert.SerializeObject(mListShip));
            }
            catch (Exception ex)
            {
                g_mymodel.ListShips = new List<tbl_ship>();
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
        public ActionResult Create(tbl_ship a_tblship)
        {
            ViewBag.errorCode = "0";
            ViewBag.errorMessage = "";

            try
            {
                var shipExist = db.tbl_ship.Where(m => m.via.ToLower() == a_tblship.via.ToLower()).ToList();

                if (shipExist.Count >= 1)
                {
                    ViewBag.errorCode = "0";
                    ViewBag.errorMessage = "Error (Agent Code Already Exist!)";

                    return View(a_tblship);
                }

                db.tbl_ship.Add(a_tblship);
                db.SaveChanges();

                ViewBag.errorCode = "1";
                ViewBag.errorMessage = "Successfully Created!";

            }
            catch (Exception ex)
            {
                ViewBag.errorCode = "0";
                ViewBag.errorMessage = ex.ToString();
            }

            return View(a_tblship);
        }
        public ActionResult Update(int? id)
        {
            //Checking Session
            if (Session["code"] == null) return RedirectToAction("Logout", "Home");
            
            ViewBag.errorCode = "0";
            ViewBag.errorMessage = "";

            var shipDetail = db.tbl_ship.Where(m => m.id == id).FirstOrDefault();

            return View(shipDetail);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Update(tbl_ship a_tblship)
        {
            try
            {
                var shipExist = db.tbl_ship.Where(m => m.via.ToLower() == a_tblship.via.ToLower()).ToList();

                if (shipExist.Count >= 1)
                {
                    ViewBag.errorCode = "0";
                    ViewBag.errorMessage = "Error (Agent Code Already Exist!)";

                    return View(a_tblship);
                }

                var shipDetail = db.tbl_ship.Where(m => m.id == a_tblship.id).FirstOrDefault();
                shipDetail.via = a_tblship.via;
                db.SaveChanges();

                ViewBag.errorCode = "1";
                ViewBag.errorMessage = "Successfully Updated";

            }
            catch (Exception ex)
            {
                ViewBag.errorCode = "0";
                ViewBag.errorMessage = ex.ToString();
            }

            return View(a_tblship);
        }

        public ActionResult Delete(int? id)
        {
            var response = new Response();

            try
            {
                var itemObj = db.tbl_ship.Where(m => m.id == id).FirstOrDefault();
                db.tbl_ship.Remove(itemObj);
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