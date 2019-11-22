using System;
using System.Web.Mvc;
using System.Reflection;
using System.Web;
using System.IO;
using System.Collections.Generic;

namespace Open_Shift.Controllers
{
	public class ProfileController : Controller
	{
		public ActionResult Index()
		{
			try
			{
				Models.Home h = new Models.Home();
				h.User = h.User.GetUserSession();

				if (RouteData.Values["id"] != null)
				{
					Models.Database db = new Models.Database();
					List<Models.User> users = new List<Models.User>();
					users = db.GetUsers(Convert.ToInt32(RouteData.Values["id"]));
					if (users.Count > 0)
					{
						h.ViewUser = users[0]; //get the first user
					}
				}
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
				Models.User u = new Models.User();
				Models.Home h = new Models.Home();

				if (col["btnSubmit"] == "delete") return RedirectToAction("Delete", "Profile");
				if (col["btnSubmit"] == "close") return RedirectToAction("Index", "Home");

				if (col["btnSubmit"] == "update")
				{
					u = u.GetUserSession();

					u.FirstName = col["User.FirstName"];
					u.LastName = col["User.LastName"];
					u.Birthday = col["User.Birthday"];
					u.AddressLine1 = col["User.AddressLine1"];
                    u.AddressLine2 = col["User.AddressLine2"];
					u.PostalCode = col["User.PostalCode"];
					u.StoreLocation = col["User.StoreLocation"];
					u.EmployeeNumber = col["User.EmployeeNumber"];
					u.AssociateTitle = col["User.AssociateTitle"];
					u.Phonenumber = col["User.Phonenumber"];
					u.Email = col["User.Email"];
					u.ConfirmEmail = col["User.ConfirmEmail"];
					u.UserID = col["User.UserID"];
					u.Password = col["User.Password"];

					//NEW CODE
					u.UserImage.ImageID = Convert.ToInt32(col["User.UserImage.ImageID"]);

					u.Save();
					if (u.IsAuthenticated)
					{ //user found
						if (UserImage != null)
						{
							u.UserImage = new Models.Image();
							u.UserImage.ImageID = Convert.ToInt32(col["User.UserImage.ImageID"]);
							u.UserImage.Primary = true;
							u.UserImage.FileName = Path.GetFileName(UserImage.FileName);
							if (u.UserImage.IsImageFile())
							{
								u.UserImage.Size = UserImage.ContentLength;
								Stream stream = UserImage.InputStream;
								BinaryReader binaryReader = new BinaryReader(stream);
								u.UserImage.ImageData = binaryReader.ReadBytes((int)stream.Length);
								u.UpdatePrimaryImage();
							}
						}
						u.SaveUserSession(); //save the user session object
						return RedirectToAction("Index", "Profile");
					}
					else
					{ //user failed to log in
						h.User.FirstName = col["User.FirstName"];
						h.User.LastName = col["User.LastName"];
						h.User.Birthday = col["User.Birthday"];
						h.User.AddressLine1 = col["User.AddressLine1"];
						h.User.PostalCode = col["User.PostalCode"];
						h.User.StoreLocation = col["User.StoreLocation"];
						h.User.EmployeeNumber = col["User.EmployeeNumber"];
						h.User.AssociateTitle = col["User.AssociateTitle"];
						h.User.Phonenumber = col["User.Phonenumber"];
						h.User.Email = col["User.Email"];
						h.User.ConfirmEmail = col["User.ConfirmEmail"];
						h.User.UserID = col["User.UserID"];
						h.User.Password = col["User.Password"];
					}
				}
				return View(h);
			}
			catch (Exception ex)
			{
				Models.SysLog.UpdateLogFile(this.ToString(), MethodBase.GetCurrentMethod().Name.ToString(), ex.Message);
				return RedirectToAction("Index", "Error");
			}
		}

		public ActionResult Delete()
		{
			try
			{
				Models.Home h = new Models.Home();
				h.User = h.User.GetUserSession();
				return View(h);
			}
			catch (Exception ex)
			{
				Models.SysLog.UpdateLogFile(this.ToString(), MethodBase.GetCurrentMethod().Name.ToString(), ex.Message);
				return RedirectToAction("Index", "Error");
			}
		}

		[HttpPost]
		public ActionResult Delete(Models.User m, FormCollection col)
		{
			try
			{
				Models.User u = new Models.User();
				if (col["btnSubmit"] == "cancel") return RedirectToAction("Index", "Profile");
				u = u.GetUserSession();
				u.Delete();
				return RedirectToAction("Index", "Home");
			}
			catch (Exception ex)
			{
				Models.SysLog.UpdateLogFile(this.ToString(), MethodBase.GetCurrentMethod().Name.ToString(), ex.Message);
				return RedirectToAction("Index", "Error");
			}
		}

