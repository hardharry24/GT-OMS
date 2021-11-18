using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GT_Order_Monitoring_System.Entity;
using GT_Order_Monitoring_System.Models;
using Newtonsoft.Json;
using System.Data.Entity.Validation;
using System.Data;
using GT_Order_Monitoring_System.Models;

namespace GT_Order_Monitoring_System.Controllers
{
    public class OrderController : BaseController
    {
        public MyModel orderModel = new MyModel();
        // GET: Order
        public ActionResult PurchaseOrder()
        {
            return View(g_mymodel);
        }

        public ActionResult PurchaseControl(string id)
        {
            //Checking Session
            if (Session["code"] == null) return RedirectToAction("Login", "Home", new { ReturnUrl = System.Web.HttpContext.Current.Request.Url.PathAndQuery });

            string[] argArr = id.Split('-');

            Session["PCcntrol"] = argArr[0] == DEFINITION.CONTROL.LOCAL ? DEFINITION.CONTROL.LOCAL : DEFINITION.CONTROL.OUTOFTOWN;
            Session["PCtype"] = argArr[1] == DEFINITION.JO_TYPE.POLYBAG ? DEFINITION.JO_TYPE.POLYBAG : DEFINITION.JO_TYPE.LAMINATED;

            //Initialize
            ViewBag.errorCode = "";
            ViewBag.errorMessage = "";

            //Session["PClocalMaster"] = new List<tbl_control_local_master>();
           
            Session["PClocalDetail"] = new List<tbl_control_local_detail>();
            Session["PClocalPacking"] = new List<tbl_control_local_packing>();
            //Session["PCOutTownItem"] = new tbl_control_outoftown_detail();
            Session["OrNo"] = "";

            g_mymodel.ListTruck = db.tbl_truck.ToList();
            //g_mymodel.ControlLocalMaster
            Session["PCno"] = GetControlNo(Session["PCtype"] + "-" + Session["PCcntrol"]);

            return View(g_mymodel);
        }
        [HttpPost]
        public ActionResult PurchaseControl(DateTime control_date, String trip, String PoNo, String truck, String customer_id, String status)
        {
            string cntrol = (string)Session["PCcntrol"];
            if (cntrol == DEFINITION.CONTROL.LOCAL)
            {
                var ctl_localMaster = new tbl_control_local_master();
                ctl_localMaster.control_date = control_date;
                ctl_localMaster.control_no = db.tbl_control_local_master.Max(m => m.control_no) + 1;
                ctl_localMaster.control_type = (string)Session["PCtype"] == DEFINITION.JO_TYPE.POLYBAG ? "P" : "L";
                ctl_localMaster.customer_id = Convert.ToInt32( customer_id );
                //ctl_localMaster.order_no = (string)Session["OrNo"];
                ctl_localMaster.po_no = PoNo;
                ctl_localMaster.status = status;
                ctl_localMaster.trip = trip;
                ctl_localMaster.truck_no = truck;

                db.tbl_control_local_master.Add(ctl_localMaster);
                db.SaveChanges();

                var PClocalDetail = (List<tbl_control_local_detail>)Session["PClocalDetail"];
                var PClocalPacking = (List<tbl_control_local_packing>)Session["PClocalPacking"];

                foreach (var objPClocalDetail in PClocalDetail)
                {
                    var bgVal = 0;
                    foreach (var item in db.tbl_control_local_detail.ToList())
                    {
                        if (Convert.ToInt32(item.item_code.Split('-')[0]) > bgVal)
                            bgVal = Convert.ToInt32(item.item_code.Split('-')[0]);
                    }
                    //var itemTblLocal = db.tbl_control_local_detail.OrderByDescending(m => m.item_code).FirstOrDefault().item_code.Split('-')[0] + "-1"+ ctl_localMaster.control_type;

                    objPClocalDetail.item_code = (bgVal + 1) + "-1"+ ctl_localMaster.control_type;
                    objPClocalDetail.control_no = ctl_localMaster.control_no;
                    db.tbl_control_local_detail.Add(objPClocalDetail);
                    db.SaveChanges();

                    foreach (var objPClocalPacking in PClocalPacking)
                    {
                        var pckId = db.tbl_control_local_packing.Max(m=>m.packing_id) + 1;
                        objPClocalPacking.packing_id = pckId;
                        objPClocalPacking.item_code = objPClocalDetail.item_code;
                        db.tbl_control_local_packing.Add(objPClocalPacking);
                        db.SaveChanges();
                    }
                }
            }
            ViewBag.errorCode = "1";
            ViewBag.errorMessage = "Succesfully Created!";

            g_mymodel.ListTruck = db.tbl_truck.ToList();
            return View(g_mymodel);
        }

