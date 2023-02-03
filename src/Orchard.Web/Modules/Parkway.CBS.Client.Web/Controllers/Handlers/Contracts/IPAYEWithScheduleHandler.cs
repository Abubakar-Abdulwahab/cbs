using Orchard;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;
using Parkway.CBS.Payee.PayeeAdapters;

namespace Parkway.CBS.Client.Web.Controllers.Handlers.Contracts
{
    public interface IPAYEWithScheduleHandler : IDependency
    {
        InvoiceProceedVMForPayeAssessment GetDirectAssessmentBillVM(GenerateInvoiceStepsModel processStage, TaxEntity entity);


        dynamic GetResultsViewForPAYEAssessment(GenerateInvoiceStepsModel processStage, UserDetailsModel user);


        /// <summary>
        /// Get percentage for file or onscreen processing
        /// </summary>
        /// <param name="batchToken"></param>
        /// <returns>APIResponse</returns>
        APIResponse GetFileProcessPercentage(string batchToken);



        APIResponse GetPagedPAYEData(string batchToken, long taxEntityId, int page);


        /// <summary>
        /// Get table data for this assessment
        /// </summary>
        /// <param name="batchToken"></param>
        /// <param name="taxEntityId"></param>
        /// <returns>APIResponse</returns>
        APIResponse GetTableData(string batchToken, long taxEntityId);


        /// <summary>
        /// Do processing for when the user has confirmed the results of the schedule validation
        /// </summary>
        /// <param name="processStage"></param>
        /// <param name="taxEntityId"></param>
        /// <exception cref="AmountTooSmallException"></exception>
        void DoProcessingScheduleResultConfirmation(GenerateInvoiceStepsModel processStage, long taxEntityId);

    }
}
