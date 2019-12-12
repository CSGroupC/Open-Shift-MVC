using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Open_Shift.Models;
using Open_Shift.ViewModels;
using System.IO;
using Newtonsoft.Json.Converters;
using System.Data.SqlClient;
using System.Configuration;

namespace Open_Shift.Controllers
{
    public class VerificationController : Controller
    {
        // GET: Schedule
        [HttpGet]
        public ActionResult EmailVerification(string token)
        {
            EmailController.NewAssociateVerificationManager(token); //associate's email is verified - now send email to manager to approve

            Models.Database db = new Database();
            db.ApproveNewAssociate(token);



            return RedirectToAction("Index", "Home");

        }

        [HttpGet]
        public ActionResult EmailManagerApproval(int associateID)
        {
            // EmailController.NewAssociateVerificationManager(token); //associate's email is verified - now send email to manager to approve

            Models.Database db = new Database();
            db.ChangeAssocToActive(associateID);



            return RedirectToAction("Index", "Home");

        }





        [HttpPost]
        public ActionResult Create()
        {
            var u = Models.User.GetUserSession();
            if (!u.IsAuthenticated || u.EmailVerificationToken != "" || u.StatusID == Models.User.StatusList.InActive)
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

            var id = db.InsertAvailability(availability);

            if (id > 0)
                return Content("{\"status\": \"SUCCESS\", \"id\": " + id + "}", "application/json");
            else
                return Content("{\"status\": \"INSERT_FAILED\"}", "application/json");

        }

        [HttpPut]
        public ActionResult Update()
        {
            var u = Models.User.GetUserSession();
            if (!u.IsAuthenticated || u.EmailVerificationToken != "" || u.StatusID == Models.User.StatusList.InActive)
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

            availability.Save();

            return Content("{\"status\": \"SUCCESS\"}", "application/json");
        }

        [HttpDelete]
        public ActionResult Delete()
        {
            var u = Models.User.GetUserSession();
            if (!u.IsAuthenticated || u.EmailVerificationToken != "" || u.StatusID == Models.User.StatusList.InActive)
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

            availability.Delete();

            return Content("{\"status\": \"SUCCESS\"}", "application/json");
        }
    }
}