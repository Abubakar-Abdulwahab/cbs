using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Payee;
using Parkway.CBS.Payee.PayeeAdapters.ETCC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Parkway.CBS.Client.Web.Controllers.Handlers.Contracts
{
    public interface IPAYEHandler : IDependency
    {
        /// <summary>
        /// Queues up the validation of the items in hangfire
        /// </summary>
        /// <param name="batchStagingRecordId">Batch record Id for the collection of items to be validated</param>
        /// <param name="tenantName">Tenant name</param>
        void QueueItemsForValidation(string tenantName, long batchStagingRecordId);

        /// <summary>
        /// Encrypts the value
        /// </summary>
        /// <param name="value"></param>
        /// <returns> A string of the encrypted value </returns>
        string EncryptBatchToken(string value);

        /// <summary>
        /// Gets the Assessment interface
        /// </summary>
        /// <param name="processStage"></param>
        /// <returns> AssessmentInterface object </returns>
        AssessmentInterface GetAssessmentInterface(GenerateInvoiceStepsModel processStage);

        /// <summary>
        /// Save batch record staging
        /// </summary>
        /// <param name="batchRecordStaging"></param>
        /// <exception cref="CouldNotSaveRecord">Could not save record</exception>
        /// <returns>ProcessingReportVM | An object of the saved batch record staging </returns>
        ProcessingReportVM SaveBatchRecordStaging(PAYEBatchRecordStaging batchRecordStaging, GenerateInvoiceStepsModel processStage);

        /// <summary>
        /// Validates file input
        /// </summary>
        /// <param name="file"></param>
        /// <param name="errorMessage"></param>
        /// <exception cref="FileNotFoundException">File not found</exception>
        /// <exception cref="Exception">Invalid file type </exception>
        void ValidateFileUpload(HttpPostedFileBase file, ref string errorMessage);

        /// <summary>
        /// Roll back all transactions
        /// </summary>
        void RollBackAllTransactions();
    }
}
