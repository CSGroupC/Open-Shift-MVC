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
                return RedirectToAction("SignIn", "Profile");
            }

            AvailabilityViewModel viewModel = new AvailabilityViewModel
            {
                User = u,
                Availabilities = null
            };

            if (Request.QueryString["m"] != null && Request.QueryString["y"] != null)
            {
                Models.Database db = new Database();

                viewModel.Availabilities = db.GetAvailabilities(u.StoreID, Convert.ToInt32(Request.QueryString["y"]), Convert.ToByte(Request.QueryString["m"]), u.AssociateID);

            }

            return View(viewModel);
        }
    }
}