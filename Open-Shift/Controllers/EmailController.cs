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
using Open_Shift.Models;

namespace Open_Shift.Controllers
{
    public class EmailController : Controller
    {
        static string domainName = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

        public static void NewAssociateVerification(string UserEmail, string FirstName, string LastName, string EmailVerificationToken)
        {
            string sub = "Welcome To OpenShift! Please Verify Your Email";
            string body = "Hi " + FirstName + "! <br><br> Thanks so much for signing up for OpenShift!" +
                " Please click <a href='" + domainName + "/Verification/EmailVerification/?token=" + EmailVerificationToken + "' target='_blank'><b>here</b></a> to verify your email. " +
                "After you verify your email, your manager will approve your access into OpenShift. We look forward to helping you take the work out of scheduling!" +
                "<br><br>" +
                "Your OpenShift Support Team";
            SendEmail(UserEmail, FirstName, LastName, sub, body);
        }

        public static bool NewAssociateVerificationManager(string token)
        {

            Database db = new Database();
            User u = db.getNewAssociateData(token);
            if( u == null)
            {
                return false;
            }
            List<User> m = db.GetManagersAndOwners();

            string sub = "Please verify your new associate so they can access OpenShift!";
            string body = "You've got a new sign up! Please verify their information so they can start using OpenShift!<br><br>" +
                "<table align'center'>" +
                "<tr>" +
                    "<th>First Name</th>" +
                    "<td>" + u.FirstName + "</td>" +
                "</tr>" +
                "<tr>" +
                    "<th>Last Name</th>" +
                    "<td>" + u.LastName + "</td>" +
                "</tr>" +
                "<tr>" +
                    "<th>Birthday</th>" +
                    "<td>" + u.Birthday + "</td>" +
                "</tr>" +
                "<tr>" +
                    "<th>Address 1</th>" +
                    "<td>" + u.AddressLine1 + "</td>" +
                "</tr>" +
                "<tr>" +
                    "<th>Address 2</th>" +
                    "<td>" + u.AddressLine2 + "</td>" +
                "</tr>" +
                "<tr>" +
                    "<th>Postal Code</th>" +
                    "<td>" + u.PostalCode + "</td>" +
                "</tr>" +
                "<tr>" +
                    "<th>Employee Number</th>" +
                    "<td>" + u.EmployeeNumber + "</td>" +
                "</tr>" +
                "<tr>" +
                    "<th>Phone Number</th>" +
                    "<td>" + u.Phonenumber + "</td>" +
                "</tr>" +
                "<tr>" +
                    "<th>Email</th>" +
                    "<td>" + u.Email + "</td>" +
                "</tr>" +
                "<tr>" +
                    "<th>Associate Title</th>" +
                    "<td>" + u.AssociateTitle + "</td>" +
                "</tr>" +
                "<tr>" +
                    "<th>Store Location</th>" +
                    "<td>" + u.StoreID + "</td>" +
                "</tr>" +
                "<tr>" +
                    "<th>Is a manager?</th>" +
                    "<td>" + u.IsManager + "</td>" +
                "</tr>" +
                "</table>" +
                "<br><br>" +
                "Click <a href='" + domainName + "/Verification/EmailManagerApproval/?AssociateID=" + u.AssociateID + "' target='_blank'><b>here</b></a> to confirm the information is correct." +
                "<br><br>" +
                "Your OpenShift Support Team";


            foreach (var manager in m)
            {
                SendEmail(manager.Email, manager.FirstName, manager.LastName, sub, body);
            }

            return true;
        }

        public static void NewPasswordRequest(string UserEmail, string PasswordResetToken)
        {
            string sub = "OpenShift password reset request";
            string body = "It seems you've opened a password reset request." +
                "Please click <a href='" + domainName + "/Profile/ResetPassword/?token=" + PasswordResetToken + "' target='_blank'><b>here</b></a> to reset your password. " +
                "If you did not open this request, then you can ignore this email." +
                "<br><br>" +
                "Your OpenShift Support Team";
            SendEmail(UserEmail, sub, "OpenShift", "User", body);
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