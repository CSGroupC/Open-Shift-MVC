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
        public static void NewAssociateEmail(string UserEmail, string FirstName, string LastName)
        {
            string sub = "Welcome to OpenShift!";
            string body = "Thank you so much for signing up to <b>Open Shift</b>!";
            SendEmail(UserEmail, FirstName, LastName, sub, body);
        }

        public static void NewAssociateVerification(string UserEmail, string FirstName, string LastName, string EmailVerificationToken)
        {
            string sub = "Welcome To OpenShift! Please Verify Your Email";
            string body = "Hi " + FirstName + "! <br><br> Thanks so much for signing up for OpenShift!" +
                " Please click <a href='http://localhost:4040/Verification/EmailVerification/?token=" + EmailVerificationToken + "' target='_blank'><b>here</b></a> to verify your email. " +
                "After you verify your email, your manager will approve your access into OpenShift. We look forward to helping you take the work out of scheduling!" +
                "<br><br>" +
                "Your OpenShift Support Team";
            SendEmail(UserEmail, FirstName, LastName, sub, body);
        }



        //this void sends all emails. ***Must specify UserEmail, Name, subject ( called "sub" ) and body in all voids above
        public static void SendEmail(string UserEmail, string FirstName, string LastName, string sub, string body)
        {

            var FullName = FirstName + ' ' + LastName;
            var senderEmail = new MailAddress(ConfigurationManager.AppSettings["OpenShiftEmail"], "OpenShift Support", System.Text.Encoding.UTF8);
            var receiverEmail = new MailAddress(UserEmail, FullName);
            var password = ConfigurationManager.AppSettings["EmailPassword"];
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                DeliveryFormat = SmtpDeliveryFormat.International,
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