using System;
using System.Text;
using System.IO;
using System.Configuration;
using System.Web;

namespace Open_Shift.Models
{
	public class SysLog
	{
		public static bool UpdateLogFile(string ModuleName, string ProcessName, string Message)
		{
			try
			{
                string strPath = System.Web.HttpContext.Current.Server.MapPath("../App_Log");
                string strFileName = ConfigurationManager.AppSettings["SystemLogFileName"];
				string strPathAndFile = string.Concat(strPath, "\\", strFileName);


                if (!Directory.Exists(strPath))
				{ //folder doesn't not exist; get out
					return false;
				}

				if (!File.Exists(strPathAndFile))
				{ //the file doesn't not exist; create it
					using (FileStream fs = new FileStream(strPathAndFile, FileMode.Create))
					{
						//nothing else to do; object will close on its own
					}

					using (StreamWriter sw = new StreamWriter(strPathAndFile, true))
					{ //write the header line
						StringBuilder sb = new StringBuilder();
						sb.Append(string.Concat("Date-Time", "\t", "Application", "\t", "Message")); //header line
						sw.WriteLine(sb.ToString());
					}
				}
				using (StreamWriter sw = new StreamWriter(strPathAndFile, true))
				{
					StringBuilder sb = new StringBuilder();
					sb.Append(string.Concat(DateTime.Now.ToString(), "\t", ModuleName, "\t", Message)); //message
					sw.WriteLine(sb.ToString());
				}

				return true;
			}
			catch (Exception) { return false; }
		}
	}
}