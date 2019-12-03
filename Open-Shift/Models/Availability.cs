﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Open_Shift.Models
{
    public class Availability
    {
        public int Id { get; set; }
        public int AssociateId { get; set; }
        public string AssociateName { get; set; }
        public bool IsManager { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

		private User Availability(DataRow dr)
		{
			try
			{
                Id = (int)dr["ID"];
                AssociateId = (int)dr["AssociateID"];
                AssociateName = dr["AssociateName"].ToString( );
                IsManager = (bool)dr["IsManager"];
                StartTime = (DateTime)dr["StartTime"];
                EndTime = (DateTime)dr["EndTime"];

			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

        /*
        id: 1,
        associateId: 1,
        associateName: "Rikako Kakinuma",
        isManager: true,
        startTime: "2019-11-26 17:00:00",
        endTime: "2019-11-26 23:59:59"=-
        */
    }
}