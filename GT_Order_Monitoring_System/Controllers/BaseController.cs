using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GT_Order_Monitoring_System.Entity;
using GT_Order_Monitoring_System.Models;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using GT_Order_Monitoring_System.Singleton;

namespace GT_Order_Monitoring_System.Controllers
{
    public class BaseController : Controller
    {
        public GT_Order_Monitoring_SystemEntities db = Database.GetInstance().getDB();
        public GT_Billing_SystemEntities db2 = Database.GetInstance().getDB2();
        public Object resultObj = new Object();
        public tbl_user g_user_login { get; set; }
        public MyModel g_mymodel { get; set; }


        public BaseController()
        {
            ViewBag.Action = "";
            ViewBag.LogErr = "";
            ViewBag.LogCode = "";

            g_mymodel = new MyModel();
        }

        public tbl_user GetCurrentUser(string a_Ucode)
        {
            return db.tbl_user.Where(m => m.code == a_Ucode).FirstOrDefault();
        }

        public void setImage()
        {
            string Usercode = Session["code"].ToString();
            var currUser = GetCurrentUser(Usercode);
           // currUser.password = DecryptFromBase64String(currUser.password);
            g_mymodel.CurrentUser = currUser;


            if (g_mymodel.CurrentUser.img == null || g_mymodel.CurrentUser.img == "")
                Session["uImg"] = "~/Content/assets/images/avatars/admin.png";
            else
                Session["uImg"] = "~/Uploads/" + g_mymodel.CurrentUser.img;
        }

        public Object getDBbyName(string a_dbName)
        {
            if (a_dbName == DEFINITION.DB.OMS)
            {
                return new GT_Order_Monitoring_SystemEntities();
            }
            else if (a_dbName == DEFINITION.DB.BILLING)
            {
                return new GT_Billing_SystemEntities();
            }
            return null;
        }

        public tbl_customer getCustomerById(int a_id)
        {
            var db = (GT_Order_Monitoring_SystemEntities)getDBbyName(DEFINITION.DB.OMS);
            return db.tbl_customer.Where(m => m.customer_id == a_id).FirstOrDefault();
        }

        public string Encryptor(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }


        public string DecryptFromBase64String(string cipherText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
        

        public String GetPolyJobOrderNo()
        {
            return String.Format("PJ-{0}", (db.tbl_polybags_jo.Max(m => m.jo_no) + 1).ToString());
        }

        public String GetFlexJobOrderNo()
        {
            return String.Format("LJ-{0}", (db.tbl_laminated_jo.Max(m => m.jo_no) + 1).ToString());
        }

        public ActionResult GetPOdetail(int id)
        {
            var poDetail = db.tbl_po_detail.Where(m => m.item_id == id).FirstOrDefault();
            var poMaster = db.tbl_po_master.Where(m => m.po_id == poDetail.po_id).FirstOrDefault();
            var existObj = db.tbl_polybags_jo.Where(m => m.po_no == poMaster.po_no).FirstOrDefault();
            var result = 0;

            if (existObj != null)
            {
                result = 1;
            }

            var response = new
            {
                result = result,
                tbl_po_detail = poDetail
            };

            Session["order_status"] = response.tbl_po_detail.order_status;

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPOmaster(int id)
        {
            var response = new
            {
                tbl_po_master = db.tbl_po_master.Where(m => m.po_id == id).FirstOrDefault()
            };

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public void GetJobOrder(ref Object tblResult, string type, int id)
        {
            var db =(GT_Order_Monitoring_SystemEntities)getDBbyName(DEFINITION.DB.OMS);

            if (type == DEFINITION.JO_TYPE.POLYBAG)
            {

                var query = db.tbl_polybags_jo.Where(m => m.jo_no == id).FirstOrDefault();
                tblResult = query;
            }
            else if (type == DEFINITION.JO_TYPE.LAMINATED)
            {
                var query = db.tbl_laminated_jo.Where(m => m.jo_no == id).FirstOrDefault();
                tblResult = query; 
            }
        }

        public String GetControlNo(string arg)
        {
            String result = String.Empty;
            var aArrg = arg.Split('-');
            var aType = aArrg[0] == DEFINITION.JO_TYPE.POLYBAG ? DEFINITION.JO_TYPE.POLYBAG : DEFINITION.JO_TYPE.LAMINATED;
            var aControl = aArrg[1] == DEFINITION.CONTROL.LOCAL ? DEFINITION.CONTROL.LOCAL : DEFINITION.CONTROL.OUTOFTOWN;
            long nId = 0;
            switch (aControl)
            {
                case DEFINITION.CONTROL.LOCAL:
                    long id = 0;
                    try {
                        id = db.tbl_control_local_master.Max(m => m.control_no)+1;
                    }
                    catch  {
                        id++;
                    }
                    

                    nId =  + 1;
                    if (aType == DEFINITION.JO_TYPE.POLYBAG)
                        result = "GTL-" + nId;
                    else
                        result = "GTS-" + nId;
                    break;
                case DEFINITION.CONTROL.OUTOFTOWN:
                    nId = db.tbl_control_outoftown_master.Max(m => m.control_no) + 1;
                    if (aType == DEFINITION.JO_TYPE.POLYBAG)
                        result = "GTLL-" + nId;
                    else
                        result = "GTLS-" + nId;
                    break;
            }
            return result;
        }
    }
}