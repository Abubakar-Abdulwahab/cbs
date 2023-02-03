using Parkway.CBS.Client.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Module.Web.Controllers.Handlers.BillingImpl;
using Parkway.CBS.Module.Web.Controllers.Handlers.BillingImpl.Contracts;
using Parkway.CBS.Module.Web.Controllers.Handlers.Contracts;
using System;

namespace Parkway.CBS.Client.Web.Controllers.Handlers
{
    public class PAYEWithNoScheduleUploadHandler : IPAYEWithNoScheduleUploadHandler
    {
        private readonly IModuleCollectionHandler _handler;
        private readonly IRevenueHeadManager<RevenueHead> _revenueHeadRepository;
        private readonly IBillingTypeInvoiceConfirmation _directAssessmentConfirmation;


        public PAYEWithNoScheduleUploadHandler(IModuleCollectionHandler handler, IRevenueHeadManager<RevenueHead> revenueHeadRepository, DirectAssessmentBillingImpl directAssessmentConfirmation)
        {
            _handler = handler;
            _revenueHeadRepository = revenueHeadRepository;
            _directAssessmentConfirmation = directAssessmentConfirmation;
        }


        public string ConfirmPAYESchedule(InvoiceConfirmedModel invoiceConfirmedModel)
        {
            return _directAssessmentConfirmation.InvoiceHasBeenConfirmed(invoiceConfirmedModel, 0);
        }


        /// <summary>
        /// Get generate invoice steps model for PAYE no schedule schedule upload
        /// </summary>
        /// <param name="userDetails"></param>
        /// <returns>GenerateInvoiceStepsModel</returns>
        public GenerateInvoiceStepsModel GetPAYEDetailsForNoSchdeuleFileUpload(UserDetailsModel userDetails)
        {
            //get PAYE revenue head
            RevenueHeadDetails payeDeets = _revenueHeadRepository.GetRevenueHeadDetailsForPaye();
            ProceedWithInvoiceGenerationVM proceedWithInvoiceVM = _handler.GetModelWhenUserIsSignedIn(userDetails, payeDeets.RevenueHead.Id, userDetails.CategoryVM.Id);

            GenerateInvoiceStepsModel processStage = new GenerateInvoiceStepsModel { CategoryId = userDetails.CategoryVM.Id, RevenueHeadId = payeDeets.RevenueHead.Id, InvoiceGenerationStage = InvoiceGenerationStage.InvoiceProceed };

            processStage.BillingType = proceedWithInvoiceVM.BillingType;
            processStage.ProceedWithInvoiceGenerationVM = proceedWithInvoiceVM;
            processStage.InvoiceGenerationStage = InvoiceGenerationStage.PAYEProcessNoScheduleUpload;

            return processStage;
        }


    }
}