using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Open_Shift.Models;

namespace Open_Shift.Models
{
    public class Store
    {
        // NOTE: The client-side code relies on this being "ID"
        public int StoreID { get; set; }
        public string StoreName { get; set; }
        public int StoreNumber { get; set; }
        public string PostalCode { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public int DailyAssociateMinimum { get; set; }
        public int DailyManagerMinimum { get; set; }
        public List<User> Associates { get; set; } = null;

        public Store()
        {

        }

        public Store(DataRow dr)
        {
            try
            {
                //public int StoreID { get; set; }
                StoreID = (int)dr["intStoreID"];
                //public string StoreName { get; set; }
                StoreName = dr["strStoreName"].ToString();
                //public int StoreNumber { get; set; }
                StoreNumber = (int)dr["intStoreNumber"];
                //public string PostalCode { get; set; }
                PostalCode = dr["strPostalCode"].ToString();
                //public string AddressLine1 { get; set; }
                AddressLine1 = dr["strAddressLine1"].ToString();
                //public string AddressLine2 { get; set; }
                AddressLine2 = dr["strAddressLine2"].ToString();
                //public int DailyAssociateMinimum { get; set; }
                DailyAssociateMinimum = (int)dr["intDailyAssociateMinimum"];
                //public int DailyManagerMinimum { get; set; }
                DailyManagerMinimum = (int)dr["intDailyManagerMinimum"];

                // NOTE: Initialize Associates separately
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public bool Save()
        {
            try
            {
                Models.Database db = new Database();
                int NewStoreID = 0;
                if (StoreID == 0)
                {
                    //NewStoreID = db.InsertStore(this);
                    if (NewStoreID > 0) StoreID = NewStoreID;
                }
                else
                {
                    //db.UpdateStore(this);
                }

                return true;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public bool Delete()
        {
            try
            {
                Database db = new Database();
                // db.DeleteStore(this.StoreID);
                return true;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}