using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Open_Shift.Models
{
    public class Shift
    {
        public int Id { get; set; }
        public int AvailabilityId { get; set; }
        public int AssociateId { get; set; }
        public string AssociateName { get; set; }
        public bool IsManager { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

		private User Shift(DataRow dr)
		{
			try
			{
                Id = (int)dr["ID"];
                AvailabilityId = (int)dr["AvailabilityId"];
                AssociateId = (int)dr["AssociateID"];
                AssociateName = dr["AssociateName"].ToString( );
                IsManager = (bool)dr["IsManager"];
                StartTime = (DateTime)dr["StartTime"];
                EndTime = (DateTime)dr["EndTime"];

			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}
    }
}