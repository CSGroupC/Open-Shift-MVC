using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Open_Shift.Models;
using Newtonsoft.Json.Converters;


namespace Open_Shift.Controllers
{
    public class ManagerController : Controller
    {

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
            string bodyJson = new StreamReader(req).ReadToEnd().ToString();
            var dateTimeConverter = new IsoDateTimeConverter();
            var bodyType = new
            {
                AssociateID = 0,
                IsManager = Open_Shift.Models.User.IsManagerEnum.Associate
            };
            var body = Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(bodyJson, bodyType);

            if (db.UpdateUserManagerStatus(body.AssociateID, body.IsManager))
            {
                return Content("{\"status\": \"SUCCESS\"}", "application/json");
            }
            else
            {
                return Content("{\"status\": \"UPDATE_FAILED\"}", "application/json");
            }
        }
    }
}