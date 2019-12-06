using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Open_Shift.Models;
using Open_Shift.ViewModels;

namespace Open_Shift.Controllers
{
    public class ScheduleController : Controller
    {
        // GET: Schedule
        public ActionResult Index()
        {
            if (!Models.User.GetUserSession().IsAuthenticated)
            {
                return RedirectToAction("SignIn", "Profile");
            }

            return View();
        }
    }
}