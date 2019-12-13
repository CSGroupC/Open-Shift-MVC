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
        public List<SelectListItem> IsManager = new List<SelectListItem>();
        public List<SelectListItem> AssociateTitle = new List<SelectListItem>();


        public SystemLists()
        {
            try
            {
                StatusList = new List<SelectListItem>();
                StatusList.Add(new SelectListItem() { Value = User.StatusList.Active.ToString(), Text = "Active" });
                StatusList.Add(new SelectListItem() { Value = User.StatusList.InActive.ToString(), Text = "In-Active" });


                StoreLocationList = new List<SelectListItem>();
                StoreLocationList.Add(new SelectListItem() { Value = User.StoreLocationList.Kotetsu.ToString(), Text = "Kotetsu" });

                IsManager = new List<SelectListItem>();
                IsManager.Add(new SelectListItem() { Value = User.IsManagerEnum.Associate.ToString(), Text = "Associate" });
                IsManager.Add(new SelectListItem() { Value = User.IsManagerEnum.Manager.ToString(), Text = "Manager" });

                AssociateTitle = new List<SelectListItem>();
                AssociateTitle.Add(new SelectListItem() { Value = User.AssociateTitles.Cook.ToString(), Text = "Cook" });
                AssociateTitle.Add(new SelectListItem() { Value = User.AssociateTitles.Server.ToString(), Text = "Server" });

            }
            catch (Exception ex)
            {
                SysLog.UpdateLogFile(this.ToString(), MethodBase.GetCurrentMethod().Name.ToString(), ex.Message);
                return;
            }

        }
    }
}