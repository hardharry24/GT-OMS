using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Newtonsoft.Json;
using GT_Order_Monitoring_System.Entity;
using Newtonsoft.Json.Linq;

namespace GT_Order_Monitoring_System.Controllers
{
    public class AdminController : BaseController
    {
        // GET: Admin
        public ActionResult Index()
        {
        

            //String path = @"C:\Users\hn185036\OneDrive - NCR Corporation\Documents\GT\";

            //String filename = "tbl_products_json.json";


            //string rdrValue = "";
            //using (StreamReader readtext = new StreamReader(path+filename))
            //{
            //    rdrValue = readtext.ReadToEnd();
            //}


            //JArray array = JArray.Parse(rdrValue);

            //foreach (JObject obj in array.Children<JObject>())
            //{
            //    tbl_job_product mobj = obj.ToObject<tbl_job_product>();
            //    db_billing.tbl_job_product.Add(mobj);
            //    db_billing.SaveChanges();
            //}

           // var custId = db_billing.tbl_customer.Max(m => m.customer_id);
            ///var spres = db_billing.getCustomerDetails("TEST", "TEST", "TEST").ToList();

            // List<vw_job_product_customer> ListResult = JsonConvert.DeserializeObject<List<vw_job_product_customer>>(JsonConvert.SerializeObject(spres));



            //db_billing.vw_job_product_customer = spres;


            return View();
        }

        public ActionResult Test()
        {
            //if (Session["code"] == null)
            //{
            //    Session["redirect"] = "1";
            //    return RedirectToAction("Login", "Home", new { ReturnUrl = System.Web.HttpContext.Current.Request.Url.PathAndQuery });
            //}

            var users = db.tbl_user.Where(m => m.code == "melbacs").FirstOrDefault();
            users.password = Encryptor(users.password);
            db.SaveChanges();
            //foreach (var item in users)
            //{
            //    item.password = Encryptor(item.password);
            //    db.SaveChanges();
            //}
            return View();
        }
        public String GetPasswordBy(string id)
        {
            var user = db.tbl_user.Where(m => m.code == id).FirstOrDefault();

            return DecryptFromBase64String(user.password);
        }
    }
}