        public PartialViewResult PartialPurchaseCtlPromptItemDetail(string poNo)
        {
          
            var PCcntrol = (string)Session["PCcntrol"];
            var PCtype = (string)Session["PCtype"] == DEFINITION.JO_TYPE.POLYBAG ? "P" : "L";
            //Session["PCType"] = PCtype;

            var poMaster = db.tbl_po_master.Where(m => m.po_no == poNo).FirstOrDefault();
            var poDetail = db.tbl_po_detail.Where(m => m.po_id == poMaster.po_id && m.control_type == PCtype).ToList();
            
            //g_mymodel.ListSalesOrderMaster = db.tbl_po_master.Where(m => m.po_no == poNo && m.control == PCcntrol).ToList();
            g_mymodel.ListSalesOrderDetails = poDetail;


            return PartialView(g_mymodel);
        }

        public void addPurchaseOrderMaster(int itemId,int qty, string unit)
        {
            var PClocalDetail = (List<tbl_control_local_detail>)Session["PClocalDetail"];
            var PClocalPacking = (List<tbl_control_local_packing>)Session["PClocalPacking"];

            var itemDetail = db.tbl_po_detail.Where(m => m.item_id == itemId).FirstOrDefault();
            Session["OrNo"] = itemDetail.order_no;
            //create item 
            var itemLocalDetail = new tbl_control_local_detail();

            itemLocalDetail.control_no = Convert.ToUInt32(Session["PCno"].ToString().Split('-')[1]);
            itemLocalDetail.control_type = itemDetail.control_type;
            itemLocalDetail.description = itemDetail.description;
            itemLocalDetail.item_code = itemLocalDetail.control_no + "-1" + ((string)Session["PCtype"] == DEFINITION.JO_TYPE.POLYBAG ? "P" : "L");
            itemLocalDetail.qty = qty;
            itemLocalDetail.status = "Ok";
            itemLocalDetail.unit = unit;

            PClocalDetail.Add(itemLocalDetail);

            for (int i = 0; i < qty; i++)
            {
                var itemPacking = new tbl_control_local_packing();
                itemPacking.code = itemDetail.code;
                itemPacking.description = itemDetail.description;
                itemPacking.item_code = itemLocalDetail.item_code;
                itemPacking.order_no = itemDetail.order_no;
                itemPacking.packing_id = i;//db.tbl_control_local_packing.Max(m => m.packing_id) + 1;
                itemPacking.qtyinkgs = 0;
                itemPacking.qtyinpcs = 0;
                itemPacking.qtyinrolls = 0;
                itemPacking.status = "Ok";

                PClocalPacking.Add(itemPacking);
            }

            Session["PClocalDetail"] = PClocalDetail;
            Session["PClocalPacking"] = PClocalPacking;
        }

