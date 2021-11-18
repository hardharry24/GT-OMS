using GT_Order_Monitoring_System.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GT_Order_Monitoring_System.Controllers
{
    public class HomeController : BaseController
    {
        //DATABASE
        //db -> OMS DATABASE
        //db2 -> OMS BILLING DATABASE
        // GET: Home
        public ActionResult Index()
        {
            ViewBag.Action = "Home";
            Session["from"] = "";
            
            //Checking Session
           if (Session["code"] == null) return RedirectToAction("Login", "Home", new { ReturnUrl = System.Web.HttpContext.Current.Request.Url.PathAndQuery });

            //Set Image Login
            setImage();
            

            g_mymodel.ListCustomer = db2.tbl_customer.ToList();
            g_mymodel.ListPurchaseOrder = db.tbl_po_master.ToList();
            g_mymodel.ListTruck = db.tbl_truck.ToList();
            g_mymodel.ListShips = db.tbl_ship.ToList();

            return View(g_mymodel);
        }
        
        public ActionResult Login(String ReturnUrl="")
        {
            ViewBag.Action = "Login";
            ViewBag.UserCode = "melbacs";
            Session["ReturnUrl"] = ReturnUrl;

            try
            {
                if (Session["isLogged"].ToString() == "true")
                    return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                Session["isLogged"] = "";
            }
            //Checking Session
          
            

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login( String code, String password)
        {
           

            ViewBag.Action = "Login";
            ViewBag.UserCode = code;

            var response = new Response();

            var isExist  = db.tbl_user.Where(m => m.code == code).FirstOrDefault();

            if (isExist == null){
                //ViewBag.LogCode = "0";
                //ViewBag.LogErr = "User Not Register!";

                response.code = 0;
                response.message = "User Not Register!";
                return Json(response, JsonRequestBehavior.AllowGet);
            }

            if (DecryptFromBase64String(isExist.password) == password)  {
                //Get Current Login User
                g_user_login = GetCurrentUser(code);
                Session["code"] = g_user_login.code;
                Session["FullName"] = g_user_login.name.ToString();
                string returnUrl = Session["ReturnUrl"].ToString();

                g_mymodel.CurrentUser = g_user_login;
                Session["isLogged"] = "true";

                if (!string.IsNullOrEmpty( returnUrl ))
                {
                    response.code = 1;
                    response.message = "Successfully Login!";
                    response.url = returnUrl;
                    Session["returnUrl"] = returnUrl;
                    return Json(response, JsonRequestBehavior.AllowGet);
                }
                response.code = 1;
                response.message = "Successfully Login!";
            }
            else {
                response.code = 0;
                response.message = "Incorrect Password!";
                return Json(response, JsonRequestBehavior.AllowGet);
            }

            response.url = "/Home/Index";
            Session["returnUrl"] = "/Home/Index";
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Profile()
        {

            //Checking Session
            if (Session["code"] == null)
            {
                return RedirectToAction("Login", "Home",new { ReturnUrl = System.Web.HttpContext.Current.Request.Url.PathAndQuery });
            }
               

            //Set Image Login
            setImage();

            var currPass = DecryptFromBase64String( g_mymodel.CurrentUser.password );
            g_mymodel.CurrentUser.password = currPass;


            return View(g_mymodel);
        }
        [HttpPost]
        public ActionResult Profile(MyModel a_model, string password2, HttpPostedFileBase file)
        {
            //Set Image Login
            setImage();
           

            var uUpdateUser = db.tbl_user.Where(m => m.code == a_model.CurrentUser.code).FirstOrDefault();
            uUpdateUser.name = a_model.CurrentUser.name;
            uUpdateUser.code = a_model.CurrentUser.code;
            uUpdateUser.password = Encryptor(a_model.CurrentUser.password);

            //Signature
            if (file != null && file.ContentLength > 0)
                try
                {
                    FileInfo fileInfo = new FileInfo(file.FileName);
                    String strSignatureName = DateTime.Now.Ticks+""+ fileInfo.Extension;
                    string path = Path.Combine(Server.MapPath("~/Uploads"),Path.GetFileName(strSignatureName));
                    file.SaveAs(path);
                    uUpdateUser.signature_path = strSignatureName;
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                }
            else
            {
                ViewBag.Message = "You have not specified a file.";
            }

            if (password2 != a_model.CurrentUser.password)
            {
                ViewBag.LogErr = "Password Not Match!";
                ViewBag.LogCod = "NOT";
                return View(a_model);
            }
            else
            {
                db.SaveChanges();
                ViewBag.LogErr = "Successfully Updated!";
                ViewBag.LogCod = "OK";
                a_model.CurrentUser = uUpdateUser;

                return View(a_model);
            }
        }

  

        public ActionResult Logout()
        {
            g_user_login = null;
            ViewBag.Action = "Logout";

            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();

            return RedirectToAction("Login", "Home");
        }
        [HttpPost]
        public ActionResult UploadProfile()
        {
            if (Request.Files.Count > 0)
            {
                try
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";  
                        //string filename = Path.GetFileName(Request.Files[i].FileName);  

                        HttpPostedFileBase file = files[i];
                        string fname, f;//Session["code"].ToString();

                        // Checking for Internet Explorer  
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }

                        string Ucode = Session["code"].ToString();

                        var userLog = db.tbl_user.Where(m => m.code == Ucode).FirstOrDefault();
                        userLog.img = fname;
                        db.SaveChanges();

                        if (!Directory.Exists(Server.MapPath("~/Uploads/")))
                            Directory.CreateDirectory(Server.MapPath("~/Uploads/"));

                            // Get the complete folder path and store the file inside it.  
                        fname = Path.Combine(Server.MapPath("~/Uploads/"), fname);
                        file.SaveAs(fname);
                    }
                    // Returns message that successfully uploaded  

                    
                    return Json("File Uploaded Successfully!");
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }
    }
}