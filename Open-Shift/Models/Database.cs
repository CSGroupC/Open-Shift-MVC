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
                        users.Add(new User(dr));
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



        public int CheckIfUserExists(string strEmail)
        {
            try
            {

                DataSet ds = new DataSet();
                SqlConnection cn = new SqlConnection();
                if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
                var da = new SqlDataAdapter("CHECK_USER_EXISTS", cn);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;

                int intReturnValue = -1;

                SetParameter(ref da, "@strEmail", strEmail, SqlDbType.NVarChar);

                try
                {
                    da.Fill(ds);
                }
                catch (Exception ex2)
                {
                    SysLog.UpdateLogFile(this.ToString(), MethodBase.GetCurrentMethod().Name.ToString(), ex2.Message);
                }
                finally { CloseDBConnection(ref cn); }

                if (ds.Tables[0].Rows.Count == 0)
                {
                    intReturnValue = 0;
                }
                else intReturnValue = 1;

                return intReturnValue;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }



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
                SetParameter(ref cm, "@blnIsManager", u.IsManager, SqlDbType.Bit);
                SetParameter(ref cm, "@strEmailVerificationToken", u.EmailVerificationToken, SqlDbType.NVarChar);

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
                        DataRow dr = ds.Tables[0].Rows[0];
                        newUser = new User(dr);

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
                SqlDataAdapter da = new SqlDataAdapter("UPDATE_PASSWORD", cn);
                DataSet ds;
                User newUser = null;

                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                SetParameter(ref da, "@intAssociateID", u.AssociateID, SqlDbType.NVarChar);
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

        public User getNewAssociateData(string token)
        {
            try
            {
                DataSet ds = new DataSet();
                SqlConnection cn = new SqlConnection();
                if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
                SqlDataAdapter da = null;

                da = new SqlDataAdapter("SELECT * FROM Tassociates where strEmailVerificationToken ='" + token + "';", cn);

                User UserInfo = null;

                da.SelectCommand.CommandType = CommandType.Text;

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
                        UserInfo = new User(dr);
                    }
                }

                return UserInfo;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

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

        // ====================================================================================================================================
        // Availabilities 
        // ====================================================================================================================================

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

                SetParameter(ref cm, "@intAvailabilityID", a.ID, SqlDbType.Int);
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
                DataSet ds = new DataSet();
                if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
                var da = new SqlDataAdapter("DELETE_AVAILABILITY", cn);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;

                SetParameter(ref da, "@intAvailabilityID", AvailabilityID, SqlDbType.Int);
           
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
                    return false;
                }

                else
                    return true;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public bool ApproveNewAssociate(string token)
        {
            string sqlStatement = "Update TAssociates set strEmailVerificationToken = '' where intAssociateID = (SELECT intAssociateID FROM TAssociates WHERE strEmailVerificationToken = '" + token + "');";

            try
            {
                SqlConnection cn = new SqlConnection();
                if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
                SqlCommand cm = new SqlCommand(sqlStatement, cn);

                try
                {
                    cm.ExecuteNonQuery();
                }
                finally { CloseDBConnection(ref cn); }

                return true;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }


            return true;
        }

        public bool ChangeAssocToActive(int AssociateID)
        {
            string sqlStatement = "Update TAssociates set intStatusID = 1 where intAssociateID = " + AssociateID + ";";

            try
            {
                SqlConnection cn = new SqlConnection();
                if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
                SqlCommand cm = new SqlCommand(sqlStatement, cn);

                try
                {
                    cm.ExecuteNonQuery();
                }
                finally { CloseDBConnection(ref cn); }

                return true;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }


            return true;
        }



        public List<User> GetManagersAndOwners()
        {
            try
            {
                DataSet ds = new DataSet();
                SqlConnection cn = new SqlConnection();
                if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
                SqlDataAdapter da = null;

                da = new SqlDataAdapter("Select * from tassociates where blnIsManager='True' and strEmailVerificationToken='';", cn);

                List<User> managers = new List<User>();

                da.SelectCommand.CommandType = CommandType.Text;

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
                        managers.Add(new User(dr));
                    }
                }

                return managers;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }




        // ====================================================================================================================================
        // Shifts 
        // ====================================================================================================================================


        public int InsertShift(Shift a)
        {
            try
            {
                int intShiftID = -1;
                DataSet ds = new DataSet();
                SqlConnection cn = new SqlConnection();
                if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
                var da = new SqlDataAdapter("CREATE_SHIFT", cn);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;

                SetParameter(ref da, "@intAssociateID", a.AssociateID, SqlDbType.Int);
                SetParameter(ref da, "@intStoreID", a.StoreID, SqlDbType.Int);
                SetParameter(ref da, "@dtmShiftDate", a.StartTime, SqlDbType.DateTime);
                SetParameter(ref da, "@dtmShiftBegin", a.StartTime, SqlDbType.DateTime);
                SetParameter(ref da, "@dtmShiftEnd", a.EndTime, SqlDbType.DateTime);
                SetParameter(ref da, "@strNotes", a.Notes, SqlDbType.NVarChar);
                //SetParameter(ref da, "@intAssociateTitleID", a.AssociateTitleID, SqlDbType.Int);
                SetParameter(ref da, "@blnIsOpen", a.IsOpen, SqlDbType.Bit);
                SetParameter(ref da, "@intAvailabilityID", a.AvailabilityID, SqlDbType.Int);

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
                        intShiftID = Convert.ToInt32(dr["intShiftID"].ToString());
                    }
                }

                if (intShiftID > 0)
                    return intShiftID;
                else
                    return 0;

            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }


        // TODO: Maybe change storeID to an int
        // Gets shifts by store, by year, by month, and optionally by associate
        public List<Shift> GetShifts(User.StoreLocationList storeID, int year, byte month, long associateID = 0)
        {
            try
            {
                DataSet ds = new DataSet();
                SqlConnection cn = new SqlConnection();
                if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
                SqlDataAdapter da = null;

                if (associateID == 0)
                {
                    da = new SqlDataAdapter("GET_SHIFT_STORE_MONTH_YEAR", cn);
                }
                else
                {
                    da = new SqlDataAdapter("GET_SHIFT_STORE_MONTH_YEAR", cn);
                }

                List<Shift> shifts = new List<Shift>();

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
                        shifts.Add(new Shift(dr));
                    }
                }

                return shifts;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        // Gets the next shift by associate
        public Shift GetNextShift(long associateID)
        {
            try
            {
                DataSet ds = new DataSet();
                SqlConnection cn = new SqlConnection();
                if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
                SqlDataAdapter da = new SqlDataAdapter("GET_NEXT_SHIFT", cn);


                Shift shift = null;

                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                SetParameter(ref da, "@intAssociateID", associateID, SqlDbType.Int);

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
                        shift = new Shift(dr);
                    }
                }

                return shift;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public bool DeleteShift(int ShiftID)
        {
            try
            {
                SqlConnection cn = new SqlConnection();
                if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
                SqlCommand cm = new SqlCommand("DELETE_SHIFT", cn);

                SetParameter(ref cm, "@intShiftID", ShiftID, SqlDbType.Int);

                try
                {
                    cm.ExecuteNonQuery();
                }
                finally { CloseDBConnection(ref cn); }

                return true;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }


        // ====================================================================================================================================
        // Stores 
        // ====================================================================================================================================


        public List<Store> GetStores(int AssociateID = 0)
        {
            try
            {
                DataSet ds = new DataSet();
                DataSet AssociatesDataSet = new DataSet();
                SqlConnection cn = new SqlConnection();
                if (!GetDBConnection(ref cn)) throw new Exception("Database did not connect");
                SqlDataAdapter da = null;

                if (AssociateID == 0)
                {
                    da = new SqlDataAdapter("GET_STORES", cn);
                }
                else
                {
                    da = new SqlDataAdapter("GET_STORES_BY_ASSOCIATE", cn);
                }

                List<Store> stores = new List<Store>();

                da.SelectCommand.CommandType = CommandType.StoredProcedure;

                if (AssociateID > 0) SetParameter(ref da, "@intAssociateID", AssociateID, SqlDbType.Int);

                try
                {
                    da.Fill(ds);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            stores.Add(new Store(dr));
                            da = new SqlDataAdapter("GET_ASSOCIATES_BY_STORE", cn);
                            da.SelectCommand.CommandType = CommandType.StoredProcedure;
                            SetParameter(ref da, "@intStoreID", stores[stores.Count - 1].StoreID, SqlDbType.Int);
                            da.Fill(AssociatesDataSet);
                            stores[stores.Count - 1].Associates = new List<User>();

                            foreach (DataRow AssociateDataRow in AssociatesDataSet.Tables[0].Rows)
                            {
                                stores[stores.Count - 1].Associates.Add(new User(AssociateDataRow));
                            }
                        }
                    }
                }
                catch (Exception ex2)
                {
                    SysLog.UpdateLogFile(this.ToString(), MethodBase.GetCurrentMethod().Name.ToString(), ex2.Message);
                }

                CloseDBConnection(ref cn);

                return stores;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }



        // ====================================================================================================================================
        // Database 
        // ====================================================================================================================================


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