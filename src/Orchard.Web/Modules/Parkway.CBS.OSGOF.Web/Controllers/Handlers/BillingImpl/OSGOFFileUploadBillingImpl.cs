using Newtonsoft.Json;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.FileUpload;
using Parkway.CBS.Module.Web.Controllers.Handlers.BillingImpl;
using Parkway.CBS.OSGOF.Admin.Models;
using Parkway.CBS.OSGOF.Admin.ViewModels;
using Parkway.CBS.OSGOF.Web.Controllers.Handlers.BillingImpl.Contracts;
using Parkway.CBS.OSGOF.Web.CoreServices.Contracts;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.OSGOF.Web.Controllers.Handlers.BillingImpl
{
    public class OSGOFFileUploadBillingImpl : FileUploadBillingImpl, IOSGOFBillingImpl
    {
        private readonly IFileUploadConfiguration _fileUploadConfig;
        private readonly ICoreOSGOFService _coreOSGOFService;
        private readonly IStateModelManager<StateModel> _stateRepo;


        public OSGOFFileUploadBillingImpl(ICoreCollectionService coreCollectionService, ICoreOSGOFService coreOSGOFService, IStateModelManager<StateModel> stateRepo) : base(coreCollectionService, stateRepo)
        {
            _fileUploadConfig = new FileUploadConfiguration();
            _coreOSGOFService = coreOSGOFService;
            _stateRepo = stateRepo;
        }


        public override dynamic ConfirmingInvoice(GenerateInvoiceStepsModel processStage, UserDetailsModel user)
        {
            Int64 batchRecordId = 0;
            CellSiteReportVM report = null;
            var token = processStage.ProcessingDirectAssessmentReportVM.RequestToken;

            var entityAndHeaderObj = GetTaxEntityAndHeader(processStage, user);
            TaxEntity entity = entityAndHeaderObj.TaxEntity;

            if (entity == null)
            { throw new AuthorizedUserNotFoundException(); }

            CellSiteClientPaymentBatch record = _coreOSGOFService.GetBatchRecord(batchRecordId);

            if (processStage.ProcessingDirectAssessmentReportVM.Type == PayeAssessmentType.FileUpload)
            { report = GetProcessingReportForFileUpload(record, entity); }
            else if (processStage.ProcessingDirectAssessmentReportVM.Type == PayeAssessmentType.OnScreen)
            { report = GetPayeAssessmentReportForOnscreen(record, entity); }
            else { throw new NoBillingTypeSpecifiedException("Could not find the osgof PayeAssessment Type " + processStage.ProcessingDirectAssessmentReportVM.Type); }

            report.MDAName = processStage.ProceedWithInvoiceGenerationVM.MDAName;
            report.RevenueHeadName = processStage.ProceedWithInvoiceGenerationVM.RevenueHeadName;
            report.Category = processStage.ProceedWithInvoiceGenerationVM.CategoryName;
            report.Token = token;
            report.AdapterValue = processStage.ProcessingDirectAssessmentReportVM.AdapterValue;
            report.HeaderObj = entityAndHeaderObj.HeaderObj;

            return new ViewToShowVM { ViewToShow = "OSGOFCellSiteReport", ViewModel = report };
        }



        public override void ValidateConfirmingInvoice(GenerateInvoiceStepsModel processStage, ConfirmInvoiceVM model, ref List<ErrorModel> errors)
        {
            //to validate a paye direct assessment we would need to check that the paye record processing was completed to 100% and the amount for this process
            //is greater than 0 naira
            //lets decrypt the token value
            var stringObj = Util.LetsDecrypt(model.Token, AppSettingsConfigurations.EncryptionSecret);
            FileProcessModel fileObj = JsonConvert.DeserializeObject<FileProcessModel>(stringObj);
            //lets get the record for the database
            CellSiteClientPaymentBatch record = _coreOSGOFService.GetBatchRecord(fileObj.Id);
            //check that processing has finished
            if (record.PercentageProgress != 100 || !record.Processed) { errors.Add(new ErrorModel { ErrorMessage = ErrorLang.payeassessmentstillprocessing().ToString(), FieldName = "Token" }); return; }
            decimal amount = _coreOSGOFService.GetTotalAmountForBatch(record);
            if (amount <= 0) { errors.Add(new ErrorModel { FieldName = "Amount", ErrorMessage = ErrorLang.invoiceamountistoosmall().ToString() }); return; }

            if (record.ErrorOccurred) { errors.Add(new ErrorModel { FieldName = "File", ErrorMessage = ErrorLang.errorprocessingfile().ToString() }); return; }
        }


        /// <summary>
        /// Create model for invoice generation
        /// </summary>
        /// <param name="processStage"></param>
        /// <param name="user"></param>
        /// <returns>CreateInvoiceModel</returns>
        public override CreateInvoiceModel GenerateInvoice(GenerateInvoiceStepsModel processStage, TaxEntity entity)
        {
            var stringObj = Util.LetsDecrypt(processStage.InvoiceConfirmedModel.Token, AppSettingsConfigurations.EncryptionSecret);
            FileProcessModel fileObj = JsonConvert.DeserializeObject<FileProcessModel>(stringObj);
            //lets get the record for the database
            CellSiteClientPaymentBatch record = _coreOSGOFService.GetBatchRecord(fileObj.Id);
            decimal amount = _coreOSGOFService.GetTotalAmountForBatch(record);
            return new CreateInvoiceModel
            {
                UserIsAuthorized = true, //lets us know that the user generating the invoice is authorized to do so
                RevenueHeadId = processStage.RevenueHeadId,
                TaxEntityInvoice = new TaxEntityInvoice
                {
                    TaxEntity = new TaxEntity
                    {
                        Address = processStage.ProceedWithInvoiceGenerationVM.Entity.Address,
                        Email = processStage.ProceedWithInvoiceGenerationVM.Entity.Email,
                        PhoneNumber = processStage.ProceedWithInvoiceGenerationVM.Entity.PhoneNumber,
                        Recipient = processStage.ProceedWithInvoiceGenerationVM.Entity.Recipient,
                        TaxPayerIdentificationNumber = processStage.ProceedWithInvoiceGenerationVM.Entity.TaxPayerIdentificationNumber,
                        Id = processStage.ProceedWithInvoiceGenerationVM.Entity.Id,
                        PayerId = processStage.ProceedWithInvoiceGenerationVM.Entity.PayerId
                    },
                    Amount = amount,
                    AdditionalDetails = GetAdditionalFormFields(processStage),
                    CategoryId = processStage.CategoryId
                },
                FileUploadModel = fileObj,
                ExternalRefNumber = processStage.InvoiceConfirmedModel.ExternalRef,
            };
        }

        /// <summary>
        /// get tax entity and header obj
        /// </summary>
        /// <param name="processStage"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        protected TaxEntityAndHeader GetTaxEntityAndHeader(GenerateInvoiceStepsModel processStage, UserDetailsModel user)
        {
            TaxEntity entity = null;
            HeaderObj headerObj = new HeaderObj { ShowSignin = true };
            if (user != null && user.Entity != null) { entity = user.Entity; headerObj.IsLoggedIn = true; headerObj.DisplayText = user.Name; }

            if (entity == null)
            {
                if (processStage.ProceedWithInvoiceGenerationVM != null)
                {
                    if (processStage.ProceedWithInvoiceGenerationVM.FromTaxProfileSetup)
                    { entity = processStage.ProceedWithInvoiceGenerationVM.Entity; }
                }
            }
            return new TaxEntityAndHeader { TaxEntity = entity, HeaderObj = headerObj };
        }


        private CellSiteReportVM GetPayeAssessmentReportForOnscreen(CellSiteClientPaymentBatch record, TaxEntity entity)
        {
            CellSiteReportVM report = null;
            if (record.Processed)
            {
                report = _coreOSGOFService.GetCellSitesReport(record, 10, 0, entity, true);
            }
            else
            {
                report = new CellSiteReportVM
                {
                    Recipient = entity.Recipient,
                    TIN = entity.TaxPayerIdentificationNumber,
                    PhoneNumber = entity.PhoneNumber,
                    Email = entity.Email,
                };
            }
            report.DoLeg2 = true; report.DoWork = true;
            return report;
        }


        private CellSiteReportVM GetProcessingReportForFileUpload(CellSiteClientPaymentBatch record, TaxEntity entity)
        {
            CellSiteReportVM report = null;
            if (record.Processed)
            {
                report = _coreOSGOFService.GetCellSitesReport(record, 10, 0, entity, true);
            }
            else
            {
                report = new CellSiteReportVM
                {
                    Recipient = entity.Recipient,
                    TIN = entity.TaxPayerIdentificationNumber,
                    PhoneNumber = entity.PhoneNumber,
                    Email = entity.Email,
                };
            }
            report.DoWork = true;
            return report;
        }
    }    
}