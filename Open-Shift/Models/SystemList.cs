using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Open_Shift.Models
{
	public class SystemLists
	{

		public List<SelectListItem> StatusList = new List<SelectListItem>();
		public List<SelectListItem> StoreLocationList = new List<SelectListItem>();


		public SystemLists()
		{
			try
			{
				StatusList = new List<SelectListItem>();
				StatusList.Add(new SelectListItem() { Value = "", Text = "" });
				StatusList.Add(new SelectListItem() { Value = User.StatusList.Active.ToString(), Text = "Active" });
				StatusList.Add(new SelectListItem() { Value = User.StatusList.InActive.ToString(), Text = "In-Active" });
				StatusList.Add(new SelectListItem() { Value = User.StatusList.Active.ToString(), Text = "Probation" });


				StoreLocationList = new List<SelectListItem>();
				StoreLocationList.Add(new SelectListItem() { Value = "", Text = "" });
				StoreLocationList.Add(new SelectListItem() { Value = "1", Text = "1" });
				StoreLocationList.Add(new SelectListItem() { Value = "2", Text = "2" });
				StoreLocationList.Add(new SelectListItem() { Value = "3", Text = "3" });

			}
			catch (Exception ex)
			{
				SysLog.UpdateLogFile(this.ToString(), MethodBase.GetCurrentMethod().Name.ToString(), ex.Message);
				return;
			}

		}
	}
}