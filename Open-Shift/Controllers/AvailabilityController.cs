using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Open_Shift.Models;
using Open_Shift.ViewModels;
using System.IO;
using Newtonsoft.Json.Converters;

namespace Open_Shift.Controllers
{
    public class AvailabilityController : Controller
    {
        // GET: Availbility
        [HttpGet]
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

        [HttpPost]
        public ActionResult Create()
        {
            var u = Models.User.GetUserSession();
            if (!u.IsAuthenticated)
            {
                return Content("{\"status\": \"AUTHENTICATION_FAILED\"}", "application/json");
            }

            Models.Database db = new Database();

            Stream req = Request.InputStream;
            req.Seek(0, System.IO.SeekOrigin.Begin);
            string availabilityJson = new StreamReader(req).ReadToEnd().ToString();
            string buffer = availabilityJson.ToString();
            var dateTimeConverter = new IsoDateTimeConverter();

            var availability = Newtonsoft.Json.JsonConvert.DeserializeObject<Availability>(availabilityJson, dateTimeConverter);

            var result = db.InsertAvailability(availability);

            return Content("{\"status\": \"SUCCESS\"}", "application/json");
        }

        [HttpPut]
        public ActionResult Update()
        {

            return View();
        }

        [HttpDelete]
        public ActionResult Delete()
        {

            return View();
        }
    }
}