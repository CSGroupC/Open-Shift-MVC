﻿using System;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;

namespace Open_Shift.Models
{
	public class User
	{
		public long AssociateID = 0;

		[DisplayName("User ID")]
		public string UserID { get; set; }

		[DisplayName("First Name")]
		public string FirstName { get; set; }

		[DisplayName("Last Name")]
		public string LastName { get; set; }

		[DisplayName("Birthday")]
		public string Birthday { get; set; }

		[DisplayName("AddressLine1")]
		public string AddressLine1 { get; set; }

		[DisplayName("AddressLine2")]
		public string AddressLine2 { get; set; }

		[DisplayName("PostalCode")]
		public string PostalCode { get; set; }

		[DisplayName("StoreLocation")]
		public string StoreLocation { get; set; }

		[DisplayName("EmployeeNumber")]
		public string EmployeeNumber { get; set; }

		[DisplayName("AssociateTitle")]
		public string AssociateTitle { get; set; }

		[DisplayName("Phonenumber")]
		public string Phonenumber { get; set; }

		[DisplayName("E-Mail")]
		public string Email { get; set; }

		[DisplayName("ConfirmEmail")]
		public string ConfirmEmail { get; set; }

		[DisplayName("NewPassword")]
		public string NewPassword { get; set; }

		[DisplayName("Password")]
		public string Password { get; set; }

		public bool LoginAttempted = false;

		public bool LoginFailed
		{
			get
			{
				try
				{
					if (AssociateID == 0 && LoginAttempted == true) { return true; }
					return false;
				}
				catch (Exception ex)
				{
					SysLog.UpdateLogFile(this.ToString(), MethodBase.GetCurrentMethod().Name.ToString(), ex.Message);
					return false;
				}
			}
		}

		public bool IsAuthenticated
		{
			get
			{
				try
				{
					if (AssociateID > 0) return true;
					return false;
				}
				catch (Exception ex)
				{
					SysLog.UpdateLogFile(this.ToString(), MethodBase.GetCurrentMethod().Name.ToString(), ex.Message);
					return false;
				}
			}
		}

		public Image UserImage = new Image();

		public static List<User> GetUsers(long AssociateID = 0, byte StatusID = 0, byte PrivacyID = 0)
		{
			try
			{
				Database db = new Database();
				List<User> users = db.GetUsers(AssociateID, StatusID, PrivacyID);
				return users;
			}
			catch (Exception ex)
			{
				Models.SysLog.UpdateLogFile("User.cs", MethodBase.GetCurrentMethod().Name.ToString(), ex.Message);
				return null;
			}
		}

		public bool Login()
		{
			try
			{
				Database db = new Database();
				User newUser = new User();

				LoginAttempted = true;
				newUser = db.Login(this);
				if (newUser != null)
				{
					AssociateID = newUser.AssociateID;
					Password = newUser.Password;
					UserID = newUser.UserID;
					FirstName = newUser.FirstName;
					LastName = newUser.LastName;
					Email = newUser.Email;
					Birthday = newUser.Birthday;
					AddressLine1 = newUser.AddressLine1;
                    AddressLine2 = newUser.AddressLine2;
					PostalCode = newUser.PostalCode;
					StoreLocation = newUser.StoreLocation;
					EmployeeNumber = newUser.EmployeeNumber;
					AssociateTitle = newUser.AssociateTitle;
					Phonenumber = newUser.Phonenumber;
					return true;
				}
				else { return false; }
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

		//Not sure if this works
		public bool ResetPassword()
		{
		//try
		//	{
				User u = new User();
		//		u = (User)HttpContext.Current.Session["CurrentUser"];
		//		Database db = new Database();
		//		u.Password = db.ResetPassword(u.Password);
				return true;
		//	}
		//	catch (Exception ex) { throw new Exception(ex.Message); }
		}

		public bool RemoveUserSession()
		{
			try
			{
				HttpContext.Current.Session["CurrentUser"] = null;
				return true;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

		public User GetUserSession()
		{
			try
			{
				User u = new User();
				if (HttpContext.Current.Session["CurrentUser"] == null)
				{
					return u;
				}
				u = (User)HttpContext.Current.Session["CurrentUser"];
				Database db = new Database();
				u.UserImage = db.GetUserImage(u.AssociateID);
				return u;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

		public bool SaveUserSession()
		{
			try
			{
				this.UserImage = null; //don't save the user image
					HttpContext.Current.Session["CurrentUser"] = this;
				return true;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

		public bool Delete()
		{
			try
			{
				Database db = new Database();
				db.DeleteUser(this.AssociateID);
				RemoveUserSession();
				return true;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

		public bool Save()
		{
			try
			{
				Models.Database db = new Database();
				long NewAssociateID;
				if (AssociateID == 0)
				{
					NewAssociateID = db.InsertUser(this);
					if (NewAssociateID > 0) AssociateID = NewAssociateID;
				}
				else
				{
					db.UpdateUser(this);
				}

				return true;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

		public sbyte UpdatePrimaryImage()
		{
			try
			{
				Models.Database db = new Database();
				long NewAssociateID;
				if (this.UserImage.ImageID == 0)
				{
					NewAssociateID = db.InsertUserImage(this);
					if (NewAssociateID > 0) UserImage.ImageID = NewAssociateID;
				}
				else
				{
					db.UpdateUserImage(this);
				}
				return 0;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

		public User()
		{ //empty constructor
			UserID = string.Empty;
			Password = string.Empty;
			FirstName = string.Empty;
			LastName = string.Empty;
			Email = string.Empty;
		}
		public User(string UserID, string Password)
		{
			this.UserID = UserID;
			this.Password = Password;
			FirstName = string.Empty;
			LastName = string.Empty;
			Email = string.Empty;
			Birthday = string.Empty;
		    AddressLine1 =  string.Empty;
            AddressLine2 = string.Empty;
			PostalCode = string.Empty;
			StoreLocation = string.Empty;
			EmployeeNumber = string.Empty;
			AssociateTitle = string.Empty;
			Phonenumber = string.Empty;
			ConfirmEmail = string.Empty;
			this.NewPassword = NewPassword;

		}

		public User(string UserID, string Password, string FirstName, string LastName, string Birthday, string AddressLine1, string AddressLine2, string PostalCode,
			 String StoreLocation,string EmployeeNumber,string AssociateTitle,string Phonenumber, string Email, string ConfirmEmail )
		{
			this.UserID = UserID;
			this.Password = Password;
			this.FirstName = FirstName;
			this.LastName = LastName;
			this.Email = Email;
			this.Birthday = Birthday;
			this.AddressLine1 = AddressLine1;
            this.AddressLine2 = AddressLine2;
			this.PostalCode = PostalCode;
			this.StoreLocation = StoreLocation;
			this.EmployeeNumber = EmployeeNumber;
			this.AssociateTitle = AssociateTitle;
			this.Phonenumber = Phonenumber;
			this.ConfirmEmail = ConfirmEmail;
			this.NewPassword = NewPassword;




		}
	}
}