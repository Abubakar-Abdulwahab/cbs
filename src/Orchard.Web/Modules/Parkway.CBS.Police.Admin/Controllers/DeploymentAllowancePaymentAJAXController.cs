using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using System;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Admin.Controllers
{
    public class DeploymentAllowancePaymentAJAXController : Controller
    {
        private readonly IDeploymentAllowancePaymentHandler _handler;
        public ILogger Logger { get; set; }


        public DeploymentAllowancePaymentAJAXController(IDeploymentAllowancePaymentHandler handler)
        {
            _handler = handler;
            Logger = NullLogger.Instance;
        }


        public JsonResult GetPersonnelAccountName(string accountNumber, string bankCode)
        {
            try
            {
                if (string.IsNullOrEmpty(accountNumber?.Trim()))
                {
                    return Json(new APIResponse { Error = true, ResponseObject = "Account number not specified" }, JsonRequestBehavior.AllowGet);
                }

                if (string.IsNullOrEmpty(bankCode?.Trim()))
                {
                    return Json(new APIResponse { Error = true, ResponseObject = "Bank not specified" }, JsonRequestBehavior.AllowGet);
                }

                return Json(_handler.ValidateAccountNumber(accountNumber.Trim(), bankCode.Trim()));

            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().Text }, JsonRequestBehavior.AllowGet);
            }
        }



        public JsonResult ComputeAmountForPersonnel(string startDate, string endDate, int commandTypeId, int dayTypeId, string invoiceNumber, int sourceAccountId)
        {
            try
            {
                Logger.Information($"Computing deployment rate for personnel from {startDate}  to {endDate} for command type with id {commandTypeId}, day type with id {dayTypeId}, invoice with invoice number {invoiceNumber} on source account with account wallet configuration id {sourceAccountId}");
                return Json(_handler.ComputeAmountForPersonnel(startDate, endDate, commandTypeId, dayTypeId, invoiceNumber, sourceAccountId));
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().Text }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}