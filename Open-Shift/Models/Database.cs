using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace Open_Shift.Models
{
    //**TODO needs to change the table names and columns to match our database
    public class Database
    {
        private User AddUser(DataRow dr)
        {
            try
            {
                User u = new User();
                u.AssociateID = (int)dr["AssociateID"];
                u.FirstName = dr["User.FirstName"].ToString();
                u.LastName = dr["User.LastName"].ToString();
                u.Birthday = (DateTime)dr["User.Birthday"];
                u.AddressLine1 = dr["User.AddressLine1"].ToString();
                u.AddressLine2 = dr["User.AddressLine2"].ToString();
                u.PostalCode = dr["User.PostalCode"].ToString();
                u.EmployeeNumber = (int)dr["User.EmployeeNumber"];
                u.AssociateTitle = (User.AssociateTitles)Enum.Parse(typeof(User.AssociateTitles), dr["User.AssociateTitle"].ToString());
                u.Phonenumber = dr["User.Phonenumber"].ToString();
                u.Email = dr["User.Email"].ToString();
                u.ConfirmEmail = dr["User.ConfirmEmail"].ToString();
                u.Password = dr["User.Password"].ToString();
                u.IsManager = (User.IsManagerEnum)Enum.Parse(typeof(User.IsManagerEnum), dr["User.blnIsManager"].ToString());
                u.StatusID = (User.StatusList)Enum.Parse(typeof(User.StatusList), dr["User.intStatusID"].ToString());
                u.StoreID = (User.StoreLocationList)Enum.Parse(typeof(User.StoreLocationList), dr["User.intStoreID"].ToString());


                //u.UserImage = GetUserImage(u.AssociateID);

                return u;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public List<User> GetUsers(long AssociateID = 0, byte StatusID = 0, byte PrivacyID = 0)
        {
            try
            {
                DataSet ds = new DataSet();
                SqlConnection cn = new SqlConnection();
                if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
                SqlDataAdapter da = new SqlDataAdapter("SELECT_USERS", cn);
                List<User> users = new List<User>();

                da.SelectCommand.CommandType = CommandType.StoredProcedure;

                if (AssociateID > 0) SetParameter(ref da, "@AssociateID", AssociateID, SqlDbType.BigInt);
                try
                {
                    da.Fill(ds);
                }
                catch (Exception ex2)
                {
                    SysLog.UpdateLogFile(this.ToString(), MethodBase.GetCurrentMethod().Name.ToString(), ex2.Message);
                }
                finally { CloseDBConnection(ref cn); }

                if (ds.Tables[0].Rows.Count != 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        users.Add(AddUser(dr));
                    }
                }

                return users;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
        public bool DeleteUser(long AssociateID)
        {
            try
            {
                SqlConnection cn = new SqlConnection();
                if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
                SqlCommand cm = new SqlCommand("DELETE_USER", cn);

                SetParameter(ref cm, "@intAssociateID", AssociateID, SqlDbType.Int);

                try
                {
                    cm.ExecuteNonQuery();
                }
                finally { CloseDBConnection(ref cn); }

                return true;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        //public Image GetUserImage(long AssociateID = 0, long UserImageID = 0)
        //{
        //	try
        //	{
        //		DataSet ds = new DataSet();
        //		SqlConnection cn = new SqlConnection();
        //		if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
        //		SqlDataAdapter da = new SqlDataAdapter("SELECT_USER_IMAGES", cn);
        //		Image img = new Image();

        //		da.SelectCommand.CommandType = CommandType.StoredProcedure;

        //		if (AssociateID > 0) SetParameter(ref da, "@AssociateID", AssociateID, SqlDbType.Int);
        //		if (UserImageID > 0) SetParameter(ref da, "@user_image_id", UserImageID, SqlDbType.Int);

        //		try
        //		{
        //			da.Fill(ds);
        //		}
        //		catch (Exception ex2)
        //		{
        //			SysLog.UpdateLogFile(this.ToString(), MethodBase.GetCurrentMethod().Name.ToString(), ex2.Message);
        //		}
        //		finally { CloseDBConnection(ref cn); }

        //		if (ds.Tables[0].Rows.Count != 0)
        //		{
        //			DataRow dr = ds.Tables[0].Rows[0];
        //			img.ImageID = (long)dr["UserImageID"];
        //			img.ImageData = (byte[])dr["Image"];
        //			img.FileName = (string)dr["FileName"];
        //			img.Size = (long)dr["ImageSize"];
        //			if (dr["PrimaryImage"].ToString() == "Y")
        //				img.Primary = true;
        //			else
        //				img.Primary = false;
        //		}
        //		return img;
        //	}
        //	catch (Exception ex) { throw new Exception(ex.Message); }
        //}



        //public long InsertUserImage(User u)
        //{
        //try
        //{
        //	SqlConnection cn = null;
        //	if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
        //	SqlCommand cm = new SqlCommand("INSERT_USER_IMAGE", cn);

        //	SetParameter(ref cm, "@user_image_id", null, SqlDbType.BigInt, Direction: ParameterDirection.Output);
        //	SetParameter(ref cm, "@AssociateID", u.AssociateID, SqlDbType.Int);
        //	if (u.UserImage.Primary)
        //		SetParameter(ref cm, "@primary_image", "Y", SqlDbType.Char);
        //	else
        //		SetParameter(ref cm, "@primary_image", "N", SqlDbType.Char);

        //	SetParameter(ref cm, "@image", u.UserImage.ImageData, SqlDbType.VarBinary);
        //	SetParameter(ref cm, "@file_name", u.UserImage.FileName, SqlDbType.NVarChar);
        //	SetParameter(ref cm, "@image_size", u.UserImage.Size, SqlDbType.BigInt);

        //	cm.ExecuteReader();
        //	CloseDBConnection(ref cn);
        //	return (long)cm.Parameters["@user_image_id"].Value;
        //}
        //catch (Exception ex)
        //{
        //	throw new Exception(string.Concat(this.ToString(), ".", MethodBase.GetCurrentMethod().Name.ToString(), "(): ", ex.Message));
        //}
        //}

        public int InsertUser(User u)
        {
            try
            {
                SqlConnection cn = null;
                if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
                SqlCommand cm = new SqlCommand("INSERT_USER", cn);
                int intReturnValue = -1;
                int intAssociateID = -1;

                SetParameter(ref cm, "@intAssociateID", u.AssociateID, SqlDbType.Int, Direction: ParameterDirection.Output);


                SetParameter(ref cm, "@strFirstName", u.FirstName, SqlDbType.NVarChar);
                SetParameter(ref cm, "@strLastName", u.LastName, SqlDbType.NVarChar);
                SetParameter(ref cm, "@strPostalCode", u.PostalCode, SqlDbType.NVarChar);
                SetParameter(ref cm, "@strAddressLine1", u.AddressLine1, SqlDbType.NVarChar);
                SetParameter(ref cm, "@strAddressLine2", u.AddressLine2, SqlDbType.NVarChar);
                SetParameter(ref cm, "@strPhoneNumber", u.Phonenumber, SqlDbType.NVarChar);
                SetParameter(ref cm, "@dtmBirthdate", u.Birthday, SqlDbType.DateTime);
                SetParameter(ref cm, "@strEmail", u.Email, SqlDbType.NVarChar);
                SetParameter(ref cm, "@intEmployeeNumber", u.EmployeeNumber, SqlDbType.Int);
                SetParameter(ref cm, "@intAssociateTitleID", u.AssociateTitle, SqlDbType.Int);
                SetParameter(ref cm, "@strPassword", u.Password, SqlDbType.NVarChar);
                SetParameter(ref cm, "@intStatusID", u.StatusID, SqlDbType.Int);
                SetParameter(ref cm, "@intStoreID", u.StoreID, SqlDbType.Int);
                SetParameter(ref cm, "@blnIsManager", u.IsManager, SqlDbType.Int);


                cm.ExecuteReader();

                intAssociateID = (int)cm.Parameters["@intAssociateID"].Value;

                CloseDBConnection(ref cn);

                if (intAssociateID > 0)
                { intReturnValue = 1; }
                else intReturnValue = 0;

                switch (intReturnValue)
                {
                    case 1: //new user created
                        return intAssociateID;
                    default:
                        return 0;
                }

            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public Models.User Login(Models.User u)
        {
            try
            {
                SqlConnection cn = new SqlConnection();
                if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
                SqlDataAdapter da = new SqlDataAdapter("LOGIN", cn);
                DataSet ds;
                User newUser = null;

                da.SelectCommand.CommandType = CommandType.StoredProcedure;

                SetParameter(ref da, "@strEmail", u.Email, SqlDbType.NVarChar);
                SetParameter(ref da, "@strPassword", u.Password, SqlDbType.NVarChar);

                try
                {
                    ds = new DataSet();
                    da.Fill(ds);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        newUser = new User();
                        DataRow dr = ds.Tables[0].Rows[0];
                        newUser.Password = u.Password;
                        newUser.FirstName = (string)dr["strFirstName"];
                        newUser.LastName = (string)dr["strLastName"];
                        newUser.Birthday = (DateTime)dr["dtmBirthdate"];
                        newUser.AddressLine1 = (string)dr["strAddressLine1"];
                        newUser.AddressLine2 = (string)dr["strAddressLine2"];
                        newUser.PostalCode = (string)dr["strPostalCode"];

                        newUser.EmployeeNumber = (int)dr["intEmployeeNumber"];
                        newUser.AssociateTitle = (User.AssociateTitles)Enum.Parse(typeof(User.AssociateTitles), dr["intAssociateTitleID"].ToString());
                        newUser.Phonenumber = (string)dr["strPhonenumber"];
                        newUser.Email = u.Email;
                        //	newUser.ConfirmEmail = (string)dr["strConfirmEmail"];
                        var managerStatus = "Associate";
                        if (dr["blnIsManager"].ToString() == "True") managerStatus = "Manager";

                        //newUser.blnIsManager = 1;
                        newUser.IsManager = (User.IsManagerEnum)Enum.Parse(typeof(User.IsManagerEnum), managerStatus);
                        newUser.StatusID = (User.StatusList)Convert.ToInt32(dr["intStatusID"].ToString());
                        newUser.StoreID = (User.StoreLocationList)Convert.ToInt32(dr["intStoreID"].ToString());

                        // NOTE: Do this last, so authentication will fail if the above fails
                        newUser.AssociateID = (int)dr["intAssociateID"];
                    }
                }
                catch (Exception ex2)
                {
                    SysLog.UpdateLogFile(this.ToString(), MethodBase.GetCurrentMethod().Name.ToString(), ex2.Message);
                }
                finally
                {
                    CloseDBConnection(ref cn);
                }
                return newUser; //alls well in the world
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        //Not sure if this will work, so
        public Models.User ResetPassword(User u)
        {
            try
            {
                SqlConnection cn = new SqlConnection();
                if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
                SqlDataAdapter da = new SqlDataAdapter("ResetPassword", cn);
                DataSet ds;
                User newUser = null;

                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                SetParameter(ref da, "@strEmail", u.Email, SqlDbType.NVarChar);
                SetParameter(ref da, "@strPassword", u.Password, SqlDbType.NVarChar);


                try
                {
                    ds = new DataSet();
                    da.Fill(ds);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        newUser = new User();
                        DataRow dr = ds.Tables[0].Rows[0];
                        newUser.AssociateID = (int)dr["AssociateID"];
                        newUser.Email = u.Email;
                        newUser.Password = u.Password;
                    }
                }
                catch (Exception ex2)
                {
                    SysLog.UpdateLogFile(this.ToString(), MethodBase.GetCurrentMethod().Name.ToString(), ex2.Message);
                }
                finally
                {
                    CloseDBConnection(ref cn);
                }
                return newUser; //alls well in the world
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        /*
        public long UpdateUserImage(User u)
        {
            try
            {
                SqlConnection cn = null;
                if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
                SqlCommand cm = new SqlCommand("UPDATE_USER_IMAGE", cn);

                SetParameter(ref cm, "@user_image_id", u.UserImage.ImageID, SqlDbType.BigInt);
                if (u.UserImage.Primary)
                    SetParameter(ref cm, "@primary_image", "Y", SqlDbType.Char);
                else
                    SetParameter(ref cm, "@primary_image", "N", SqlDbType.Char);

                SetParameter(ref cm, "@image", u.UserImage.ImageData, SqlDbType.VarBinary);
                SetParameter(ref cm, "@file_name", u.UserImage.FileName, SqlDbType.NVarChar);
                SetParameter(ref cm, "@image_size", u.UserImage.Size, SqlDbType.BigInt);

                cm.ExecuteReader();
                CloseDBConnection(ref cn);

                return 0; //success	
            }
            catch (Exception ex)
            {
                throw new Exception(string.Concat(this.ToString(), ".", MethodBase.GetCurrentMethod().Name.ToString(), "(): ", ex.Message));
            }
        }
        */

        public bool UpdateUser(User u)
        {
            try
            {
                SqlConnection cn = null;
                if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
                SqlCommand cm = new SqlCommand("UPDATE_USER", cn);
                int intReturnValue = -1;

                SetParameter(ref cm, "@intAssociateID", u.AssociateID, SqlDbType.Int);

                SetParameter(ref cm, "@strFirstName", u.FirstName, SqlDbType.NVarChar);
                SetParameter(ref cm, "@strLastName", u.LastName, SqlDbType.NVarChar);
                SetParameter(ref cm, "@strPostalCode", u.PostalCode, SqlDbType.NVarChar);
                SetParameter(ref cm, "@strAddressLine1", u.AddressLine1, SqlDbType.NVarChar);
                SetParameter(ref cm, "@strAddressLine2", u.AddressLine2, SqlDbType.NVarChar);
                SetParameter(ref cm, "@strPhoneNumber", u.Phonenumber, SqlDbType.NVarChar);
                SetParameter(ref cm, "@dtmBirthdate", u.Birthday, SqlDbType.Date);
                SetParameter(ref cm, "@intEmployeeNumber", u.EmployeeNumber, SqlDbType.Int);
                SetParameter(ref cm, "@intAssociateTitleID", u.AssociateTitle, SqlDbType.Int);
                SetParameter(ref cm, "@blnIsManager", u.IsManager, SqlDbType.Bit);
                SetParameter(ref cm, "@intStatusID", u.StatusID, SqlDbType.TinyInt);
                SetParameter(ref cm, "@intStoreID", u.StoreID, SqlDbType.TinyInt);

                SetParameter(ref cm, "ReturnValue", 0, SqlDbType.Int, Direction: ParameterDirection.ReturnValue);

                cm.ExecuteReader();

                intReturnValue = (int)cm.Parameters["ReturnValue"].Value;
                CloseDBConnection(ref cn);

                switch (intReturnValue)
                {
                    case 1: //new user created
                        return true;
                    default:
                        return false;
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }


        public int InsertAvailability(Availability a)
        {
            try
            {
                int intAvailabilityID = -1;
                DataSet ds = new DataSet();
                SqlConnection cn = new SqlConnection();
                if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
                var da = new SqlDataAdapter("CREATE_AVAILABILITY", cn);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;

                SetParameter(ref da, "@intAssociateID", a.AssociateID, SqlDbType.Int);
                SetParameter(ref da, "@dtmBeginAvailability", a.StartTime, SqlDbType.DateTime);
                SetParameter(ref da, "@dtmEndAvailability", a.EndTime, SqlDbType.DateTime);
                SetParameter(ref da, "@strNotes", a.Notes, SqlDbType.NVarChar);

                try
                {
                    da.Fill(ds);
                }
                catch (Exception ex2)
                {
                    SysLog.UpdateLogFile(this.ToString(), MethodBase.GetCurrentMethod().Name.ToString(), ex2.Message);
                }
                finally { CloseDBConnection(ref cn); }

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        // NOTE: Just casting to int with (int) is not enough apparently
                        intAvailabilityID = Convert.ToInt32(dr["intAvailabilityID"].ToString());
                    }
                }

                if (intAvailabilityID > 0)
                    return intAvailabilityID;
                else
                    return 0;

            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        // TODO: Maybe change storeID to an int
        // Gets availabilities by store, by year, by month, and optionally by associate
        public List<Availability> GetAvailabilities(User.StoreLocationList storeID, int year, byte month, long associateID = 0)
        {
            try
            {
                DataSet ds = new DataSet();
                SqlConnection cn = new SqlConnection();
                if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
                SqlDataAdapter da = null;

                if (associateID == 0)
                {
                    da = new SqlDataAdapter("GET_AVAILABILITIES_MONTHLY", cn);
                }
                else
                {
                    da = new SqlDataAdapter("GET_AVAILABILITIES_MONTHLY_BY_ASSOCIATE", cn);
                }

                List<Availability> availabilities = new List<Availability>();

                da.SelectCommand.CommandType = CommandType.StoredProcedure;

                SetParameter(ref da, "@intStoreID", storeID, SqlDbType.Int);
                SetParameter(ref da, "@intYear", year, SqlDbType.Int);
                SetParameter(ref da, "@intMonth", month, SqlDbType.Int);
                if (associateID > 0) SetParameter(ref da, "@intAssociateID", associateID, SqlDbType.Int);

                try
                {
                    da.Fill(ds);
                }
                catch (Exception ex2)
                {
                    SysLog.UpdateLogFile(this.ToString(), MethodBase.GetCurrentMethod().Name.ToString(), ex2.Message);
                }
                finally { CloseDBConnection(ref cn); }

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        availabilities.Add(new Availability(dr));
                    }
                }

                return availabilities;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }


        public bool UpdateAvailability(Availability a)
        {
            try
            {
                SqlConnection cn = null;
                if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
                SqlCommand cm = new SqlCommand("UPDATE_AVAILABILITY", cn);

                SetParameter(ref cm, "@intAvailabilityID", a.AssociateID, SqlDbType.Int);
                SetParameter(ref cm, "@intAssociateID", a.AssociateID, SqlDbType.Int);
                SetParameter(ref cm, "@dtmBeginAvailability", a.StartTime, SqlDbType.DateTime);
                SetParameter(ref cm, "@dtmEndAvailability", a.EndTime, SqlDbType.DateTime);
                SetParameter(ref cm, "@strNotes", a.Notes, SqlDbType.NVarChar);

                SetParameter(ref cm, "ReturnValue", 0, SqlDbType.Int, Direction: ParameterDirection.ReturnValue);

                cm.ExecuteReader();

                int intReturnValue = (int)cm.Parameters["ReturnValue"].Value;
                CloseDBConnection(ref cn);

                switch (intReturnValue)
                {
                    case 1: //new availability created
                        return true;
                    default:
                        return false;
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }


        public bool DeleteAvailability(int AvailabilityID)
        {
            try
            {
                SqlConnection cn = new SqlConnection();
                if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
                SqlCommand cm = new SqlCommand("DELETE_AVAILABILITY", cn);

                SetParameter(ref cm, "@intAvailabilityID", AvailabilityID, SqlDbType.Int);

                try
                {
                    cm.ExecuteNonQuery();
                }
                finally { CloseDBConnection(ref cn); }

                return true;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        private bool GetDBConnection(ref SqlConnection SQLConn)
        {
            try
            {
                if (SQLConn == null) SQLConn = new SqlConnection();
                if (SQLConn.State != ConnectionState.Open)
                {
                    SQLConn.ConnectionString = ConfigurationManager.ConnectionStrings["OpenShift"].ConnectionString;
                    SQLConn.Open();
                }
                return true;
            }
            catch (Exception ex)
            { throw new Exception(ex.Message); }
        }

        private bool CloseDBConnection(ref SqlConnection SQLConn)
        {
            try
            {
                if (SQLConn.State != ConnectionState.Closed)
                {
                    SQLConn.Close();
                    SQLConn.Dispose();
                    SQLConn = null;
                }
                return true;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        private int SetParameter(ref SqlCommand cm, string ParameterName, Object Value
            , SqlDbType ParameterType, int FieldSize = -1
            , ParameterDirection Direction = ParameterDirection.Input
            , Byte Precision = 0, Byte Scale = 0)
        {
            try
            {
                cm.CommandType = CommandType.StoredProcedure;
                if (FieldSize == -1)
                    cm.Parameters.Add(ParameterName, ParameterType);
                else
                    cm.Parameters.Add(ParameterName, ParameterType, FieldSize);

                if (Precision > 0) cm.Parameters[cm.Parameters.Count - 1].Precision = Precision;
                if (Scale > 0) cm.Parameters[cm.Parameters.Count - 1].Scale = Scale;

                cm.Parameters[cm.Parameters.Count - 1].Value = Value;
                cm.Parameters[cm.Parameters.Count - 1].Direction = Direction;

                return 0;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        private int SetParameter(ref SqlDataAdapter cm, string ParameterName, Object Value
            , SqlDbType ParameterType, int FieldSize = -1
            , ParameterDirection Direction = ParameterDirection.Input
            , Byte Precision = 0, Byte Scale = 0)
        {
            try
            {
                cm.SelectCommand.CommandType = CommandType.StoredProcedure;
                if (FieldSize == -1)
                    cm.SelectCommand.Parameters.Add(ParameterName, ParameterType);
                else
                    cm.SelectCommand.Parameters.Add(ParameterName, ParameterType, FieldSize);

                if (Precision > 0) cm.SelectCommand.Parameters[cm.SelectCommand.Parameters.Count - 1].Precision = Precision;
                if (Scale > 0) cm.SelectCommand.Parameters[cm.SelectCommand.Parameters.Count - 1].Scale = Scale;

                cm.SelectCommand.Parameters[cm.SelectCommand.Parameters.Count - 1].Value = Value;
                cm.SelectCommand.Parameters[cm.SelectCommand.Parameters.Count - 1].Direction = Direction;

                return 0;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }

}