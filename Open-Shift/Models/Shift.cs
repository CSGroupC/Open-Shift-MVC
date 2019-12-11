using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace Open_Shift.Models
{
    public class Shift
    {
        public int ID { get; set; }
        public int AvailabilityID { get; set; }
        public int AssociateID { get; set; }
        public string AssociateName { get; set; }
        public bool IsManager { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public Shift(DataRow dr)
        {
            try
            {
                ID = (int)dr["intShiftID"];
                AvailabilityID = (int)dr["intAvailabilityID"];
                AssociateID = (int)dr["intAssociateID"];
                AssociateName = dr["strFirstName"].ToString() + " " + dr["strLastName"].ToString();
                IsManager = (bool)dr["blnIsManager"];
                StartTime = (DateTime)dr["dtmShiftBegin"];
                EndTime = (DateTime)dr["dtmShiftEnd"];

            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public bool Save()
        {
            try
            {
                Models.Database db = new Database();
                int NewShiftID;
                if (ID == 0)
                {
                    NewShiftID = db.InsertShift(this);
                    if (NewShiftID > 0) ID = NewShiftID;
                }
                else
                {
                    db.UpdateShift(this);
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
                db.DeleteShift(this.ID);
                return true;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}