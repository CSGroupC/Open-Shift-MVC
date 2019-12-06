using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Open_Shift.Models;
using Open_Shift.ViewModels;

namespace Open_Shift.Controllers
{
    public class AvailabilityController : Controller
    {
        // GET: Availbility
        public ActionResult Index()
        {
            var u = Models.User.GetUserSession();

            if (!u.IsAuthenticated)
            {
                return Redirect("~/Login");
            }

            AvailabilityViewModel viewModel = new AvailabilityViewModel
            {
                User = new User()
                {
                    blnIsManager = Models.User.IsManager.Manager,
                    StoreID = Models.User.StoreLocationList.Kotetsu,
                    AssociateID = 2
                },
                availabilities = null
            };

            if (Request.QueryString["m"] != null && Request.QueryString["y"] != null)
            {
                Models.Database db = new Database();

                /*
                viewModel = new AvailabilityViewModel
                {
                    User = u,
                    availabilities = db.GetAvailabilities(u.StoreID, Convert.ToInt32(Request.QueryString["m"]), Convert.ToByte(Request.QueryString["m"]), u.AssociateID)
                };
                */
                // TODO: Stop hard-coding this and use the real data from the database
                viewModel.availabilities = db.GetAvailabilities(u.StoreID, Convert.ToInt32(Request.QueryString["m"]), Convert.ToByte(Request.QueryString["m"]), u.AssociateID);

            }

            return View(viewModel);
        }
    }
}