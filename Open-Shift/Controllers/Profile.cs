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
                if (!Models.User.GetUserSession().IsAuthenticated)
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
                if (!Models.User.GetUserSession().IsAuthenticated)
                {
                    return RedirectToAction("SignIn");
                }
                Models.User u = new Models.User();
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
                if (!Models.User.GetUserSession().IsAuthenticated)
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
                if (!Models.User.GetUserSession().IsAuthenticated)
                {
                    return RedirectToAction("SignIn");
                }
                Models.User u = new Models.User();
                Models.Home h = new Models.Home();

                if (col["btnCancel"] == "cancel") return RedirectToAction("Index", "Profile");

                {
                    u = Models.User.GetUserSession();

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
                    if (u.IsAuthenticated && u.EmailVerificationToken == "")
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
                if (!Models.User.GetUserSession().IsAuthenticated)
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
                if (!Models.User.GetUserSession().IsAuthenticated)
                {
                    return RedirectToAction("SignIn");
                }
                Models.User u = new Models.User();
                if (col["btnSubmit"] == "cancel") return RedirectToAction("Index", "Profile");
                u = Models.User.GetUserSession();
                u.Delete();
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
        public ActionResult SignIn(Models.User m, FormCollection col)
        {
            try
            {
                Models.User u = new Models.User(col["User.Email"], col["User.Password"]);

                Database db = new Database();

                u = db.Login(u);
                u.LoginAttempted = true;

                if (u.IsAuthenticated && u.EmailVerificationToken == "")
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
                if (!Models.User.GetUserSession().IsAuthenticated)
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
                h.User.IsManager = (Models.User.IsManagerEnum)Enum.Parse(typeof(Models.User.IsManagerEnum), col["User.IsManager"]);
                h.User.StatusID = Models.User.StatusList.InActive;

                if (db.CheckIfUserExists(strEmail) == 1)
                {
                    // TODO: Tell the user that the email is already taken
                    return View(h);
                }

                h.User.Save();

                if (h.User.IsAuthenticated)
                { //user found

                    if (h.User.EmailVerificationToken != "")
                    {
                        return SignIn(h.User, col);
                    }

                    h.User.SaveUserSession(); //save the user session object

                    //Send text message to new user
                    Controllers.SmsController sms = new Controllers.SmsController();
                    // sms.SendSms(h.User.Phonenumber);

                    // //Send email to new user
                    //EmailController.NewAssociateEmail(h.User.Email, fullName);
                    EmailController.NewAssociateVerification(h.User.Email, h.User.FirstName, h.User.LastName, EmailVerificationToken);

                    return RedirectToAction("Index", "Home");
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
                Models.User u = new Models.User(col["User.Email"], col["User.Password"]);
                u.ResetPassword();

                if (u.IsAuthenticated && u.EmailVerificationToken == "")
                { //user found
                    u.SaveUserSession(); //save the user session object
                    return RedirectToAction("Index", "Home");
                }
                else
                { //user failed to log in
                    h.User = u;
                    return View(h);
                }
                u.Save();
                return View(h);

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
                Models.Home h = new Models.Home();
                Models.User u = new Models.User(col["User.Email"], col["User.Password"]);

                string PasswordVerificationToken = Guid.NewGuid().ToString();

                EmailController.NewPasswordRequest(h.User.Email, h.User.FirstName, h.User.LastName, PasswordVerificationToken);

                u.ResetPassword();

                if (u.IsAuthenticated)
                { //user found
                    u.SaveUserSession(); //save the user session object
                    return RedirectToAction("Index", "Home");
                }
                else
                { //user failed to log in
                    return RedirectToAction("ResetPassword", "Profile");
                }
                u.Save();
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
