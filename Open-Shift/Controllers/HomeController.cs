using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Open_Shift.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            try
            {
                if (!Models.User.GetUserSession().IsAuthenticated)
                {
                    return RedirectToAction("SignIn", "Profile");
                }

                Models.Database db = new Models.Database();

                var viewModel = new ViewModels.HomeViewModel();
                viewModel.User = Models.User.GetUserSession();
                viewModel.NextShift = db.GetNextShift(viewModel.User.AssociateID);
                viewModel.Stores = db.GetStores(viewModel.User.AssociateID);

                return View(viewModel);
            }
            catch (Exception ex)
            {
                Models.SysLog.UpdateLogFile(this.ToString(), MethodBase.GetCurrentMethod().Name.ToString(), ex.Message);
                return RedirectToAction("Index", "Error");
            }
        }
    }
}