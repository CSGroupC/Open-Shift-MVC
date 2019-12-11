using System;
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
        // This MUST default to an invalid ID (0) in order for authentication to work
        public int AssociateID = 0;

        [DisplayName("First Name")]
        // NOTE: Names will have Japanese characters
        // [RegularExpression(@"^[A-Za-z]*$", ErrorMessage = "First Name Cannot Contain Numbers")]
        [Required(AllowEmptyStrings = false)]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        // NOTE: Names will have Japanese characters
        // [RegularExpression(@"^[A-Za-z]*$", ErrorMessage = "Last Name Cannot Contain Numbers")]
        [Required(AllowEmptyStrings = false)]
        public string LastName { get; set; }

        [DisplayName("Birthday")]
        [Required(AllowEmptyStrings = false)]
        [RegularExpression(@"^((19|20)\d{2})/((0|1)\d{1})/((0|1|2)\d{1})", ErrorMessage = "Error: Please format your birthday as 'YYYY/MM/DD'")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
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

        [DisplayName("Phone Number")]
        [RegularExpression(@"^[+]{1}[0-9]{11,13}$", ErrorMessage = "Error: Please format your phonenumber as '+' with country code and area code")]
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

        [DisplayName("New Password")]
        [Compare("Password", ErrorMessage = "Error: Your passwords do not match")]
        public string NewPassword { get; set; }

        [DisplayName("Associate Title")]
        [Required(AllowEmptyStrings = false)]
        public AssociateTitles AssociateTitle { get; set; } = AssociateTitles.NoType;

        public StatusList StatusID { get; set; } = StatusList.NoType;
        public StoreLocationList StoreID { get; set; } = StoreLocationList.NoType; //by default
        public IsManagerEnum IsManager { get; set; } = IsManagerEnum.Associate; //by default
        public bool LoginAttempted { get; set; } = false;

        public string EmailVerificationToken { get; set; } = ""; //**********THIS MUST DEFAULT TO EMPTY STRING!


        //public Image UserImage = new Image();

        public enum AssociateTitles
        {
            NoType = 0,
            Cook = 1,
            Server = 2,
            Owner = 3
        }

        public enum StatusList
        {
            NoType = 0,
            Active = 1,
            InActive = 2,
        }

        public enum StoreLocationList
        {
            NoType = 0,
            Kotetsu = 1,
            KotetsuLondon = 2,
            KotetsuBrazil = 3
        }

        // NOTE: Since this enum will be converted to a boolean, it is MUCH easier to use 1 and 0 as true and false
        public enum IsManagerEnum
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
                    IsManager = newUser.IsManager;
                    return true;
                }
                else { return false; }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        //Not sure if this works
        public bool ResetPassword()
        {
            try
            {
                User u = new User();
                u = (User)HttpContext.Current.Session["CurrentUser"];
                Database db = new Database();
                Password = u.NewPassword;
                db.UpdateUser(this);
                return true;
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

        public static User GetUserSession()
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
                //this.UserImage = null; //don't save the user image
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
            AddressLine1 = string.Empty;
            AddressLine2 = string.Empty;
            PostalCode = string.Empty;
            EmployeeNumber = null;
            Phonenumber = string.Empty;
            ConfirmEmail = string.Empty;


        }

        public User(string FirstName, string LastName, DateTime Birthday, string AddressLine1, string AddressLine2, string PostalCode,
            int EmployeeNumber, string Phonenumber, string Email, string ConfirmEmail, string Password, string EmailVerificationToken)
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
            this.Phonenumber = Phonenumber;
            this.ConfirmEmail = ConfirmEmail;

            this.EmailVerificationToken = EmailVerificationToken;




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

}