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
            var user = new User();
            var calendar = new SchedulingCalendar() { Id = 1 };

            var viewModel = new SchedulingViewModel
            {
                User = user,
                Calendar = calendar
            };

            return View(viewModel);
        }
    }
}