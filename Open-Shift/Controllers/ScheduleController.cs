﻿using System;
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
    public class ScheduleController : Controller
    {
        // GET: Schedule
        [HttpGet]
        public ActionResult Index()
        {
            var u = Models.User.GetUserSession();

            if (!u.IsAuthenticated || u.EmailVerificationToken != "" || u.StatusID == Models.User.StatusList.InActive)
            {
                return RedirectToAction("SignIn", "Profile");
            }

            var viewModel = new ScheduleViewModel
            {
                User = u,
                Availabilities = null,
                Shifts = null
            };

            if (Request.QueryString["m"] != null && Request.QueryString["y"] != null)
            {
                Models.Database db = new Database();

                viewModel.Availabilities = db.GetAvailabilities(u.StoreID, Convert.ToInt32(Request.QueryString["y"]), Convert.ToByte(Request.QueryString["m"]));
                viewModel.Shifts = db.GetShifts(u.StoreID, Convert.ToInt32(Request.QueryString["y"]), Convert.ToByte(Request.QueryString["m"]));
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create()
        {
            var u = Models.User.GetUserSession();
            if (!u.IsAuthenticated || u.EmailVerificationToken != "" || u.StatusID == Models.User.StatusList.InActive)
            {
                return Content("{\"status\": \"AUTHENTICATION_FAILED\"}", "application/json");
            }
            if (u.IsManager == 0)
            {
                return Content("{\"status\": \"PERMISSION_DENIED\"}", "application/json");
            }

            Models.Database db = new Database();

            Stream req = Request.InputStream;
            req.Seek(0, System.IO.SeekOrigin.Begin);
            string shiftJson = new StreamReader(req).ReadToEnd().ToString();
            string buffer = shiftJson.ToString();
            var dateTimeConverter = new IsoDateTimeConverter();

            var shift = Newtonsoft.Json.JsonConvert.DeserializeObject<Shift>(shiftJson, dateTimeConverter);

            var id = db.InsertShift(shift);

            if (id > 0)
                return Content("{\"status\": \"SUCCESS\", \"id\": " + id + "}", "application/json");
            else
                return Content("{\"status\": \"INSERT_FAILED\"}", "application/json");

        }

        [HttpDelete]
        public ActionResult Delete()
        {
            var u = Models.User.GetUserSession();
            if (!u.IsAuthenticated || u.EmailVerificationToken != "" || u.StatusID == Models.User.StatusList.InActive)
            {
                return Content("{\"status\": \"AUTHENTICATION_FAILED\"}", "application/json");
            }
            if (u.IsManager == 0)
            {
                return Content("{\"status\": \"PERMISSION_DENIED\"}", "application/json");
            }

            Models.Database db = new Database();

            Stream req = Request.InputStream;
            req.Seek(0, System.IO.SeekOrigin.Begin);
            string shiftJson = new StreamReader(req).ReadToEnd().ToString();
            string buffer = shiftJson.ToString();
            var dateTimeConverter = new IsoDateTimeConverter();

            var shift = Newtonsoft.Json.JsonConvert.DeserializeObject<Shift>(shiftJson, dateTimeConverter);

            shift.Delete();

            return Content("{\"status\": \"SUCCESS\"}", "application/json");
        }
    }
}