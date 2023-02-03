using System.Linq;
using Newtonsoft.Json;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models;
using System.Collections.Generic;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Module.Web.Controllers.Handlers.BillingImpl.Contracts;
using System.Dynamic;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using System;

namespace Parkway.CBS.Module.Web.Controllers.Handlers.BillingImpl
{
    public class DirectAssessmentBillingImpl : BaseBillingImpl, IBillingImpl, IBillingTypeInvoiceConfirmation
    {
        private readonly IPAYEBatchRecordStagingManager<PAYEBatchRecordStaging> _payeBatchRepo;

        public DirectAssessmentBillingImpl(IPAYEBatchRecordStagingManager<PAYEBatchRecordStaging> payeBatchRepo, ICoreCollectionService coreCollectionService) : base(coreCollectionService)
        {
            _payeBatchRepo = payeBatchRepo;
        }


        public BillingType BillingType { get => BillingType.DirectAssessment; }


        /// <summary>
        /// For invoice generation get  the view model for the billing type
        /// </summary>
        /// <param name="revenueHeadDetails"></param>
        /// <param name="entity"></param>
        /// <param name="controls"></param>
        /// <param name="category"></param>
        /// <param name="processStage"></param>
        /// <param name="tenant"></param>
        /// <returns>TaxProfileFormVM</returns>
        public TaxProfileFormVM GetModelForFormInput(RevenueHeadDetails revenueHeadDetails, IEnumerable<FormControl> controls, TaxEntityCategory category, GenerateInvoiceStepsModel processStage, TenantCBSSettings tenant)
        {
            DirectAssessmentModel directAssessment = string.IsNullOrEmpty(revenueHeadDetails.Billing.DirectAssessmentModel) ? null : JsonConvert.DeserializeObject<DirectAssessmentModel>(revenueHeadDetails.Billing.DirectAssessmentModel);

            return new TaxProfileFormVM
            {
                HeaderObj = new HeaderObj { ShowSignin = true },
                RevenueHeadName = revenueHeadDetails.RevenueHead.Name,
                MDAName = revenueHeadDetails.Mda.Name,
                CategoryName = GetCatText(category),
                TaxEntityViewModel = new TaxEntityViewModel { },
                Amount = revenueHeadDetails.Billing.Amount,
                BillingType = BillingType,
                Surcharge = revenueHeadDetails.Billing.Surcharge
            };
        }


        /// <summary>
        /// After a new user profile has been created or one has been retrieved from storage, lets determine the route to get this user to
        /// <para>For example, direct assessment with file upload should redirect the user to a page where the user can add payes/upload a file</para>
        /// </summary>
        /// <returns>ProfileRouteToVM</returns>
        public RouteToVM ConfirmingInvoiceRoute() { return new RouteToVM { RouteName = "C.SelectPAYEOption", Stage = InvoiceGenerationStage.InvoiceGenerated }; }



        public dynamic ConfirmingInvoice(GenerateInvoiceStepsModel processStage, UserDetailsModel user)
        {
            var entityAndHeaderObj = GetTaxEntity(processStage, user);

            if (entityAndHeaderObj.TaxEntity == null)
            { throw new AuthorizedUserNotFoundException(); }

            dynamic returnOBJ = new ExpandoObject();
            returnOBJ.ViewToShow = "PayeeAssessmentReport";
            returnOBJ.ViewModel = new DirectAssessmentReportVM
            {
                MDAName = processStage.ProceedWithInvoiceGenerationVM.MDAName,
                RevenueHeadName = processStage.ProceedWithInvoiceGenerationVM.RevenueHeadName,
                Category = processStage.ProceedWithInvoiceGenerationVM.CategoryName,
                AdapterValue = processStage.ProcessingDirectAssessmentReportVM.AdapterValue,
                HeaderObj = entityAndHeaderObj.HeaderObj,
                Email = processStage.ProceedWithInvoiceGenerationVM.Entity.Email,
                PhoneNumber = processStage.ProceedWithInvoiceGenerationVM.Entity.PhoneNumber,
                Recipient = processStage.ProceedWithInvoiceGenerationVM.Entity.Recipient,
                TIN = processStage.ProceedWithInvoiceGenerationVM.Entity.TaxPayerIdentificationNumber,
                PayerId = processStage.ProceedWithInvoiceGenerationVM.Entity.PayerId,
                Token = processStage.ProcessingDirectAssessmentReportVM.RequestToken
            };

            return returnOBJ;
        }


        public void ValidateConfirmingInvoice(GenerateInvoiceStepsModel processStage, ConfirmInvoiceVM model, ref List<ErrorModel> errors)
        {
            //to validate a paye direct assessment we would need to check that the paye record processing was completed to 100% and the amount for this process
            //is greater than 0 naira
            //lets decrypt the token value
            var stringObj = Util.LetsDecrypt(model.Token, AppSettingsConfigurations.EncryptionSecret);
            FileProcessModel fileObj = JsonConvert.DeserializeObject<FileProcessModel>(stringObj);
            //lets get the record for the database
            DirectAssessmentBatchRecord record = _coreCollectionService.GetBatchRecord(fileObj.Id);
            //check that processing has finished
            if (record.PercentageProgress != 100) { errors.Add(new ErrorModel { ErrorMessage = ErrorLang.payeassessmentstillprocessing().ToString(), FieldName = "Token" }); return; }

            if (record.Amount <= 0) { errors.Add(new ErrorModel { FieldName = "Amount", ErrorMessage = ErrorLang.invoiceamountistoosmall().ToString() }); return; }

            if (record.ErrorOccurred) { errors.Add(new ErrorModel { FieldName = "File", ErrorMessage = ErrorLang.errorprocessingfile().ToString() }); return; }
        }


