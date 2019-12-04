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
            var u = new User().GetUserSession( );

            if(!u.IsAuthenticated)
{
                return Redirect("~/Login");
            }

            Models.Database db = new Database();
            AvailabilityViewModel viewModel = null;

            if (Request.QueryString["m"] != null && Request.QueryString["y"] != null)
            {
                viewModel = new AvailabilityViewModel
                {
                    User = u,
                    availabilities = db.GetAvailabilities(u.StoreID, Convert.ToInt32(Request.QueryString["m"]), Convert.ToByte(Request.QueryString["m"]), u.AssociateID)
                };
            }

            return View(viewModel);
        }
    }
}