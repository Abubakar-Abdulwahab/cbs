using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using System;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Client.Controllers
{
    public class ValidateIdentificationNumberAJAXController : Controller
    {
        private readonly IValidateIdentificationNumberAJAXHandler _handler;
        public ILogger Logger { get; set; }

        public ValidateIdentificationNumberAJAXController(IValidateIdentificationNumberAJAXHandler handler)
        {
            _handler = handler;
            Logger = NullLogger.Instance;
        }

        public JsonResult GetIdentificationTypes(string categoryId)
        {
            try
            {
                int categoryIdParsed = 0;
                if (string.IsNullOrEmpty(categoryId) || !int.TryParse(categoryId, out categoryIdParsed))
                {
                    return Json(new APIResponse { Error = true, ResponseObject = "category id specified is invalid." }, JsonRequestBehavior.AllowGet);
                }
                //get identification types
                return Json(new APIResponse { ResponseObject = _handler.GetIdentificationTypesForCategory(categoryIdParsed) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
            return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().Text }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateIdentificaitonNumber(string idNumber, string idType)
        {
            string errorMessage = string.Empty;
            try
            {
                int idTypeParsed = 0;
                if (string.IsNullOrEmpty(idNumber)) { errorMessage = "Identification number not specified."; throw new Exception("Identification number not specified."); }
                if (string.IsNullOrEmpty(idType)) { errorMessage = "1:Identification type not specified."; throw new Exception("Identification type not specified."); }
                if (int.TryParse(idType, out idTypeParsed))
                {
                    var identificationType = _handler.validateIdType(idTypeParsed);
                    if (identificationType != null)
                    {
                        ValidateIdentificationNumberResponseModel response = _handler.ValidateIdentificationNumber(idNumber, idTypeParsed, identificationType.ImplementingClassName, out errorMessage);
                        response.PhoneNumber = !response.HasError ? $"*******{response.PhoneNumber.Substring(response.PhoneNumber.Length - 4, 4)}" : "**********";
                        response.EmailAddress = !string.IsNullOrEmpty(response.EmailAddress) ? $"{response.EmailAddress.Substring(0, 4)}{new string('*', response.EmailAddress.Length - 4)}" : string.Empty;

                        return Json(new APIResponse { ResponseObject = response }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new APIResponse { Error = true, ResponseObject = "1:Selected Identification type is not valid" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new APIResponse { Error = true, ResponseObject = "1:Selected Identification type is not valid" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                errorMessage = string.IsNullOrEmpty(errorMessage) ? ErrorLang.genericexception().Text : errorMessage;
            }
            return Json(new APIResponse { Error = true, ResponseObject = errorMessage }, JsonRequestBehavior.AllowGet);
        }
    }
}