        public ActionResult RetrieveItems()
        {
            var objResponse = new {
                tbl_control_local_detail = (List<tbl_control_local_detail>)Session["PClocalDetail"],
                tbl_control_local_packing = (List<tbl_control_local_packing>)Session["PClocalPacking"]
            };

            return Json(objResponse, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult UpdatePcQty(double valueTxt, int id, string Qtytype)
        {
            var response = new Response();
            string cntrol = (string)Session["PCcntrol"];
            if (cntrol == DEFINITION.CONTROL.LOCAL)
            {
                var PClocalPacking = (List<tbl_control_local_packing>)Session["PClocalPacking"];
                var iObj = PClocalPacking.Where(m => m.packing_id == id).FirstOrDefault();

                if (Qtytype == DEFINITION.QTY.KGS)
                    iObj.qtyinkgs = (decimal)valueTxt;
                if (Qtytype == DEFINITION.QTY.PCS)
                    iObj.qtyinpcs = (long)valueTxt;
                if (Qtytype == DEFINITION.QTY.ROLL)
                    iObj.qtyinrolls = (long)valueTxt;

                Session["PClocalPacking"] = PClocalPacking;
            }
            
            response.code = 1;

            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Sale()
        {
            //Checking Session
            if (Session["code"] == null) return RedirectToAction("Login", "Home",new { ReturnUrl = System.Web.HttpContext.Current.Request.Url.PathAndQuery });


            ViewBag.a_criteria1 = "";
            ViewBag.a_criteria2 = "";
            ViewBag.a_criteria3 = "";

           
            //Fetch PO has customer id equivalent from the customer table
            var listAllMasterOrder = db.tbl_po_master.ToList();
            var listOrderMasterWithCustomer = new List<tbl_po_master>();
            foreach (var item in listAllMasterOrder)
            {
                var hasCustomer = db2.tbl_customer.Where(m => m.customer_id == item.customer_id).FirstOrDefault();
                if (hasCustomer != null)
                    listOrderMasterWithCustomer.Add(item);
            }

            g_mymodel.ListSalesOrderMaster = listOrderMasterWithCustomer;

            return View(g_mymodel);
        }
        [HttpPost]
        public ActionResult Sale(String a_criteria1, String a_criteria2, String a_criteria3)
        {
            //Checking Session
            if (Session["code"] == null) return RedirectToAction("Login", "Home",new { ReturnUrl = System.Web.HttpContext.Current.Request.Url.PathAndQuery });
            
            

            ViewBag.a_criteria1 = a_criteria1;
            ViewBag.a_criteria2 = a_criteria2;
            ViewBag.a_criteria3 = a_criteria3;

            try
            {
                var mListSaleMaster = db2.sp_billing_search_commercial_product(a_criteria1, a_criteria2, a_criteria3);
                g_mymodel.ListCommercialProducts = JsonConvert.DeserializeObject<List<tbl_commercial_product>>(JsonConvert.SerializeObject(mListSaleMaster));
            }
            catch (Exception ex)
            {
                g_mymodel.ListCommercialProducts = new List<tbl_commercial_product>();
            }
            

            //Fetch PO has customer id equivalent from the customer table
            var listAllMasterOrder = db.tbl_po_master.ToList();
            var listOrderMasterWithCustomer = new List<tbl_po_master>();
            foreach (var item in listAllMasterOrder)
            {
                var hasCustomer = db2.tbl_customer.Where(m => m.customer_id == item.customer_id).FirstOrDefault();
                if (hasCustomer != null)
                    listOrderMasterWithCustomer.Add(item);
            }

            g_mymodel.ListSalesOrderMaster = listOrderMasterWithCustomer;

            return View(g_mymodel);
        }

        public ActionResult Create()
        {
            ViewBag.TabTitle = "";
            //Checking Session
            if (Session["code"] == null) return RedirectToAction("Login", "Home",new { ReturnUrl = System.Web.HttpContext.Current.Request.Url.PathAndQuery });

            Session["customerDetails"] = null;
            Session["agentDetails"] = null;
            Session["JobItems"] = new List<tbl_po_detail>();
            Session["total"] = 0;
            Session["CountItems"] = 0;
            Session["action"] = "Create";

            ViewBag.errorCode = "0";
            ViewBag.errorMessage = "";
            
            
            g_mymodel.ListCommercialProducts = db2.tbl_commercial_product.ToList();
            g_mymodel.ListJobProduct = db2.vw_job_product_customer.ToList();

            return View(g_mymodel);
        }
        [HttpPost]
        public ActionResult Create(String cust_customer_name, DateTime delivery_date, String salearea, String po_no ,String type, DateTime po_delivery_date, int terms , String total, String notes)
        {
            try
            {

                var customer = (tbl_customer)Session["customerDetails"];
                var agent = (tbl_agent)Session["agentDetails"];
                var items = (List<tbl_po_detail>)Session["JobItems"];
                Session["action"] = "Create";

                var poMaster = new tbl_po_master();
                poMaster.customer_id = customer.customer_id;
                poMaster.delivery_date = delivery_date;
                poMaster.control = salearea;
                poMaster.status = "Unserved";
                poMaster.po_date = po_delivery_date;
                poMaster.delivery_date = delivery_date;
                poMaster.po_no = po_no;
                poMaster.terms = terms;
                poMaster.po_id = (db.tbl_po_master.Max(m => m.po_id)) + 1;
                poMaster.holdstatus = "Ok";
                poMaster.hold_type = "NA";
                poMaster.notes = notes;

                db.tbl_po_master.Add(poMaster);
                db.SaveChanges();

                foreach (var item in items)
                {
                    var objItemProd = item;
                    objItemProd.po_id = poMaster.po_id;
                    objItemProd.item_id = db.tbl_po_detail.Max(m => m.item_id) + 1;
                    objItemProd.overlimit = item.overlimit == "True" ? "T" : "F";
                    objItemProd.status = "Unserved";
                    db.tbl_po_detail.Add(objItemProd);
                    db.SaveChanges();
                }

                ViewBag.errorCode = "1";
                ViewBag.errorMessage = "Successfully Created!";

                //Clear Sessions
                Session["customerDetails"] = null;
                Session["agentDetails"] = null;
                Session["JobItems"] = new List<tbl_po_detail>();
                Session["total"] = 0;
                Session["CountItems"] = 0;
            }
            catch (DbEntityValidationException e)
            {
                ViewBag.errorCode = "0";
                ViewBag.errorMessage = e.EntityValidationErrors.ToString();
            }
            
            return View();
        }

        public ActionResult Update(int id)
        {
            //Checking Session
            if (Session["code"] == null) return RedirectToAction("Login", "Home", new { ReturnUrl = System.Web.HttpContext.Current.Request.Url.PathAndQuery });

            ViewBag.TabTitle = "Update Sale Order";
            Session["action"] = "Update";
            Session["TabTitle"] = "Update Sale Order";
            
            var poMaster = db.tbl_po_master.Where(m=>m.po_id == id).FirstOrDefault();
            var poDetails = db.tbl_po_detail.Where(m => m.po_id == id).ToList();

            var customerInfo = db2.tbl_customer.Where(m => m.customer_id == poMaster.customer_id).FirstOrDefault();
            var agentInfo = db2.tbl_agent.Where(m => m.agent_code == customerInfo.agent_code).FirstOrDefault();

            ViewBag.errorCode = "0";
            ViewBag.errorMessage = "";

            Session["customerDetails"] = customerInfo;
            Session["agentDetails"] = agentInfo;
            Session["JobItems"] = poDetails;
            Session["poMaster"] = poMaster;
            Session["total"] = poDetails.Sum(m => m.amount);
            Session["CountItems"] = poDetails.Count;
            Session["JobItems"] = poDetails;
            //Session["CustomerInfo"] =

            g_mymodel.Agent = agentInfo;
            g_mymodel.Customer = customerInfo;
            g_mymodel.PoMaster = poMaster;
            g_mymodel.ListSalesOrderDetails = poDetails;

            return View(g_mymodel);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Update(String postatus, DateTime delivery_date, String salearea, String po_no, String type, DateTime po_delivery_date, int terms, String total, String notes)
        {
            try
            {
                var customer = (tbl_customer)Session["customerDetails"];
                var agent = (tbl_agent)Session["agentDetails"];
                var items = (List<tbl_po_detail>)Session["JobItems"];
                var pOmaster = (tbl_po_master)Session["poMaster"];

                var poMaster = db.tbl_po_master.Where(m => m.po_id == pOmaster.po_id).FirstOrDefault();

                Session["action"] = "Update";

                
                poMaster.delivery_date = delivery_date;
                poMaster.control = salearea;
                poMaster.po_date = po_delivery_date;
                poMaster.delivery_date = delivery_date;
                poMaster.terms = terms;
                poMaster.notes = notes;
                db.SaveChanges();
                //db.tbl_po_master.Add(poMaster);


                var dbItems = db.tbl_po_detail.Where(m => m.po_id == pOmaster.po_id).ToList();
                
                if (items.Count != dbItems.Count)
                {
                    if (items.Count > dbItems.Count)
                    {
                        foreach (var item in items)
                        {
                            //if (items.Exists(item))
                            var isExist = dbItems.Where(m => m.item_id == item.item_id).FirstOrDefault();
                            if (isExist == null)
                            {
                                //items.Add(item);
                                item.po_id = poMaster.po_id;
                                db.tbl_po_detail.Add(item);
                                db.SaveChanges();
                            }
                        }
                    }
                    else if (items.Count < dbItems.Count)
                    {
                        foreach (var item in items)
                        {
                            //if (items.Exists(item))
                            var isExist = dbItems.Where(m => m.item_id == item.item_id).FirstOrDefault();
                            if (isExist != null)
                            {
                                item.po_id = poMaster.po_id;
                                db.tbl_po_detail.Remove(item);
                                db.SaveChanges();
                            }
                        }
                    }
                    else
                    {
                        //foreach (var item in items)
                        //{

                        //}
                    }
                }

                db.SaveChanges();

                //foreach (var item in dbItems)
                //{
                   
                //}
                ViewBag.errorCode = "1";
                ViewBag.errorMessage = "Successfully Updated!";

                var updatedItems = db.tbl_po_detail.Where(m => m.po_id == pOmaster.po_id).ToList();

                //Set Sessions
                Session["customerDetails"] = customer;
                Session["agentDetails"] = agent;
                Session["JobItems"] = items;
                Session["poMaster"] = pOmaster;


                g_mymodel.Agent = agent;
                g_mymodel.Customer = customer;
                g_mymodel.PoMaster = poMaster;
                g_mymodel.ListSalesOrderDetails = items;

                Session["total"] = items.Sum(m => m.amount);
                Session["CountItems"] = items.Count;
            }
            catch (DbEntityValidationException e)
            {
                ViewBag.errorCode = "0";
                ViewBag.errorMessage = e.EntityValidationErrors.ToString();
            }

            return View(g_mymodel);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AddItem(int itemId, String unit,int qty, int disc,double pckpck, String type, String status, String prodType)
        {
            //Checking Session
            if (Session["code"] == null) return RedirectToAction("Login", "Home",new { ReturnUrl = System.Web.HttpContext.Current.Request.Url.PathAndQuery });

            var action = Session["action"].ToString();
            var total = Convert.ToDouble( Session["total"].ToString());

            var items = (List<tbl_po_detail>)Session["JobItems"];
            var jobItem = new tbl_job_product();
            var comItem = new tbl_commercial_product();
            var response = new Response();

            var obj_po_detail_item = new tbl_po_detail();
            if (prodType == "Job")
            {
                jobItem = db2.tbl_job_product.Where(m => m.id == itemId).FirstOrDefault();

                obj_po_detail_item.item_id = itemId;
                obj_po_detail_item.amount = Convert.ToDecimal( getTotalAmountByUnitDisc(jobItem,comItem, unit, qty, disc, prodType));
                obj_po_detail_item.description = jobItem.description;
                obj_po_detail_item.unit = getUnit(unit);
                obj_po_detail_item.qty = qty;
                obj_po_detail_item.code = jobItem.code;
                obj_po_detail_item.price = getPrice(jobItem,comItem, unit, prodType);
                obj_po_detail_item.disc = disc;
                obj_po_detail_item.pcsperpack = Convert.ToInt64(pckpck);
                //obj_po_detail_item.po_id = pOmaster.po_id;
                obj_po_detail_item.order_status = status;
                obj_po_detail_item.plate_no = jobItem.plate_no;
                obj_po_detail_item.revision_code = jobItem.revision_code;
                obj_po_detail_item.revision_desc = jobItem.revision_desc;
                obj_po_detail_item.prod_code = jobItem.prod_code;
                obj_po_detail_item.product_category = prodType;
                obj_po_detail_item.control_type = type;
                //obj_po_detail_item.po_id = pOmaster.po_id;
                obj_po_detail_item.item_id = db.tbl_po_detail.Max(m => m.item_id) + 1;
                obj_po_detail_item.overlimit = "F";
                obj_po_detail_item.status = "Unserved";

                if (action == "Create")
                {
                    items.Add(obj_po_detail_item);
                    total = 0;
                    foreach (var item in items)
                    {
                        total += Convert.ToDouble(item.amount);
                    }
                }
                else if (action == "Update")
                {
                    items.Add(obj_po_detail_item);
                    total = 0;
                    foreach (var item in items)
                    {
                        total += Convert.ToDouble(item.amount);
                    }
                }
               
            }
            else if (prodType == "Commercial")
            {
                comItem = db2.tbl_commercial_product.Where(m => m.id == itemId).FirstOrDefault();
                obj_po_detail_item.item_id = itemId;
                obj_po_detail_item.amount = Convert.ToDecimal(getTotalAmountByUnitDisc(jobItem, comItem, unit, qty, disc, prodType));
                obj_po_detail_item.description = comItem.description;
                obj_po_detail_item.unit = getUnit(unit);
                obj_po_detail_item.qty = qty;
                obj_po_detail_item.code = comItem.code;
                obj_po_detail_item.price = getPrice(jobItem,comItem, unit , prodType);
                obj_po_detail_item.disc = disc;
                obj_po_detail_item.pcsperpack = Convert.ToInt64(pckpck);
                //obj_po_detail_item.remqty = jobItem.
                //obj_po_detail_item.overqty = jobItem.
                //obj_po_detail_item.status 
                obj_po_detail_item.order_status = status;
                obj_po_detail_item.plate_no = null;
                obj_po_detail_item.revision_code = null;
                obj_po_detail_item.revision_desc = null;
                obj_po_detail_item.prod_code = comItem.code;
                obj_po_detail_item.product_category = prodType;
                obj_po_detail_item.control_type = type;



                if (action == "Create")
                {
                    items.Add(obj_po_detail_item);
                    total += Convert.ToDouble(obj_po_detail_item.amount);
                }
                else if (action == "Update")
                {
                    items.Add(obj_po_detail_item);
                    total += Convert.ToDouble(obj_po_detail_item.amount);

                    db.tbl_po_detail.Add(obj_po_detail_item);
                    db.SaveChanges();
                }
            }

            Session["JobItems"] = items;
            Session["total"] = total; 
            Session["CountItems"] = items.Count;


            response.code = 1;
            response.message = "Success";

            var objResponse = new
            {
                Response = response,
                ListItems = items,
                item = obj_po_detail_item,
                count = items.Count,
                total = total,
                CountItems = items.Count
            };

            return Json(objResponse, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateAllowOverLimit(String prodType, int id, String value)
        {
            //Checking Session
            if (Session["code"] == null) return RedirectToAction("Login", "Home",new { ReturnUrl = System.Web.HttpContext.Current.Request.Url.PathAndQuery });

            var response = new Response();
            try
            {
                if (prodType == "NA")
                {
                    response.code = 2;
                    response.message = "Cannot Update Null Item Type!";
                }
                else
                { 
                    var items = (List<tbl_po_detail>)Session["JobItems"];
                    
                    var objItem = items.Where(m => m.item_id == id && m.product_category.ToLower() == prodType.ToLower()).FirstOrDefault();
                    var valueToUpdate = "";

                    if (value == "True")
                        valueToUpdate = "T";
                    else
                        valueToUpdate = "F";

                    objItem.overlimit = valueToUpdate;
                    
                    var updatedItems = items;
                    Session["JobItems"] = updatedItems;

                    response.code = 1;
                    response.message = "Updated";
                }
            }
            catch (Exception ex)
            {
                response.code = 0;
                response.message = ex.Data.ToString(); ;
            }
          

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteItem(int? id)
        {
            //Checking Session
            if (Session["code"] == null) return RedirectToAction("Login", "Home",new { ReturnUrl = System.Web.HttpContext.Current.Request.Url.PathAndQuery });
            var action = Session["action"].ToString();

            var response = new Response();

            var items = (List<tbl_po_detail>)Session["JobItems"];
            var total = Convert.ToDouble(Session["total"].ToString());

            var objToDelete = items.Where(m => m.item_id == id).FirstOrDefault();
            tbl_po_detail objToDeletedb = new tbl_po_detail();

            if (action == "Update")
            {
                 objToDeletedb = db.tbl_po_detail.Where(m => m.item_id == id).FirstOrDefault();
                db.tbl_po_detail.Remove(objToDeletedb);
                db.SaveChanges();
            }
            items.Remove(objToDelete);

            
            total -= Convert.ToDouble( objToDelete.amount );

            Session["JobItems"] = items;
            Session["total"] = total;
            Session["CountItems"] = items.Count;

            response.code = 1;
            response.message = "Successfully Removed!";

            var res = new
            {
                Response = response,
                total = total,
                CountItems = items.Count,
                tbl_po_detail = objToDeletedb
            };

            return Json(res, JsonRequestBehavior.AllowGet);
        }
        

        public double getTotalAmountByUnitDisc(tbl_job_product a_jobprod, tbl_commercial_product a_commProd, String type, int qty, int disc, String jobType)
        {
            double totalAmt = 0.0;

            if (type == "pc")
            {
                if (disc <= 0)
                    totalAmt = qty * Convert.ToDouble(jobType == "Job" ? a_jobprod.unit_price : a_commProd.unit_price);
                else
                {
                    //To Compute
                    totalAmt = qty * Convert.ToDouble(a_jobprod.unit_price);
                }
            }
            else if (type == "th")
            {
                if (disc <= 0)
                    totalAmt = qty * Convert.ToDouble(jobType == "Job" ? a_jobprod.unit_priceth : a_commProd.unit_priceth);
                else
                {
                    //To Compute
                    totalAmt = qty * Convert.ToDouble(a_jobprod.unit_priceth);
                }
            }
            else if (type == "pck")
            {
                
                if (disc <= 0)
                    totalAmt = qty * Convert.ToDouble(jobType == "Job" ? a_jobprod.unit_pricepck : a_commProd.unit_pricepck);
                else
                {
                    //To Compute
                    totalAmt = qty * Convert.ToDouble(a_jobprod.unit_pricepck);
                }
            }
            else if (type == "kg")
            {
                if (disc <= 0)
                    totalAmt = qty * Convert.ToDouble(jobType == "Job" ? a_jobprod.unit_pricekg : a_commProd.unit_pricekg);
                else
                {
                    //To Compute
                    totalAmt = qty * Convert.ToDouble(a_jobprod.unit_pricekg);
                }
            }
            else if (type == "roll")
            {
                if (disc <= 0)
                    totalAmt = qty * Convert.ToDouble(jobType == "Job" ? a_jobprod.unit_priceroll : a_commProd.unit_priceroll);
                else
                {
                    //To Compute
                    totalAmt = qty * Convert.ToDouble(a_jobprod.unit_priceroll);
                }
            }

            return totalAmt;
        }
        public String getUnit(String a_unit)
        {
            if (a_unit == "pc")
                return "PC(S)";
            else if (a_unit == "th")
                return "TH(S)";
            else if (a_unit == "pck")
                return "PCK(S)";
            else if (a_unit == "kg")
                return "KG(S)";
            else if (a_unit == "roll")
                return "ROLL(S)";
            else
                return "";
        }
        public Decimal getPrice(tbl_job_product a_jobProd, tbl_commercial_product a_comProd, String a_unit, String Type)
        {
            if (a_unit == "pc")
                return Convert.ToDecimal( Type == "Job" ? a_jobProd.unit_price : a_comProd.unit_price);
            else if (a_unit == "th")
                return Convert.ToDecimal( Type == "Job" ? a_jobProd.unit_priceth : a_comProd.unit_priceth );
            else if (a_unit == "pck")
                return Convert.ToDecimal( Type == "Job" ? a_jobProd.unit_pricepck : a_comProd.unit_pricepck );
            else if (a_unit == "kg")
                return Convert.ToDecimal( Type == "Job" ? a_jobProd.unit_pricekg : a_comProd.unit_pricekg);
            else if (a_unit == "roll")
                return Convert.ToDecimal( Type == "Job" ? a_jobProd.unit_priceroll : a_comProd.unit_priceroll);
            else
                return Convert.ToDecimal(0.0);
        }

        public ActionResult GetCustomer(int? id)
        {
            var customerDetails = db2.tbl_customer.Where(m => m.customer_id == id).FirstOrDefault();
            var agentDetails = db2.tbl_agent.Where(m => m.agent_code == customerDetails.agent_code).FirstOrDefault();

            var objReturn = new {
                tbl_customer = customerDetails,
                tbl_agent = agentDetails
            };

            //orderModel.Customer = customerDetails;
            //orderModel.Agent = agentDetails;
            Session["customerDetails"] = customerDetails;
            Session["agentDetails"] = agentDetails;

            return Json(objReturn, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPOtype(String type)
        {
            var response = new Response();

            if (type == "ReqJO")
            {
                var rqId = 0;
                var reqJo = db.tbl_po_master.Where(m => m.po_no.Contains("GTRJO-")).ToList();
                if (reqJo.Count != 0)
                {
                    foreach (var item in reqJo)
                    {
                        if (Convert.ToInt32(item.po_no.Split('-')[1]) > rqId)
                            rqId = Convert.ToInt32(item.po_no.Split('-')[1]);
                    }
                    rqId += 1;
                    response.message = "GTRJO-" + rqId;
                }
                else
                    response.message = "GTRJO-1";
                
            }
            else if (type == "Profing")
            {
                var rqId = 0;
                var reqpro = db.tbl_po_master.Where(m => m.po_no.Contains("GTRPRO-")).ToList();

                if (reqpro.Count != 0)
                {
                    foreach (var item in reqpro)
                    {
                        if (Convert.ToInt32(item.po_no.Split('-')[1]) > rqId)
                            rqId = Convert.ToInt32(item.po_no.Split('-')[1]);
                    }
                    rqId += 1;
                    response.message = "GTRPRO-" + rqId;
                }
                else
                    response.message = "GTRPRO-1";
            }
            else if (type == "Temp")
            {
                var rqId = 0;
                var reqtemp = db.tbl_po_master.Where(m => m.po_no.Contains("TEMP-")).ToList();
                if (reqtemp.Count != 0)
                {
                    foreach (var item in reqtemp)
                    {
                        if (Convert.ToInt32(item.po_no.Split('-')[1]) > rqId)
                            rqId = Convert.ToInt32(item.po_no.Split('-')[1]);
                    }
                    rqId += 1;
                    response.message = "TEMP-" + rqId;
                }
                else
                    response.message = "TEMP-1";
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult Partial_PO_details(int? id)
        {
            var listPoDetails = db.tbl_po_detail.Where(m => m.po_id == id).ToList();
            var varPO = db.tbl_po_master.Where(m => m.po_id == id).FirstOrDefault();
            var contactP = db2.tbl_customer.Where(m => m.customer_id == varPO.customer_id).FirstOrDefault();

            g_mymodel.PoMaster = varPO;
            g_mymodel.ListSalesOrderDetails = listPoDetails;
            g_mymodel.Customer = contactP;

            return PartialView(g_mymodel);
        }

        public ActionResult Delete(int? id)
        {
            //Checking Session
            if (Session["code"] == null) return RedirectToAction("Login", "Home",new { ReturnUrl = System.Web.HttpContext.Current.Request.Url.PathAndQuery });
            
            var response = new Response();

            try
            {
                var itemObj = db.tbl_po_master.Where(m => m.po_id == id).FirstOrDefault();
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

    }
}