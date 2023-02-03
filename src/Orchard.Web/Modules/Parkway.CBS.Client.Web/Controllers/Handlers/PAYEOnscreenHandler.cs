using Orchard;
using Orchard.Logging;
using Parkway.CBS.Client.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Payee.PayeeAdapters.ETCC;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Client.Web.Controllers.Handlers
{
    public class PAYEOnscreenHandler : IPAYEOnscreenHandler
    {
        private readonly IPAYEHandler _payeHandler;
        private readonly IPAYEBatchItemsStagingManager<PAYEBatchItemsStaging> _payeItemsStagingrepo;
        private readonly IOrchardServices _orchardServices;

        public ILogger Logger { get; set; }

        public PAYEOnscreenHandler(IPAYEHandler payeHandler, IPAYEBatchItemsStagingManager<PAYEBatchItemsStaging> payeItemsStagingrepo, IOrchardServices orchardServices)
        {
            Logger = NullLogger.Instance;
            _payeHandler = payeHandler;
            _payeItemsStagingrepo = payeItemsStagingrepo;
            _orchardServices = orchardServices;
        }


        /// <summary>
        /// Do processing for onscreen PAYE schedule
        /// </summary>
        /// <param name="processStage"></param>
        /// <param name="payeLines"></param>
        /// <param name="userDetails"></param>
        public void DoPAYEOnScreenProcessing(GenerateInvoiceStepsModel processStage, UserDetailsModel userDetails, ICollection<PAYEAssessmentLine> payeLines)
        {
            try
            {
                PAYEBatchRecordStaging batchRecordStaging = new PAYEBatchRecordStaging
                {
                    RevenueHead = new RevenueHead { Id = processStage.RevenueHeadId },
                    Billing = new BillingModel { Id = processStage.ProceedWithInvoiceGenerationVM.BillingId },
                    TaxEntity = userDetails.Entity,
                    CBSUser = userDetails.CBSUser,
                    AssessmentType = (int)PayeAssessmentType.OnScreen,
                };

                processStage.ProcessingDirectAssessmentReportVM = _payeHandler.SaveBatchRecordStaging(batchRecordStaging, processStage);

                SavePAYEAssessmentLineItems(batchRecordStaging, payeLines);

                _payeHandler.QueueItemsForValidation(_orchardServices.WorkContext.CurrentSite.SiteName, batchRecordStaging.Id);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Could not process paye on-screen");
                _payeItemsStagingrepo.RollBackAllTransactions();
                throw;
            }
        }

        /// <summary>
        /// Save PAYE line items and batch record staging
        /// </summary>
        /// <param name="record"></param>
        /// <param name="payelines"></param>
        private void SavePAYEAssessmentLineItems(PAYEBatchRecordStaging record, ICollection<PAYEAssessmentLine> payelines)
        {
            try
            {
                _payeItemsStagingrepo.SavePAYEAssessmentLineItems(payelines.ToList(), record.Id);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Could not save onscreen paye details" + exception.Message);
                _payeHandler.RollBackAllTransactions();
                throw exception;
            }
        }

    }
}