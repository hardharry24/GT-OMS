using GT_Order_Monitoring_System.Entity;
using GT_Order_Monitoring_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GT_Order_Monitoring_System.Controllers
{
    public class JobOrderController : BaseController
    {

        // GET: JobOrder
        public ActionResult Polybag()
        {
            //Checking Session
            if (Session["code"] == null) return RedirectToAction("Login", "Home", new { ReturnUrl = System.Web.HttpContext.Current.Request.Url.PathAndQuery });
           
            ViewBag.a_criteria1 = "";
            ViewBag.a_criteria2 = "";
            ViewBag.a_criteria3 = "";

            var Polybag = db.tbl_polybags_jo.ToList();
            var ListPolybag = new List<tbl_polybags_jo>();

            foreach (var item in Polybag)
            {
                var custExist = db2.tbl_customer.Where(m => m.customer_id == item.customer_id).FirstOrDefault();
                if (custExist != null)
                    ListPolybag.Add(item);

            }

            g_mymodel.ListJOpolybags = ListPolybag;

            return View(g_mymodel);
        }

        public ActionResult CreatePolybag()
        {
            if (Session["code"] == null) return RedirectToAction("Login", "Home", new { ReturnUrl = System.Web.HttpContext.Current.Request.Url.PathAndQuery });

            ViewBag.TabTitle = "Title";
            ViewBag.errorCode = "";
            ViewBag.errorMessage = "";
            ViewBag.type = "";

            g_mymodel.Customer = new tbl_customer();
            g_mymodel.JobOrderPolybags = new tbl_polybags_jo();

            return View(g_mymodel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePolybag(string OrNo, string radio, string decription, tbl_polybags_jo a_polybags_jo)
        {
     
            if (Session["code"] == null) return RedirectToAction("Login", "Home", new { ReturnUrl = System.Web.HttpContext.Current.Request.Url.PathAndQuery });
            try
            {
                double thicknessSingle = a_polybags_jo.thickness_single == null ? 0 : Convert.ToDouble(a_polybags_jo.thickness_single);
                double thicknessTube = a_polybags_jo.thickness_tube == null ? 0 : Convert.ToDouble(a_polybags_jo.thickness_tube);
                double thicknessFold = a_polybags_jo.thickness_fold == null ? 0 : Convert.ToDouble(a_polybags_jo.thickness_fold);


                a_polybags_jo.thickness_single = (decimal)thicknessSingle;
                a_polybags_jo.thickness_tube = (decimal)thicknessTube;
                a_polybags_jo.thickness_fold = (decimal)thicknessFold;
                a_polybags_jo.created_by = GetCurrentUser(Session["code"].ToString()).name;
                a_polybags_jo.prepared_path = GetCurrentUser(Session["code"].ToString()).signature_path;
                a_polybags_jo.description = decription;
                //
                String[] OrNoArr = OrNo.Split('-');
                a_polybags_jo.jo_no = Convert.ToUInt32(OrNoArr[1]);
                //
                a_polybags_jo.order_status = Session["order_status"].ToString(); ;
                a_polybags_jo.code = a_polybags_jo.prod_code;
                a_polybags_jo.product_category = "Job";
                a_polybags_jo.status = "Pending";

                var exist = db.tbl_polybags_jo.Where(m => m.created_by == a_polybags_jo.created_by && m.code == a_polybags_jo.code && m.customer_id == a_polybags_jo.customer_id && m.po_no == a_polybags_jo.po_no).FirstOrDefault();
                if (exist == null)
                {
                    db.tbl_polybags_jo.Add(a_polybags_jo);
                    db.SaveChanges();

                    //Reset Model
                    g_mymodel.Customer = new tbl_customer();
                    g_mymodel.JobOrderPolybags = new tbl_polybags_jo();

                    ViewBag.errorCode = "1";
                    ViewBag.errorMessage = "Successfully Created!";
                }
                else
                {
                    ViewBag.errorCode = "0";
                    ViewBag.errorMessage = "Already Exist!";
                }


            }
            catch (Exception ex)
            {
                ViewBag.errorCode = "0";
                ViewBag.errorMessage = ex.Data.ToString();
            }

            g_mymodel.Customer = db2.tbl_customer.Where(m => m.customer_id == a_polybags_jo.customer_id).FirstOrDefault();
            g_mymodel.JobOrderPolybags = a_polybags_jo;


            return View(g_mymodel);
        }
        public ActionResult UpdatePolybag(int id)
        {
            if (Session["code"] == null) return RedirectToAction("Login", "Home", new { ReturnUrl = System.Web.HttpContext.Current.Request.Url.PathAndQuery });

            ViewBag.errorCode = "";
            ViewBag.errorMessage = "";
            ViewBag.type = "";

            var jOpolybag = db.tbl_polybags_jo.Where(m => m.jo_no == id).FirstOrDefault();
            var jOCustomer = db2.tbl_customer.Where(m => m.customer_id == jOpolybag.customer_id).FirstOrDefault();

            g_mymodel.Customer = jOCustomer;
            g_mymodel.JobOrderPolybags = jOpolybag;

            return View(g_mymodel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdatePolybag(string OrNo, string radio, string decription, tbl_polybags_jo a_polybags_jo)
        {
            if (Session["code"] == null) return RedirectToAction("Login", "Home", new { ReturnUrl = System.Web.HttpContext.Current.Request.Url.PathAndQuery });
            
            try
            {

                var joPolybag = db.tbl_polybags_jo.Where(m => m.jo_no == a_polybags_jo.jo_no).FirstOrDefault();
                joPolybag.remarks = decription;
                joPolybag.delivery_date = a_polybags_jo.delivery_date;
                joPolybag.ptype = a_polybags_jo.ptype;
                joPolybag.plastic_color = a_polybags_jo.plastic_color;
                joPolybag.bottom_fold = a_polybags_jo.bottom_fold;
                joPolybag.left_fold = a_polybags_jo.left_fold;
                joPolybag.right_fold = a_polybags_jo.right_fold;
                joPolybag.width = a_polybags_jo.width;
                joPolybag.length = a_polybags_jo.length;
                joPolybag.thickness_single = a_polybags_jo.thickness_single;
                joPolybag.thickness_tube = a_polybags_jo.thickness_tube;
                joPolybag.weight_per_pc = a_polybags_jo.thickness_tube;
                joPolybag.checking_based = a_polybags_jo.checking_based;
                joPolybag.plastic_type = a_polybags_jo.plastic_type;
                joPolybag.plastic_category = a_polybags_jo.plastic_category;
                joPolybag.print_style = a_polybags_jo.print_style;
                joPolybag.no_of_colors = a_polybags_jo.no_of_colors;
                joPolybag.front_print = a_polybags_jo.front_print;
                joPolybag.back_print = a_polybags_jo.back_print;
                joPolybag.ordered_qty = a_polybags_jo.ordered_qty;
                joPolybag.unit = a_polybags_jo.unit;
                joPolybag.remarks = decription;
                joPolybag.date_last_edit = DateTime.Now;

                db.SaveChanges();

                ViewBag.errorCode = "1";
                ViewBag.errorMessage = "Successfully Updated!";
                ViewBag.type = "";

                var jOpolybag = joPolybag;
                var jOCustomer = db2.tbl_customer.Where(m => m.customer_id == jOpolybag.customer_id).FirstOrDefault();

                g_mymodel.Customer = jOCustomer;
                g_mymodel.JobOrderPolybags = jOpolybag;

                return View(g_mymodel);

            }
            catch (Exception ex)
            {

                var jOpolybag = db.tbl_polybags_jo.Where(m => m.jo_no == a_polybags_jo.jo_no).FirstOrDefault();
                var jOCustomer = db2.tbl_customer.Where(m => m.customer_id == jOpolybag.customer_id).FirstOrDefault();

                ViewBag.errorCode = "0";
                ViewBag.errorMessage = ex.Data.ToString();
                ViewBag.type = "";
            }

            return View(g_mymodel);
        }

        public ActionResult DeletePoly(int? id)
        {
            var response = new Response();

            try
            {
                var itemObj = db.tbl_polybags_jo.Where(m => m.jo_no == id).FirstOrDefault();
                itemObj.status = "Void";
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

        public ActionResult DeleteFlex(int? id)
        {
            var response = new Response();

            try
            {
                var itemObj = db.tbl_laminated_jo.Where(m => m.jo_no == id).FirstOrDefault();
                itemObj.status = "Void";
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
        public ActionResult GetPObyCustomer(int id, string type)
        {
            var PoList = db.tbl_po_master.Where(m => m.customer_id == id).Select(m => new
            {
                poNo = m.po_no,
                poId = m.po_id
            }).ToList();

            var response = new
            {
                listPo = PoList
            };

            return Json(response, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public PartialViewResult Partial_PolyPOItems(string id)
        {
            var listPoMaster = db.tbl_po_master.Where(m => m.po_no == id).FirstOrDefault();
            var listPoDetails = db.tbl_po_detail.Where(m => m.po_id == listPoMaster.po_id).ToList();

            g_mymodel.ListSalesOrderDetails = listPoDetails;

            return PartialView(g_mymodel);
        }

        [HttpPost]
        public PartialViewResult Partial_FlexPOItems(string id)
        {
            var listPoMaster = db.tbl_po_master.Where(m => m.po_no == id).FirstOrDefault();
            var listPoDetails = db.tbl_po_detail.Where(m => m.po_id == listPoMaster.po_id).ToList();

            g_mymodel.ListSalesOrderDetails = listPoDetails;

            return PartialView(g_mymodel);
        }

        public ActionResult PrintPolybag(int id)
        {
            if (Session["code"] == null) return RedirectToAction("Login", "Home", new { ReturnUrl = System.Web.HttpContext.Current.Request.Url.PathAndQuery });

            var polyitem = db.tbl_polybags_jo.Where(m => m.jo_no == id).FirstOrDefault();
            var customer = db2.tbl_customer.Where(m => m.customer_id == polyitem.customer_id).FirstOrDefault();
            
            g_mymodel.JobOrderPolybags = polyitem;
            g_mymodel.Customer = customer;

            return View(g_mymodel);
        }
        [HttpPost]
        public ActionResult PrintPolybag(int id, string action)
        {
            if (Session["code"] == null) return RedirectToAction("Login", "Home", new { ReturnUrl = System.Web.HttpContext.Current.Request.Url.PathAndQuery });

            var polyitem = db.tbl_polybags_jo.Where(m => m.jo_no == id).FirstOrDefault();
            var customer = db2.tbl_customer.Where(m => m.customer_id == polyitem.customer_id).FirstOrDefault();

            g_user_login = GetCurrentUser(Session["code"].ToString());

            if (action == DEFINITION.MENU.APPROVE)
            {
                polyitem.approved_by = g_user_login.name;
                polyitem.approved_path = g_user_login.signature_path;
            }
            else if (action == DEFINITION.MENU.NOTED)
            {
                polyitem.noted_by = g_user_login.name;
                polyitem.noted_path = g_user_login.signature_path;
            }
            else if (action == DEFINITION.MENU.VERIFIED)
            {
                polyitem.verified_by = g_user_login.name;
                polyitem.verified_path = g_user_login.signature_path;
            }

            db.SaveChanges();

            g_mymodel.JobOrderPolybags = polyitem;
            g_mymodel.Customer = customer;

            return View(g_mymodel);
        }

        public ActionResult Flexible()
        {
            if (Session["code"] == null) return RedirectToAction("Login", "Home", new { ReturnUrl = System.Web.HttpContext.Current.Request.Url.PathAndQuery });
            
            ViewBag.a_criteria1 = "";
            ViewBag.a_criteria2 = "";
            ViewBag.a_criteria3 = "";

            var Laminated = db.tbl_laminated_jo.ToList();
            var ListLaminated = new List<tbl_laminated_jo>();

            foreach (var item in Laminated)
            {
                var custExist = db2.tbl_customer.Where(m => m.customer_id == item.customer_id).FirstOrDefault();
                if (custExist != null)
                    ListLaminated.Add(item);

            }

            g_mymodel.ListJOlaminated = ListLaminated;



            return View(g_mymodel);
        }



        public ActionResult CreateFlexible()
        {
            if (Session["code"] == null) return RedirectToAction("Login", "Home", new { ReturnUrl = System.Web.HttpContext.Current.Request.Url.PathAndQuery });
            
            ViewBag.errorCode = "";
            ViewBag.errorMessage = "";
            ViewBag.type = "";

            g_mymodel.Customer = new tbl_customer();
            g_mymodel.JobOrderLaminated = new tbl_laminated_jo();

            return View(g_mymodel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateFlexible(string OrNo, string radio, string decription, tbl_laminated_jo a_laminated_jo)
        {

            if (Session["code"] == null) return RedirectToAction("Login", "Home", new { ReturnUrl = System.Web.HttpContext.Current.Request.Url.PathAndQuery });
            try
            {
                a_laminated_jo.created_by = GetCurrentUser(Session["code"].ToString()).name;
                a_laminated_jo.description = decription;
                //
                String[] OrNoArr = OrNo.Split('-');
                a_laminated_jo.jo_no = Convert.ToUInt32(OrNoArr[1]);
                //
                //a_laminated_jo.
                a_laminated_jo.order_status = Session["order_status"].ToString(); ;
                a_laminated_jo.code = a_laminated_jo.prod_code;
                a_laminated_jo.product_category = "Job";
                a_laminated_jo.status = "Pending";
                
                var exist = db.tbl_laminated_jo.Where(m => m.created_by == a_laminated_jo.created_by && m.code == a_laminated_jo.code && m.customer_id == a_laminated_jo.customer_id && m.po_no == a_laminated_jo.po_no).FirstOrDefault();
                if (exist != null)
                {
                    db.tbl_laminated_jo.Add(a_laminated_jo);
                    db.SaveChanges();

                    //Reset Model
                    g_mymodel.Customer = new tbl_customer();
                    g_mymodel.JobOrderLaminated = new tbl_laminated_jo();

                    ViewBag.errorCode = "1";
                    ViewBag.errorMessage = "Successfully Created!";
                }
                else
                {
                    ViewBag.errorCode = "0";
                    ViewBag.errorMessage = "Already Exist!";
                }


            }
            catch (Exception ex)
            {
                ViewBag.errorCode = "0";
                ViewBag.errorMessage = ex.Data.ToString();
            }

            g_mymodel.Customer = db2.tbl_customer.Where(m => m.customer_id == a_laminated_jo.customer_id).FirstOrDefault();
            g_mymodel.JobOrderLaminated = a_laminated_jo;


            return View(g_mymodel);
        }

        public PartialViewResult Partial_JO_Poly(int id, string type)
        {
            GetJobOrder(ref resultObj, type, id);

            g_mymodel.JobOrderPolybags =  (tbl_polybags_jo)resultObj;
            g_mymodel.Customer = db2.tbl_customer.Where(m => m.customer_id == g_mymodel.JobOrderPolybags.customer_id).FirstOrDefault();

            return PartialView(g_mymodel);
        }

        public PartialViewResult Partial_JO_Laminated(int id, string type)
        {
            GetJobOrder(ref resultObj, type, id);
         
            g_mymodel.JobOrderLaminated = (tbl_laminated_jo)resultObj;
            g_mymodel.Customer = db2.tbl_customer.Where(m => m.customer_id == g_mymodel.JobOrderLaminated.customer_id).FirstOrDefault();

            return PartialView(g_mymodel);
        }
    }
}