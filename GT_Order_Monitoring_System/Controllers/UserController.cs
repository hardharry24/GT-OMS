using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using GT_Order_Monitoring_System.Entity;
using GT_Order_Monitoring_System.Models;
using Newtonsoft.Json;

namespace GT_Order_Monitoring_System.Controllers
{
    public class UserController : BaseController
    {
        // GET: User
        public ActionResult UserManagement()
        {
            //Checking Session
            if (Session["code"] == null) return RedirectToAction("Login", "Home", new { ReturnUrl = System.Web.HttpContext.Current.Request.Url.PathAndQuery });
            
            string userLogCode = Session["code"].ToString();
            ViewBag.a_criteria1 = "";
            ViewBag.a_criteria2 = "";
            ViewBag.a_criteria3 = "";

            g_mymodel.AllUser = db.tbl_user.Where(m => m.code != userLogCode).ToList();

            return View(g_mymodel);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult UserManagement(String a_criteria1, String a_criteria2, String a_criteria3)
        {

            string userLogCode = Session["code"].ToString();
            g_mymodel.AllUser = db.tbl_user.Where(m => m.code != userLogCode).ToList();
            ViewBag.a_criteria1 = a_criteria1;
            ViewBag.a_criteria2 = a_criteria2;
            ViewBag.a_criteria3 = a_criteria3;

            try
            {
                var mListAll = db.sp_oms_search_user(a_criteria1, a_criteria2, a_criteria3);
                g_mymodel.AllUser = JsonConvert.DeserializeObject<List<tbl_user>>(JsonConvert.SerializeObject(mListAll));
            }
            catch (Exception ex)
            {
                g_mymodel.AllUser = new List<tbl_user>();
            }
            return View(g_mymodel);
        }

        public ActionResult Create()
        {
            //Checking Session
            if (Session["code"] == null) return RedirectToAction("Login", "Home", new { ReturnUrl = System.Web.HttpContext.Current.Request.Url.PathAndQuery });

            ViewBag.errorCode = "0";
            ViewBag.errorMessage = "";
            ViewBag.password2 = "";
            var user = new tbl_user();
            user.code = "";
            return View(user);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(tbl_user a_tbluser, String password2)
        {
            ViewBag.password2 = password2;
            try
            {
                var existCode = db.tbl_user.Where(m => m.code.ToLower() == a_tbluser.code.ToLower()).ToList();

                if (existCode.Count >= 1)
                {
                    ViewBag.errorCode = "0";
                    ViewBag.errorMessage = "Error : (Code Already Exist)";

                    return View(a_tbluser);
                }
                if (a_tbluser.code == "Enter here")
                {
                    ViewBag.errorCode = "0";
                    ViewBag.errorMessage = "Error : (Please enter valid code!)";

                    return View(a_tbluser);
                }

                if (a_tbluser.password.Equals(password2))
                {
                    a_tbluser.password = Encryptor(password2);
                    db.tbl_user.Add(a_tbluser);
                    db.SaveChanges();

                    ViewBag.errorCode = "1";
                    ViewBag.errorMessage = "Successfully Created!";
                }
                else
                {
                    ViewBag.errorCode = "0";
                    ViewBag.errorMessage = "Error : (Password not match!)";

                    return View(a_tbluser);
                }
            }
            catch (Exception ex)
            {
                ViewBag.errorCode = "0";
                ViewBag.errorMessage = ex.Data.ToString();
            }

            return View(a_tbluser);
        }

        public ActionResult Update(string code)
        {
            //Checking Session
            if (Session["code"] == null) return RedirectToAction("Login", "Home", new { ReturnUrl = System.Web.HttpContext.Current.Request.Url.PathAndQuery });
            
            ViewBag.errorCode = "0";
            ViewBag.errorMessage = "";

            var tbluser = db.tbl_user.Where(m => m.code == code).FirstOrDefault();
            tbluser.password = DecryptFromBase64String(tbluser.password);
            ViewBag.password2 = tbluser.password;
           

            return View(tbluser);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Update(tbl_user a_tbluser, String password2, String a_oldCode)
        {
            ViewBag.password2 = password2;
            try
            {
                var existCode = db.tbl_user.Where(m => m.code.ToLower() == a_tbluser.code.ToLower()).FirstOrDefault();
               
                if (existCode != null)
                {
                    if (a_oldCode != existCode.code)
                    {
                        ViewBag.errorCode = "0";
                        ViewBag.errorMessage = "Error : (Code Already Exist)";

                        return View(a_tbluser);
                    }
                }

                if (a_tbluser.password.Equals(password2))
                {
                    var updateUser = db.tbl_user.Where(m => m.code == a_tbluser.code).FirstOrDefault();
                    updateUser.name = a_tbluser.name;
                    updateUser.password = Encryptor(a_tbluser.password);
                    updateUser.access = a_tbluser.access;
                    updateUser.acc_level = a_tbluser.acc_level;

                    db.SaveChanges();

                    ViewBag.errorCode = "1";
                    ViewBag.errorMessage = "Successfully Updated!";
                }
                else
                {
                    ViewBag.errorCode = "0";
                    ViewBag.errorMessage = "Error : (Password not match!)";

                    return View(a_tbluser);
                }
            }
            catch (Exception ex)
            {
                ViewBag.errorCode = "0";
                ViewBag.errorMessage = ex.Data.ToString();
            }

            return View(a_tbluser);
        }

        public ActionResult Delete(String code)
        {
            var response = new Response();

            try
            {
                var itemObj = db.tbl_user.Where(m => m.code == code).FirstOrDefault();
                db.tbl_user.Remove(itemObj);
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