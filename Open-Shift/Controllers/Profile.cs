using Open_Shift.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Open_Shift.Controllers
{
    public class ProfileController : Controller
    {
        public ActionResult Index()
        {
            try
            {
                var u = Models.User.GetUserSession();
                if (!u.IsAuthenticated || u.EmailVerificationToken != "" || u.StatusID == Open_Shift.Models.User.StatusList.InActive)
                {
                    return RedirectToAction("SignIn");
                }
                Models.Home h = new Models.Home();
                h.User = Models.User.GetUserSession();

                //if (RouteData.Values["id"] != null)
                //{
                //	Models.Database db = new Models.Database();
                //	List<Models.User> users = new List<Models.User>();
                //	users = db.GetUsers(Convert.ToInt32(RouteData.Values["id"]));
                //	if (users.Count > 0)
                //	{
                //		h.ViewUser = users[0]; //get the first user
                //	}
                //}
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
                var u = Models.User.GetUserSession();
                if (!u.IsAuthenticated || u.EmailVerificationToken != "" || u.StatusID == Open_Shift.Models.User.StatusList.InActive)
                {
                    return RedirectToAction("SignIn");
                }
                Models.Home h = new Models.Home();

                if (col["btnSubmit"] == "delete") return RedirectToAction("Delete", "Profile");
                if (col["btnSubmit"] == "close") return RedirectToAction("Index", "Home");
                if (col["btnSubmit"] == "resetpassword") return RedirectToAction("ResetPassword", "Profile");
                if (col["btnSubmit"] == "update") return RedirectToAction("Update", "Profile");
                return View(u);
            }
            catch (Exception ex)
            {
                Models.SysLog.UpdateLogFile(this.ToString(), MethodBase.GetCurrentMethod().Name.ToString(), ex.Message);
                return RedirectToAction("Index", "Error");
            }
        }

        public ActionResult Update()
        {
            try
            {
                var u = Models.User.GetUserSession();
                if (!u.IsAuthenticated || u.EmailVerificationToken != "" || u.StatusID == Open_Shift.Models.User.StatusList.InActive)
                {
                    return RedirectToAction("SignIn");
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
        public ActionResult Update(HttpPostedFileBase UserImage, Models.User m, FormCollection col)
        {
            try
            {
                var u = Models.User.GetUserSession();
                if (!u.IsAuthenticated || u.EmailVerificationToken != "" || u.StatusID == Open_Shift.Models.User.StatusList.InActive)
                {
                    return RedirectToAction("SignIn");
                }
                Models.Home h = new Models.Home();

                if (col["btnCancel"] == "cancel") return RedirectToAction("Index", "Profile");

                u.FirstName = col["User.FirstName"];
                u.LastName = col["User.LastName"];
                u.Birthday = Convert.ToDateTime(col["User.Birthday"]);
                u.AddressLine1 = col["User.AddressLine1"];
                u.AddressLine2 = col["User.AddressLine2"];
                u.PostalCode = col["User.PostalCode"];
                u.EmployeeNumber = Convert.ToInt32(col["User.EmployeeNumber"]);
                u.AssociateTitle = (Models.User.AssociateTitles)Enum.Parse(typeof(Models.User.AssociateTitles), col["User.AssociateTitle"].ToString());
                u.Phonenumber = col["User.Phonenumber"];
                u.Email = col["User.Email"];
                u.ConfirmEmail = col["User.ConfirmEmail"];
                u.IsManager = (Models.User.IsManagerEnum)Enum.Parse(typeof(Models.User.IsManagerEnum), col["User.IsManager"]);
                u.StoreID = (Models.User.StoreLocationList)Enum.Parse(typeof(Models.User.StoreLocationList), col["User.StoreID"].ToString());

                //NEW CODE

                u.Save();
                if (u.IsAuthenticated && u.EmailVerificationToken == "" && u.StatusID == Models.User.StatusList.Active)
                { //user found
                  //if (UserImage != null)
                  //{
                  //	u.UserImage = new Models.Image();
                  //	u.UserImage.ImageID = Convert.ToInt32(col["User.UserImage.ImageID"]);
                  //	u.UserImage.Primary = true;
                  //	u.UserImage.FileName = Path.GetFileName(UserImage.FileName);
                  //	if (u.UserImage.IsImageFile())
                  //	{
                  //		u.UserImage.Size = UserImage.ContentLength;
                  //		Stream stream = UserImage.InputStream;
                  //		BinaryReader binaryReader = new BinaryReader(stream);
                  //		u.UserImage.ImageData = binaryReader.ReadBytes((int)stream.Length);
                  //		u.UpdatePrimaryImage();
                  //	}
                  //}
                    u.SaveUserSession(); //save the user session object
                    return RedirectToAction("Index", "Profile");
                }
                else
                { //user failed to log in
                    h.User.FirstName = col["User.FirstName"];
                    h.User.LastName = col["User.LastName"];
                    h.User.Birthday = Convert.ToDateTime(col["User.Birthday"]);
                    h.User.AddressLine1 = col["User.AddressLine1"];
                    h.User.AddressLine2 = col["User.AddressLine2"];
                    h.User.PostalCode = col["User.PostalCode"];

                    h.User.EmployeeNumber = Convert.ToInt32(col["User.EmployeeNumber"]);
                    h.User.AssociateTitle = (Models.User.AssociateTitles)Enum.Parse(typeof(Models.User.AssociateTitles), col["User.AssociateTitle"].ToString());
                    h.User.Phonenumber = col["User.Phonenumber"];
                    h.User.Email = col["User.Email"];
                    h.User.ConfirmEmail = col["User.ConfirmEmail"];
                    h.User.Password = col["User.Password"];
                }

                return View(u);
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
                var u = Models.User.GetUserSession();
                if (!u.IsAuthenticated || u.EmailVerificationToken != "" || u.StatusID == Open_Shift.Models.User.StatusList.InActive)
                {
                    return RedirectToAction("SignIn");
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
        public ActionResult Delete(Models.User m, FormCollection col)
        {
            try
            {
                var u = Models.User.GetUserSession();
                if (!u.IsAuthenticated || u.EmailVerificationToken != "" || u.StatusID == Open_Shift.Models.User.StatusList.InActive)
                {
                    return RedirectToAction("SignIn");
                }

                if (col["btnSubmit"] == "cancel") return RedirectToAction("Index", "Profile");
                u.Delete();
                u.RemoveUserSession();
                return RedirectToAction("SignUp", "Profile");
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
                var u = Models.User.GetUserSession();
                if (u.IsAuthenticated && u.EmailVerificationToken == "" && u.StatusID == Open_Shift.Models.User.StatusList.Active)
                {
                    return RedirectToAction("Index", "Home");
                }
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
                Models.User u = new Models.User(col["User.Email"], col["User.Password"]);

                Database db = new Database();

                u = db.Login(u);
                u.LoginAttempted = true;

                if (u.IsAuthenticated && u.EmailVerificationToken == "" && u.StatusID == Models.User.StatusList.Active)
                { //user found

                    u.SaveUserSession(); //save the user session object
                    return RedirectToAction("Index", "Home");
                }
                else
                { //user failed to log in
                    Models.Home h = new Models.Home();
                    h.User = u;
                    // NOTE: This is required, in case we get to this line from the SignUp action
                    return View("~/Views/Profile/SignIn.cshtml", h);
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
                var u = Models.User.GetUserSession();
                if (!u.IsAuthenticated || u.EmailVerificationToken != "" || u.StatusID == Open_Shift.Models.User.StatusList.InActive)
                {
                    return RedirectToAction("Index", "Home");
                }
                Models.Home h = new Models.Home();
                h.User = Models.User.GetUserSession();
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

                if (Models.User.GetUserSession().IsAuthenticated)
                {
                    return RedirectToAction("Index", "Home");
                }
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
                if (Models.User.GetUserSession().IsAuthenticated)
                {
                    return RedirectToAction("Index", "Home");
                }

                Database db = new Database();

                string strEmail = col["User.Email"];

                Models.Home h = new Models.Home();

                string EmailVerificationToken = Guid.NewGuid().ToString();

                h.User = new Models.User(col["User.FirstName"], col["User.LastName"], Convert.ToDateTime(col["User.Birthday"]),
                                                col["User.AddressLine1"], col["User.AddressLine2"], col["User.PostalCode"],
                                               Convert.ToInt32(col["User.EmployeeNumber"]),
                                          col["User.PhoneNumber"], col["User.Email"], col["User.ConfirmEmail"], col["User.Password"], EmailVerificationToken);

                h.User.AssociateTitle = (Models.User.AssociateTitles)Enum.Parse(typeof(Models.User.AssociateTitles), col["User.AssociateTitle"]);
                h.User.StoreID = (Models.User.StoreLocationList)Enum.Parse(typeof(Models.User.StoreLocationList), col["User.StoreID"]);
                h.User.StatusID = Models.User.StatusList.InActive;

                if (db.CheckIfUserExists(strEmail) == 1)
                {
                    h.User.EmailTaken = true;
                    return View(h);
                }

                h.User.Save();

                if (h.User.IsAuthenticated)
                { //user found
                    //Send text message to new user
                    Controllers.SmsController sms = new Controllers.SmsController();
                    // sms.SendSms(h.User.Phonenumber);

                    // //Send email to new user
                    //EmailController.NewAssociateEmail(h.User.Email, fullName);
                    EmailController.NewAssociateVerification(h.User.Email, h.User.FirstName, h.User.LastName, EmailVerificationToken);

                    if (h.User.EmailVerificationToken != "" || h.User.StatusID == Models.User.StatusList.InActive)
                    {
                        return SignIn(h.User, col);
                    }

                    return RedirectToAction("Index", "Home");

                    h.User.SaveUserSession(); //save the user session object
                }
                else
                { //user failed to log in

                    return View(h);
                }
            }
            catch (Exception ex)
            {
                Models.SysLog.UpdateLogFile(this.ToString(), MethodBase.GetCurrentMethod().Name.ToString(), ex.Message);
                return RedirectToAction("Index", "Error");
            }
        }

        public ActionResult ResetPassword(string token)
        {
            try
            {
                Models.Database db = new Database();

                Home h = new Home();
                h.User = db.GetUserByPasswordResetToken(token);

                return View(h);
            }
            catch (Exception ex)
            {
                Models.SysLog.UpdateLogFile(this.ToString(), MethodBase.GetCurrentMethod().Name.ToString(), ex.Message);
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        public ActionResult ResetPassword(FormCollection col)
        {
            try
            {
                Models.Home h = new Models.Home();
                h.User = new Models.User(col["User.Email"], col["User.Password"]);
                h.User.PasswordResetToken = col["User.PasswordResetToken"];
                if (!h.User.ResetPassword())
                {
                    // TODO: Handle this error
                }

                return View("~/Views/Profile/SignIn.cshtml", h);
            }
            catch (Exception ex)
            {
                Models.SysLog.UpdateLogFile(this.ToString(), MethodBase.GetCurrentMethod().Name.ToString(), ex.Message);
                return RedirectToAction("Index", "Error");
            }
        }




        public ActionResult ResetPasswordRequest()
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
        public ActionResult ResetPasswordRequest(Models.User m, FormCollection col)
        {
            try
            {
                string PasswordResetToken = Guid.NewGuid().ToString();

                Models.Database db = new Database();

                db.SetUserPasswordResetToken(col["User.Email"], PasswordResetToken);

                EmailController.NewPasswordRequest(col["User.Email"], PasswordResetToken);

                return RedirectToAction("SignIn", "Profile");

            }
            catch (Exception ex)
            {
                Models.SysLog.UpdateLogFile(this.ToString(), MethodBase.GetCurrentMethod().Name.ToString(), ex.Message);
                return RedirectToAction("Index", "Error");
            }
        }



    }
}
