using System;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;

namespace Open_Shift.Models
{
	public class User
	{
		public long UID = 0;

		[DisplayName("User ID")]
		public string UserID { get; set; }

		[DisplayName("First Name")]
		public string FirstName { get; set; }

		[DisplayName("Last Name")]
		public string LastName { get; set; }

		[DisplayName("E-Mail")]
		public string Email { get; set; }

		[DisplayName("Password")]
		public string Password { get; set; }
		public bool LoginAttempted = false;
		public bool LoginFailed
		{
			get
			{
				try
				{
					if (UID == 0 && LoginAttempted == true) { return true; }
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
					if (UID > 0) return true;
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

		public static List<User> GetUsers(long UID = 0, byte StatusID = 0, byte PrivacyID = 0)
		{
			try
			{
				Database db = new Database();
				List<User> users = db.GetUsers(UID, StatusID, PrivacyID);
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
					UID = newUser.UID;
					Password = newUser.Password;
					UserID = newUser.UserID;
					FirstName = newUser.FirstName;
					LastName = newUser.LastName;
					Email = newUser.Email;
					return true;
				}
				else { return false; }
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
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
				u.UserImage = db.GetUserImage(u.UID);
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
				db.DeleteUser(this.UID);
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
				long NewUID;
				if (UID == 0)
				{
					NewUID = db.InsertUser(this);
					if (NewUID > 0) UID = NewUID;
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
				long NewUID;
				if (this.UserImage.ImageID == 0)
				{
					NewUID = db.InsertUserImage(this);
					if (NewUID > 0) UserImage.ImageID = NewUID;
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
		}

		public User(string UserID, string Password, string FirstName, string LastName, string Email)
		{
			this.UserID = UserID;
			this.Password = Password;
			this.FirstName = FirstName;
			this.LastName = LastName;
			this.Email = Email;
		}
	}
}