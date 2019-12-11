using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Twilio.TwiML;
using Twilio.AspNet.Mvc;
using System.Net.Mail;
using System.Net;

namespace Open_Shift.Controllers
{
    public class EmailController : Controller
    {
        public static void NewAssociateEmail(string UserEmail, String Name)
        {
            string sub = "Welcome to OpenShift!";
            string body = "Thank you so much for signing up to <b>Open Shift</b>!";
            SendEmail(UserEmail, Name, sub, body);
        }

        public static void NewAssociateEmailManager(string UserEmail, String Name)
        {
            string sub = "OpenShift: A New User Signed Up!";
            string body = "click <a href='http://localhost:4040/Profile/' target='_blank'>here</a> to give your new user access!";
            SendEmail(UserEmail, Name, sub, body);
        }



        //this void sends all emails. ***Must specify UserEmail, Name, subject ( called "sub" ) and body in all voids above
        public static void SendEmail(string UserEmail, string Name, string sub, string body)
        {

            var senderEmail = new MailAddress(ConfigurationManager.AppSettings["OpenShiftEmail"], "OpenShift Support");
            var receiverEmail = new MailAddress(UserEmail, Name);
            var password = ConfigurationManager.AppSettings["EmailPassword"];
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(senderEmail.Address, password)
            };
            using (var mess = new MailMessage(senderEmail, receiverEmail)
            {
                Subject = sub,
                Body = body
            })
            {
                mess.IsBodyHtml = true;
                smtp.Send(mess);
            }
            //return View();
        }

    }

}