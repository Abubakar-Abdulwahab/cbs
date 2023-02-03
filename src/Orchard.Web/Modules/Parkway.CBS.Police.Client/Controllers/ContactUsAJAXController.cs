using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.Mail.Provider.Contracts;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Client.Controllers
{
    public class ContactUsAJAXController : Controller
    {
        private readonly IContactUsHandler _contactUsHandler;

        public ILogger Logger { get; set; }

        public ContactUsAJAXController(IContactUsHandler contactUsHandler)
        {
            Logger = NullLogger.Instance;
            _contactUsHandler = contactUsHandler;
        }

        public JsonResult ContactUs(string name, string email, string message, string subject)
        {
            try
            {
                dynamic messageParameters = new ExpandoObject();
                messageParameters.Name = name;
                messageParameters.Message = message;
                messageParameters.Email = email;
                messageParameters.Subject = subject;

                _contactUsHandler.SendContactUsRequest(messageParameters);
                return Json(new APIResponse { Error = false, ResponseObject = "Message Sent!" });
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
            return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().Text });
        }
    }
}