		public ActionResult SignIn()
		{
			try
			{
				Models.Home h = new Models.Home();
				return View(h);
			}
			catch (Exception ex)
			{
				Models.SysLog.UpdateLogFile(this.ToString(), MethodBase.GetCurrentMethod().Name.ToString(), ex.Message);
				return RedirectToAction("Index", "Error");
			}
		}

		[HttpPost]
		public ActionResult SignIn(Models.User m, FormCollection col)
		{
			try
			{
				Models.User u = new Models.User(col["UserID"], col["Password"]);
				u.Login();

				if (u.IsAuthenticated)
				{ //user found
					u.SaveUserSession(); //save the user session object
					return RedirectToAction("Index", "Main");
				}
				else
				{ //user failed to log in
					Models.Home h = new Models.Home();
					h.User = u;
					return View(h);
				}
			}
			catch (Exception ex)
			{
				Models.SysLog.UpdateLogFile(this.ToString(), MethodBase.GetCurrentMethod().Name.ToString(), ex.Message);
				return RedirectToAction("Index", "Error");
			}
		}
		public ActionResult SignOut()
		{
			try
			{
				Models.Home h = new Models.Home();
				h.User = h.User.GetUserSession();
				h.User.RemoveUserSession();
				h.User = new Models.User(); //ensure the user object is cleared
				return RedirectToAction("Index", "Home");
			}
			catch (Exception ex)
			{
				Models.SysLog.UpdateLogFile(this.ToString(), MethodBase.GetCurrentMethod().Name.ToString(), ex.Message);
				return RedirectToAction("Index", "Error");
			}
		}

		public ActionResult SignUp()
		{
			try
			{
				Models.Home h = new Models.Home();
				return View(h);
			}
			catch (Exception ex)
			{
				Models.SysLog.UpdateLogFile(this.ToString(), MethodBase.GetCurrentMethod().Name.ToString(), ex.Message);
				return RedirectToAction("Index", "Error");
			}
		}

		[HttpPost]
		public ActionResult SignUp(Models.User m, FormCollection col)
		{
			try
			{
				Models.User u = new Models.User(col["User.FirstName"], col["User.LastName"], col["User.Birthday"],
										col["User.AddressLine1"], col["User.AddressLine2"],  col["User.PostalCode"], col["User.StoreLocation"],
										col["User.EmployeeNumber"], col["User.AssociateTitle"], col["User.PhoneNumber"], col["User.Email"], col["User.ConfirmEmail"],
										col["User.UserID"], col["User.Password"]);
				u.Save();
				if (u.IsAuthenticated)
				{ //user found
					u.SaveUserSession(); //save the user session object
					return RedirectToAction("Index", "Main");
				}
				else
				{ //user failed to log in
					Models.Home h = new Models.Home();
					h.User.FirstName = col["User.FirstName"];
					h.User.LastName = col["User.LastName"];
					h.User.Birthday= col["User.Birthday"];
					h.User.AddressLine1 = col["User.AddressLine1"];
                    h.User.AddressLine2 = col["User.AddressLine2"];
					h.User.PostalCode = col["User.PostalCode"];
					h.User.StoreLocation = col["User.StoreLocation"];
					h.User.EmployeeNumber = col["User.EmployeeNumber"];
					h.User.AssociateTitle = col["User.AssociateTitle"];
					h.User.Phonenumber = col["User.Phonenumber"];
					h.User.Email = col["User.Email"];
					h.User.ConfirmEmail = col["User.ConfirmEmail"];
					h.User.UserID = col["User.UserID"];
					h.User.Password = col["User.Password"];
					return View(h);
				}
			}
			catch (Exception ex)
			{
				Models.SysLog.UpdateLogFile(this.ToString(), MethodBase.GetCurrentMethod().Name.ToString(), ex.Message);
				return RedirectToAction("Index", "Error");
			}
		}

		public ActionResult ResetPassword()
		{
			try
			{
				Models.Home h = new Models.Home();
				return View(h);
			}
			catch (Exception ex)
			{
				Models.SysLog.UpdateLogFile(this.ToString(), MethodBase.GetCurrentMethod().Name.ToString(), ex.Message);
				return RedirectToAction("Index", "Error");
			}
		}

		[HttpPost]
		public ActionResult ResetPassword(Models.User m, FormCollection col)
		{
			try
			{
				Models.Home h = new Models.Home();
				h.User.Password = col["User.NewPassword"];
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
