using Orchard.Logging;
using Orchard.UI.Notify;
using Parkway.CBS.Client.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Module.Web.Controllers;
using Parkway.CBS.Module.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Module.Web.Middleware.Filters;
using System;
using System.Web.Mvc;


namespace Parkway.CBS.Client.Web.Controllers
{
    [CBSCollectionAJAXAuthorized]
    public class PAYEWithScheduleAJAXController : BaseLiteController
    {
        private readonly IPAYEWithScheduleHandler _handler;
        public ILogger Logger { get; set; }

        public PAYEWithScheduleAJAXController(IPAYEWithScheduleHandler handler, IHandlerComposition handlerComposition) : base(handlerComposition)
        {
            _handler = handler;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Get the process percentage for this batch record
        /// </summary>
        /// <param name="batchToken"></param>
        /// <returns></returns>
        public virtual JsonResult GetProcessPercentage(string batchToken)
        {
            Logger.Information("starting process payee");
            return Json(_handler.GetFileProcessPercentage(batchToken), JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// here we want to get the starting point basically for the 
        /// assessment
        /// </summary>
        /// <param name="batchToken"></param>
        /// <returns></returns>
        public virtual JsonResult GetReportData(string batchToken)
        {
            try
            {
                Logger.Information(string.Format("getting page data for batch token - {0} page - {1}", batchToken, 1));
                string errorMessage = string.Empty;
                UserDetailsModel user = GetLoggedInUserDetails();
                TaxEntity entity = null;
                if (user != null && user.Entity != null) { entity = new TaxEntity { Id = user.TaxPayerProfileVM.Id }; }

                if (entity == null)
                {
                    GenerateInvoiceStepsModel processStage = GetDeserializedSessionObj(ref errorMessage);
                    if (processStage.ProceedWithInvoiceGenerationVM != null)
                    {
                        if (processStage.ProceedWithInvoiceGenerationVM.FromTaxProfileSetup)
                        { entity = processStage.ProceedWithInvoiceGenerationVM.Entity; }
                    }
                }

                if (entity == null) { throw new AuthorizedUserNotFoundException(); }

                return Json(_handler.GetTableData(batchToken, entity.Id), JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
            return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().ToString() }, JsonRequestBehavior.AllowGet);
        }



        public virtual JsonResult GetPAYEMoveRight(string batchToken, int page)
        {
            try
            {
                Logger.Information(string.Format("getting page data for batch token - {0} page - {1}", batchToken, page));
                string errorMessage = string.Empty;
                UserDetailsModel user = GetLoggedInUserDetails();
                TaxEntity entity = null;
                if (user != null && user.Entity != null) { entity = new TaxEntity { Id = user.TaxPayerProfileVM.Id }; }

                if (entity == null)
                {
                    GenerateInvoiceStepsModel processStage = GetDeserializedSessionObj(ref errorMessage);
                    if (processStage.ProceedWithInvoiceGenerationVM != null)
                    {
                        if (processStage.ProceedWithInvoiceGenerationVM.FromTaxProfileSetup)
                        { entity = processStage.ProceedWithInvoiceGenerationVM.Entity; }
                    }
                }

                if (entity == null) { throw new AuthorizedUserNotFoundException(); }
                return Json(_handler.GetPagedPAYEData(batchToken, entity.Id, page), JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
            return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().ToString() }, JsonRequestBehavior.AllowGet);            
        }

    }
}