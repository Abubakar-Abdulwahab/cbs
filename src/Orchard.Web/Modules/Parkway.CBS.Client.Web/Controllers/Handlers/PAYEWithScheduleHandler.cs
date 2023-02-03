using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Orchard.Logging;
using Parkway.CBS.Client.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Module.Web.Controllers.Handlers.BillingImpl;
using Parkway.CBS.Payee.PayeeAdapters;

namespace Parkway.CBS.Client.Web.Controllers.Handlers
{
    public class PAYEWithScheduleHandler : IPAYEWithScheduleHandler
    {
        private readonly DirectAssessmentBillingImpl _directAssessmentBillingImpl;
        public ILogger Logger { get; set; }
        private readonly ICorePAYEService _corePAYE;
        private readonly IPAYEExemptionTypeManager<PAYEExemptionType> _payeExemptionTypeManager;

        public PAYEWithScheduleHandler(ICorePAYEService corePAYE, DirectAssessmentBillingImpl billingImpl, IPAYEExemptionTypeManager<PAYEExemptionType> payeExemptionTypeManager)
        {
            _directAssessmentBillingImpl = billingImpl;
            Logger = NullLogger.Instance;
            _corePAYE = corePAYE;
            _payeExemptionTypeManager = payeExemptionTypeManager;
        }

        /// <summary>
        /// Returns a list of PAYEExemptionTypeVM of all active exemptionTypes
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PAYEExemptionTypeVM> GetAllActivePAYEExemptionTypes()
        {
            try
            {
                return _payeExemptionTypeManager.GetAllActivePAYEExemptionTypes();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        public InvoiceProceedVMForPayeAssessment GetDirectAssessmentBillVM(GenerateInvoiceStepsModel processStage, TaxEntity entity)
        {
            InvoiceProceedVM model = new InvoiceProceedVM { };

            model.MDAName = processStage.ProceedWithInvoiceGenerationVM.MDAName;
            model.RevenueHeadName = processStage.ProceedWithInvoiceGenerationVM.RevenueHeadName;
            model.CategoryName = processStage.ProceedWithInvoiceGenerationVM.CategoryName;

            model.HasMessage = processStage.ProceedWithInvoiceGenerationVM.HasMessage;
            model.Message = processStage.ProceedWithInvoiceGenerationVM.Message;

            model.TaxEntityViewModel = new TaxEntityViewModel
            {
                Address = entity.Address,
                Email = entity.Email,
                PhoneNumber = entity.PhoneNumber,
                Recipient = entity.Recipient,
                TaxPayerIdentificationNumber = entity.TaxPayerIdentificationNumber,
                PayerId = entity.PayerId,
                
            };

            return new InvoiceProceedVMForPayeAssessment { InvoiceProceedVM = model, PAYEExemptionTypes = GetAllActivePAYEExemptionTypes() };
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="processStage"></param>
        /// <param name="user"></param>
        /// <returns>dynamic</returns>
        public dynamic GetResultsViewForPAYEAssessment(GenerateInvoiceStepsModel processStage, UserDetailsModel user)
        {
            return _directAssessmentBillingImpl.ConfirmingInvoice(processStage, user);
        }


        private FileProcessModel GetFileProcessModel(string batchToken)
        {
            return JsonConvert.DeserializeObject<FileProcessModel>(Util.LetsDecrypt(batchToken, System.Configuration.ConfigurationManager.AppSettings["EncryptionSecret"]));
        }


        /// <summary>
        /// Get percentage for file or onscreen processing
        /// </summary>
        /// <param name="batchToken"></param>
        /// <returns>APIResponse</returns>
        public APIResponse GetFileProcessPercentage(string batchToken)
        {
            try
            {
                Logger.Information("Decrypting direct assessment file processing batch token");
                return _corePAYE.CheckForCompletionPercentage(GetFileProcessModel(batchToken));
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message + " .Error decrypting batch token " + batchToken);
            }

            return new APIResponse { ResponseObject = ErrorLang.genericexception().ToString(), Error = true };
        }



        /// <summary>
        /// Get table data for this assessment
        /// </summary>
        /// <param name="batchToken"></param>
        /// <returns>APIResponse</returns>
        public APIResponse GetTableData(string batchToken, long taxEntityId)
        {
            try
            {
                Logger.Information("Decrypting direct assessment file processing batch token");
                FileProcessModel objValue = GetFileProcessModel(batchToken);
                //get the report details
                return new APIResponse { ResponseObject = _corePAYE.GetReportDetails(objValue.Id, taxEntityId) };
            }
            catch (NoRecordFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                return new APIResponse { Error = true, ResponseObject = ErrorLang.norecord404().ToString() };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().ToString() };
            }
        }



        public APIResponse GetPagedPAYEData(string batchToken, long taxEntityId, int page)
        {
            try
            {
                Logger.Information("Decrypting direct assessment file processing batch token");
                FileProcessModel objValue = GetFileProcessModel(batchToken);
                //get the report details
                int take = 10;
                int skip = page == 1 ? 0 : (take * page) - take;
                return new APIResponse { ResponseObject = _corePAYE.GetPagedDataForBatchItemsStaging(objValue.Id, taxEntityId, skip, take) };
            }
            catch (NoRecordFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                return new APIResponse { Error = true, ResponseObject = ErrorLang.norecord404().ToString() };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().ToString() };
            }
        }



        /// <summary>
        /// Do processing for when the user has confirmed the results of the schedule validation
        /// </summary>
        /// <param name="processStage"></param>
        /// <param name="taxEntityId"></param>
        /// <exception cref="AmountTooSmallException"></exception>
        public void DoProcessingScheduleResultConfirmation(GenerateInvoiceStepsModel processStage, long taxEntityId)
        {
            FileProcessModel objValue = JsonConvert.DeserializeObject<FileProcessModel>(Util.LetsDecrypt(processStage.ProcessingDirectAssessmentReportVM.RequestToken, AppSettingsConfigurations.EncryptionSecret));
            //check for the amount of valid entries
            decimal totalValidAmount = GetValidEntriesAmountForStagingBatch(objValue.Id, taxEntityId);
            if (totalValidAmount <= 0.00m)
            {
                throw new AmountTooSmallException();
            }
            //
            processStage.InvoiceConfirmedModel = new InvoiceConfirmedModel { Amount = totalValidAmount, Token = processStage.ProcessingDirectAssessmentReportVM.RequestToken };
            processStage.InvoiceGenerationStage = InvoiceGenerationStage.GenerateInvoice;
        }



        /// <summary>
        /// Get the total amount of valid entries
        /// </summary>
        /// <param name="id"></param>
        /// <param name="taxEntityId"></param>
        /// <returns>decimal</returns>
        public decimal GetValidEntriesAmountForStagingBatch(long id, long taxEntityId)
        {
            try
            {
                return _corePAYE.GetTotalAmountForValidEntriesInBatchStaging(id, taxEntityId);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception getting valid amount for batch Id {0}, {1}", id, exception.Message));
                throw;
            }
        }

    }
}