﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Open_Shift.Controllers
{
	public class ErrorController : Controller
	{
		public ActionResult Index()
		{
			try
			{
				Models.Error E = new Models.Error();
				Models.Database db = new Models.Database();

				return View(E);
			}
			catch (Exception ex)
			{
				Models.SysLog.UpdateLogFile(this.ToString(), MethodBase.GetCurrentMethod().Name.ToString(), ex.Message);
				return RedirectToAction("Index", "Error");
			}
		}

		public ActionResult About()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}

		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}
	}
}