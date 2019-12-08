using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace Open_Shift.Models
{
    public class Availability
    {
        public int ID { get; set; }
        public int AssociateID { get; set; }
        public string AssociateName { get; set; }
        public bool IsManager { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public Availability(DataRow dr)
        {
            try
            {
                // TODO: Add this after the procedure includes the AvailabilityID
                // Id = (int)dr["AvailabilityID"];
                AssociateID = (int)dr["AssociateID"];
                AssociateName = dr["LastName"].ToString() + " " + dr["FirstName"].ToString();
                IsManager = (bool)dr["ManagerStatus"];
                StartTime = (DateTime)dr["startTime"];
                EndTime = (DateTime)dr["endTime"];

            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}