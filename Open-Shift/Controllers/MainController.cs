using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Open_Shift.Controllers
{
    public class MainController : Controller
    {
        // GET: Main
        public ActionResult Index()
        {
            try
            {
                if (!Models.User.GetUserSession().IsAuthenticated)
                {
                    return RedirectToAction("SignIn", "Profile");
                }

                Models.Home h = new Models.Home();
                h.User = Models.User.GetUserSession();

                return View(h);
            }
            catch (Exception ex)
            {
                Models.SysLog.UpdateLogFile(this.ToString(), MethodBase.GetCurrentMethod().Name.ToString(), ex.Message);
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase UserImage, Models.User m, FormCollection col)
        {
            try
            {
                if (!Models.User.GetUserSession().IsAuthenticated)
                {
                    return RedirectToAction("SignIn", "Profile");
                }

                Models.User u = new Models.User();
                Models.Home h = new Models.Home();

                if (col["btnSubmit"] == "Profile") return RedirectToAction("Index", "Profile");
                if (col["btnSubmit"] == "Availibity") return RedirectToAction("Index", "Availbility");
                if (col["btnSubmit"] == "Schedule") return RedirectToAction("Index", "Schedule");
                return View(h);
            }
            catch (Exception ex)
            {
                Models.SysLog.UpdateLogFile(this.ToString(), MethodBase.GetCurrentMethod().Name.ToString(), ex.Message);
                return RedirectToAction("Index", "Error");
            }
        }
    }
}