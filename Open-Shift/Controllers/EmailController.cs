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
        // GET: Sms
        public ActionResult SendEmail(string UserEmail, string Name)
        {

            var senderEmail = new MailAddress(ConfigurationManager.AppSettings["OpenShiftEmail"], "OpenShift Support");
            var receiverEmail = new MailAddress(UserEmail, Name);
            var password = ConfigurationManager.AppSettings["EmailPassword"];
            var sub = "Welcome to OpenShift";
            var body = "Thanks, " + Name + ", for signing up to <b>OpenShift</b>!";
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
            return View();
        }

    }

}