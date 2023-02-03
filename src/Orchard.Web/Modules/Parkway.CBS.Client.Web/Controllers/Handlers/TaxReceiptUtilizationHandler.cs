using Newtonsoft.Json;
using Orchard;
using Orchard.Logging;
using Parkway.CBS.Client.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Module.Web.Controllers.Handlers.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Client.Web.Controllers.Handlers
{
    public class TaxReceiptUtilizationHandler : ITaxReceiptUtilizationHandler
    {
        private readonly IOrchardServices _orchardServices;
        private readonly ICoreReceiptUtilizationService _coreReceiptUtilizationService;
        private readonly IModuleCollectionHandler _moduleCollectionHandler;
        public ILogger Logger { get; set; }

        public TaxReceiptUtilizationHandler(IOrchardServices orchardServices, ICoreReceiptUtilizationService coreReceiptUtilizationService, IModuleCollectionHandler moduleCollectionHandler)
        {
            _orchardServices = orchardServices;
            _coreReceiptUtilizationService = coreReceiptUtilizationService;
            _moduleCollectionHandler = moduleCollectionHandler;
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// Gets ReceiptUtilizationVM for schedule with specified batch ref
        /// </summary>
        /// <param name="batchRef"></param>
        /// <returns></returns>
        public ReceiptUtilizationVM GetVM(string batchRef)
        {
            try
            {
                PAYEBatchRecordVM batchInfo = _coreReceiptUtilizationService.GetBatchRecordWithBatchRef(batchRef);
                Decimal amountPaid = _coreReceiptUtilizationService.GetBatchAmountPaidWithId(batchInfo.BatchRecordId);
                return new ReceiptUtilizationVM
                {
                    BatchRecordId = batchInfo.BatchRecordId,
                    BatchRef = batchInfo.BatchRef,
                    ScheduleAmount = batchInfo.TotalIncomeTaxForPayesInSchedule + batchInfo.RevenueHeadSurCharge,
                    AmountPaid = amountPaid,
                    Surcharge = batchInfo.RevenueHeadSurCharge,
                    ScheduleType = Core.Utilities.Util.InsertSpaceBeforeUpperCase(batchInfo.AssessmentType.ToString()),
                    PaymentCompleted = batchInfo.PaymentCompleted,
                    CreatedAt = batchInfo.CreatedAt,
                    UtilizedReceipts = _coreReceiptUtilizationService.GetUtilizedReceiptsForBatch(batchInfo.BatchRecordId),
                    UnpaidInvoiceNumber = _coreReceiptUtilizationService.GetUnpaidInvoiceNumberForBatch(batchInfo.BatchRecordId).SingleOrDefault()
                };
            }
            catch (NoRecordFoundException) { throw; }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Get PAYE Receipt with specified receipt number
        /// </summary>
        /// <param name="receiptNumber"></param>
        /// <param name="userId"></param>
        /// <returns>APIResponse</returns>
        public APIResponse GetPAYEReceipt(string receiptNumber, long userId)
        {
            try
            {
                PAYEReceiptVM receipt = _coreReceiptUtilizationService.GetPAYEReceiptVMWithNumber(receiptNumber, userId);
                if(receipt == null) { return new APIResponse { ResponseObject = "Receipt not found", Error = true }; }
                return new APIResponse { ResponseObject = new { TotalAmount = string.Format("{0:n2}", receipt.TotalAmount), AvailableAmount = string.Format("{0:n2}", receipt.AvailableAmount), UtilizedAmount = string.Format("{0:n2}", receipt.UtilizedAmount), receipt.ReceiptNumber, receipt.Status } };
            }
            catch (NoRecordFoundException) { throw; }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Apply receipt with specified receipt number to batch with specified batch ref
        /// </summary>
        /// <param name="receiptNumber"></param>
        /// <param name="batchRef"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public APIResponse ApplyReceiptToBatch(string receiptNumber, string batchRef, long userId)
        {
            try
            {
                if(string.IsNullOrEmpty(receiptNumber) || string.IsNullOrEmpty(batchRef)) { throw new Exception("Receipt number and batch ref not specified"); }
                if (_coreReceiptUtilizationService.ApplyReceiptToBatch(receiptNumber, batchRef, userId)) {
                    return new APIResponse { ResponseObject = "Receipt has been applied successfully." };
                }
                else { return new APIResponse { ResponseObject = "Receipt cannot be applied to batch that has been fully paid for." }; }
            }
            catch (NoRecordFoundException) { throw; }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// When a user is signed in, we would like to redirect the user to a page where they can progress with their invoice generation
        /// </summary>
        /// <param name="user"></param>
        /// <param name="revenueHeadId"></param>
        /// <param name="categoryId"></param>
        /// <returns>ProceedWithInvoiceGenerationVM</returns>
        public ProceedWithInvoiceGenerationVM GetModelWhenUserIsSignedIn(UserDetailsModel user, int revenueHeadId, int categoryId)
        {
            try
            {
                return _moduleCollectionHandler.GetModelWhenUserIsSignedIn(user, revenueHeadId, categoryId);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Encrypt specified batch token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public string EncryptBatchToken(string token)
        {
            return Util.LetsEncrypt(token, AppSettingsConfigurations.EncryptionSecret);
        }

        /// <summary>
        /// Get Revenue Head Id for PAYE Assessment
        /// </summary>
        /// <returns></returns>
        public int GetRevenueHeadIdForPAYE()
        {
            try
            {
                return _coreReceiptUtilizationService.GetRevenueHeadIdForPAYE();
            }
            catch (Exception) { throw; }
        }
    }
}