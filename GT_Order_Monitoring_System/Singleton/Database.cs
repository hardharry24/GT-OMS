using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GT_Order_Monitoring_System.Entity;

namespace GT_Order_Monitoring_System.Singleton
{
   
    public class Database
    {
        private static Database instance;
        private GT_Order_Monitoring_SystemEntities db;
        private GT_Billing_SystemEntities db2;

        public Database()
        {
            db = new GT_Order_Monitoring_SystemEntities();
            db2 = new GT_Billing_SystemEntities();
        }

        public static Database GetInstance()
        {
            if (instance == null)
                instance = new Database();
            return instance;
        }

        public GT_Order_Monitoring_SystemEntities getDB()
        {
            return db;
        }

        public GT_Billing_SystemEntities getDB2()
        {
            return db2;
        }
    }
}