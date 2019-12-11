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

namespace Open_Shift.Controllers
{
    public class SmsController : TwilioController
    {
        // GET: Sms
        //public ActionResult SendSms(string phone)
        //{
        //    var accountSid = ConfigurationManager.AppSettings["TwilioAccountSid"];
        //    var authToken = ConfigurationManager.AppSettings["TwilioAuthToken"];
        //    TwilioClient.Init(accountSid, authToken);

        //    var to = new PhoneNumber(phone); // pulled from sign up 
        //    var from = ConfigurationManager.AppSettings["OpenShiftTwilioPhoneNumber"];

        //    var message = MessageResource.Create(
        //        to: to,
        //        from: from,
        //        body: "Thank you for signing up to OpenShift! An email has been sent to your manager(s) to complete your registration. We hope you enjoy our app!");

        //    return Content(message.Sid);
        //}

        public ActionResult ReceiveSms()
        {
            var response = new MessagingResponse();
            response.Message("The Robots are coming! Head for the hills!");

            return TwiML(response);
        }
    }
}