        public ValidateInvoiceConfirmModel ConfirmingInvoiceFailed(GenerateInvoiceStepsModel processStage, ConfirmInvoiceVM model, UserDetailsModel user, List<ErrorModel> errors)
        {
            var pageModel = ConfirmingInvoice(processStage, user);
            pageModel.ViewModel.ExternalRef = model.ExternalRef;
            string sErrors = string.Join("\r", errors.Select(e => e.ErrorMessage).ToArray());
            return new ValidateInvoiceConfirmModel { ViewModel = pageModel.ViewModel, ViewToShow = pageModel.ViewToShow, ErrorMessage = sErrors };
        }


        /// <summary>
        /// Get model invoice confirmation
        /// </summary>
        /// <param name="model"></param>
        /// <returns>InvoiceConfirmedModel</returns>
        public InvoiceConfirmedModel ConfirmedInvoice(GenerateInvoiceStepsModel processStage, ConfirmInvoiceVM model)
        { return new InvoiceConfirmedModel { Token = model.Token, ExternalRef = processStage.ExternalRef }; }


        /// <summary>
        /// Create model for invoice generation
        /// </summary>
        /// <param name="processStage"></param>
        /// <param name="user"></param>
        /// <returns>CreateInvoiceModel</returns>
        public CreateInvoiceModel GenerateInvoice(GenerateInvoiceStepsModel processStage, TaxEntity entity)
        {
            var stringObj = Util.LetsDecrypt(processStage.InvoiceConfirmedModel.Token, AppSettingsConfigurations.EncryptionSecret);
            FileProcessModel fileObj = JsonConvert.DeserializeObject<FileProcessModel>(stringObj);
            //lets get the record for the database
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
                        PayerId = processStage.ProceedWithInvoiceGenerationVM.Entity.PayerId,
                    },
                    Amount = processStage.ProceedWithInvoiceGenerationVM.Amount,
                    CategoryId = processStage.CategoryId
                },
                FileUploadModel = fileObj,
                ExternalRefNumber = processStage.InvoiceConfirmedModel.ExternalRef,
            };
        }


        /// <summary>
        /// When the invoice has been confirmed and created
        /// We need to move the valid record entries into
        /// the PAYE records table and also move the valid items as well
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="invoiceId"></param>
        /// <returns>string | BatchRef</returns>
        public string InvoiceHasBeenConfirmed(InvoiceConfirmedModel confirmationModel, long invoiceId)
        {
            var stringObj = Util.LetsDecrypt(confirmationModel.Token, AppSettingsConfigurations.EncryptionSecret);
            FileProcessModel fileObj = JsonConvert.DeserializeObject<FileProcessModel>(stringObj);
            //if no schedule present, meaning this is a PAYE assessment with no schedule
            //we simple return
            if (fileObj.NoSchedulePresent) { return string.Empty; }

            if (confirmationModel.IsNotStaging)
            {
                _payeBatchRepo.InvoiceConfirmedAttachExisitngBatch(fileObj.BatchRecordId, invoiceId);
                return null;
            }
            else
            {
                return _payeBatchRepo.InvoiceConfirmedMovePAYE(fileObj.Id, invoiceId, confirmationModel.RevenueHeadSurcharge);
            }
        }


        /// <summary>
        /// Get invoice input page, is used for billing types that require additional input for invoice generation. Like a file upload page or an onscreen input page
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="processStage"></param>
        /// <returns>InvoiceProceedVM</returns>
        public ViewToShowVM InvoiceInputPage(TaxEntity entity, GenerateInvoiceStepsModel processStage, string siteName = null)
        {
            InvoiceProceedVM model = new InvoiceProceedVM { };

            model.CollectionFormVM = processStage.ProceedWithInvoiceGenerationVM.AdditionalFormFields;

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
                PayerId = entity.PayerId
            };

            return new ViewToShowVM
            { ViewToShow = "PayeeAssessment", ViewModel = new InvoiceProceedVMForPayeAssessment { InvoiceProceedVM = model, StateLGAs = GetLGAs(siteName) } };
        }

        private DirectAssessmentReportVM GetProcessingReportForFileUpload(DirectAssessmentBatchRecord record, TaxEntity entity)
        {
            //get the category
            DirectAssessmentReportVM report = null;
            if (record.PercentageProgress == 100m)
            {
                report = _coreCollectionService.GetPayeAsessmentReport(record, 10, 0, entity);
            }
            else
            {
                report = new DirectAssessmentReportVM
                {
                    Recipient = entity.Recipient,
                    TIN = entity.TaxPayerIdentificationNumber,
                    PhoneNumber = entity.PhoneNumber,
                    Email = entity.Email,
                    PayerId = entity.PayerId
                };
            }
            report.DoWork = true;
            return report;
        }


        /// <summary>
        /// Get direct assessment report
        /// </summary>
        /// <param name="batchRecordId"></param>
        /// <returns>DirectAssessmentReportVM</returns>
        private DirectAssessmentReportVM GetPayeAssessmentReportForOnscreen(DirectAssessmentBatchRecord record, TaxEntity entity)
        {
            return _coreCollectionService.GetPayeAsessmentReport(record, 10, 0, entity);
        }

    }
}