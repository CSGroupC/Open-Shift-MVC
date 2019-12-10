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
                ID = (int)dr["ID"];
                AvailabilityID = (int)dr["AvailabilityID"];
                AssociateID = (int)dr["AssociateID"];
                AssociateName = dr["AssociateName"].ToString();
                IsManager = (bool)dr["IsManager"];
                StartTime = (DateTime)dr["StartTime"];
                EndTime = (DateTime)dr["EndTime"];

            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}