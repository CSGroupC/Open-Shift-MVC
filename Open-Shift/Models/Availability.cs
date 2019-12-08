using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace Open_Shift.Models
{
    public class Availability
    {
        // NOTE: The client-side code relies on this being "ID"
        public int ID { get; set; }
        public int AssociateID { get; set; }
        public string AssociateName { get; set; }
        public bool IsManager { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Notes { get; set; } = "";

        public Availability()
        {

        }

        public Availability(DataRow dr)
        {
            try
            {
                ID = (int)dr["AvailabilityID"];
                AssociateID = (int)dr["AssociateID"];
                AssociateName = dr["FirstName"].ToString() + " " + dr["LastName"].ToString();
                IsManager = (bool)dr["ManagerStatus"];
                StartTime = (DateTime)dr["StartTime"];
                EndTime = (DateTime)dr["EndTime"];

            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public bool Save()
        {
            try
            {
                Models.Database db = new Database();
                int NewAvailabilityID;
                if (ID == 0)
                {
                    NewAvailabilityID = db.InsertAvailability(this);
                    if (NewAvailabilityID > 0) ID = NewAvailabilityID;
                }
                else
                {
                    db.UpdateAvailability(this);
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
                db.DeleteAvailability(this.ID);
                return true;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}