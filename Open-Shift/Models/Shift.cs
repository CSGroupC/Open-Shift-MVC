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
                AssociateName = dr["strFirstName"].ToString() + " " + dr["strFirstName"].ToString();
                //IsManager = (bool)dr["blnIsManager"];
                StartTime = (DateTime)dr["dtmShiftBegin"];
                EndTime = (DateTime)dr["dtmShiftEnd"];

            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}