﻿using System;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;
using System.ComponentModel.DataAnnotations;

namespace Open_Shift.Models
{
    //[MetadataType(typeof(UserMetaData))]
    public partial class User
	{
		public SystemLists Lists = new SystemLists();
		public int AssociateID = 1;

		[DisplayName("First Name")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z'\s]*$", ErrorMessage ="First Name Cannot Contain Numbers")]
        [Required(AllowEmptyStrings =false)]
        public string FirstName { get; set; }

		[DisplayName("Last Name")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z'\s]*$", ErrorMessage = "Last Name Cannot Contain Numbers")]
        [Required(AllowEmptyStrings = false)]
        public string LastName { get; set; }

		[DisplayName("Birthday")]
        [Required(AllowEmptyStrings = false)]
        [RegularExpression(@"^((19|20)\d{2})/((0|1)\d{1})/((0|1|2)\d{1})", ErrorMessage = "Error: Please format your birthday as 'YYYY/MM/DD'")]
        public DateTime? Birthday { get; set; }

		[DisplayName("Address Line One")]
        [Required(AllowEmptyStrings = false)]
        public string AddressLine1 { get; set; }

		[DisplayName("Address Line Two")]
        [Required(AllowEmptyStrings = false)]
        public string AddressLine2 { get; set; }

		[DisplayName("Postal Code")]
        [Required(AllowEmptyStrings = false)]
        public string PostalCode { get; set; }

		[DisplayName("Employee Number")]
        [Required(AllowEmptyStrings = false)]
        public int? EmployeeNumber { get; set; }

		[DisplayName("Associate Title")]
        [Required]
        public int? AssociateTitle { get; set; }

		[DisplayName("Phone Number")]
        [Required(AllowEmptyStrings = false)]
        public string Phonenumber { get; set; }

		[DisplayName("E-Mail")]
        [Required(AllowEmptyStrings = false)]
        [RegularExpression(@".+\@.+\..+", ErrorMessage = "Error: Your email isn't in the right format")]
        public string Email { get; set; }

		[DisplayName("Confirm Email")]
        [Compare("Email", ErrorMessage = "Error: Email does not match")]
        public string ConfirmEmail { get; set; }

        [DisplayName("Password")]
        [Required(AllowEmptyStrings = false)]
        public string Password { get; set; }

        [DisplayName("Confirm Password")]
        [Compare("Password", ErrorMessage = "Error: Your passwords do not match")]
        public string ConfirmPassword { get; set; }

		public StatusList StatusID = StatusList.Active;
		public StoreLocationList StoreID = StoreLocationList.NoType; //by default
        public IsManager blnIsManager = IsManager.Associate; //by default
        public bool LoginAttempted = false;

		public enum StatusList
		{
			NoType = 0,
			Active = 1,
			InActive = 2,
		}

		public enum StoreLocationList
		{
			NoType = 0,
			Kotetsu= 1,
			KotetsuLondon = 2,
			KotetsuBrazil = 3
		}

        public enum IsManager
        {
            Associate = 0,
            Manager = 1
        }

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

		public static List<User> GetUsers(long AssociateID = 0, byte StatusID = 0, byte StoreID = 0)
		{
			try
			{
				Database db = new Database();
				List<User> users = db.GetUsers(AssociateID, StatusID, StoreID);
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
					Email = newUser.Email;
					FirstName = newUser.FirstName;
					LastName = newUser.LastName;
					Birthday = newUser.Birthday;
					AddressLine1 = newUser.AddressLine1;
					AddressLine2 = newUser.AddressLine2;
					PostalCode = newUser.PostalCode;
					StoreID = newUser.StoreID;
					StatusID = newUser.StatusID;
					EmployeeNumber = newUser.EmployeeNumber;
					AssociateTitle = newUser.AssociateTitle;
					Phonenumber = newUser.Phonenumber;
					blnIsManager = newUser.blnIsManager;
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
				//User u = new User();
				//u = (User)HttpContext.Current.Session["CurrentUser"];
				//Database db = new Database();
				//u.Password = db.ResetPassword(u.Password);
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
				//u.UserImage = db.GetUserImage(u.AssociateID);
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
				int NewAssociateID;
				if (AssociateID == 1)
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

		//public sbyte UpdatePrimaryImage()
		//{
		//	try
		//	{
		//		Models.Database db = new Database();
		//		long NewAssociateID;
		//		if (this.UserImage.ImageID == 0)
		//		{
		//			NewAssociateID = db.InsertUserImage(this);
		//			if (NewAssociateID > 0) UserImage.ImageID = NewAssociateID;
		//		}
		//		else
		//		{
		//			db.UpdateUserImage(this);
		//		}
		//		return 0;
		//	}
		//	catch (Exception ex) { throw new Exception(ex.Message); }
		//}

		public User()
		{ //empty constructor
			Password = string.Empty;
			FirstName = string.Empty;
			LastName = string.Empty;
			Email = string.Empty;
			Birthday = null;
		}
		public User(string Email, string Password)
		{
			this.Email = Email;
			this.Password = Password;
			FirstName = string.Empty;
			LastName = string.Empty;
		    Birthday = null;
		    AddressLine1 =  string.Empty;
			AddressLine2 = string.Empty;
			PostalCode = string.Empty;
			EmployeeNumber = null;
			AssociateTitle = null;
			Phonenumber = string.Empty;
			ConfirmEmail = string.Empty;
		

		}

		public User( string FirstName, string LastName, DateTime Birthday, string AddressLine1, string AddressLine2,  string PostalCode,
			int EmployeeNumber,int AssociateTitle,string Phonenumber, string Email, string ConfirmEmail, string Password )
		{

			this.Password = Password;
			this.FirstName = FirstName;
			this.LastName = LastName;
			this.Email = Email;
			this.Birthday = Birthday;
			this.AddressLine1 = AddressLine1;
			this.AddressLine2 = AddressLine2;
			this.PostalCode = PostalCode;
			
			this.EmployeeNumber = EmployeeNumber;
			this.AssociateTitle = AssociateTitle;
			this.Phonenumber = Phonenumber;
			this.ConfirmEmail = ConfirmEmail;
			this.ConfirmPassword = Password;





		}

		//public User(string UserID, string Password, string FirstName, string LastName, DateTime Birthday, string AddressLine1, string AddressLine2, string PostalCode,
		//	 String StoreLocation, string EmployeeNumber, string AssociateTitle, string Phonenumber, string Email, string ConfirmEmail)
		//{
		//	this.UserID = UserID;
		//	this.Password = Password;
		//	this.FirstName = FirstName;
		//	this.LastName = LastName;
		//	this.Email = Email;
		//	this.Birthday = Birthday;
		//	this.AddressLine1 = AddressLine1;
		//	this.AddressLine2 = AddressLine2;
		//	this.PostalCode = PostalCode;
		//	this.StoreLocation = StoreLocation;
		//	this.EmployeeNumber = EmployeeNumber;
		//	this.AssociateTitle = AssociateTitle;
		//	this.Phonenumber = Phonenumber;
		//	this.ConfirmEmail = ConfirmEmail;
		//	this.NewPassword = NewPassword;
		//}
	}

    public class UserMetaData
    {
        [StringLength(10, MinimumLength =5)]
        public string FirstName { get; set; }
    }

}