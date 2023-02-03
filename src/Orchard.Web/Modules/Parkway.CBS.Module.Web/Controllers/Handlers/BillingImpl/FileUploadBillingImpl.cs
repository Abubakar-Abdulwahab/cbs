using Newtonsoft.Json;
using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.FileUpload;
using Parkway.CBS.FileUpload.Implementations;
using Parkway.CBS.FileUpload.Implementations.Contracts;
using Parkway.CBS.Module.Web.Controllers.Handlers.BillingImpl.Contracts;
using Parkway.CBS.Module.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace Parkway.CBS.Module.Web.Controllers.Handlers.BillingImpl
{
    public class FileUploadBillingImpl : BaseBillingImpl, IBillingImpl
    {
        private readonly IFileUploadConfiguration _fileUploadConfig;
        private readonly IStateModelManager<StateModel> _stateRepo;
        private readonly ICoreCollectionService _coreCollectionService;

        public FileUploadBillingImpl(ICoreCollectionService coreCollectionService, IStateModelManager<StateModel> stateRepo) : base(coreCollectionService)
        {
            _fileUploadConfig = new FileUploadConfiguration();
            _stateRepo = stateRepo;
            _coreCollectionService = coreCollectionService;

        }

        public BillingType BillingType { get => BillingType.FileUpload; }


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
        /// Get invoice input page, is used for billing types that require additional input for invoice generation. Like a file upload page or an onscreen input page
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="processStage"></param>
        /// <returns>InvoiceProceedVM</returns>
        public ViewToShowVM InvoiceInputPage(TaxEntity entity, GenerateInvoiceStepsModel processStage, string siteName = null)
        {
            InvoiceProceedVM model = new InvoiceProceedVM { };
            //we need to get the billing model for this guy so we can know what template it has and what implementation it has to go through
            BillingModel billing = _coreCollectionService.GetRevenueHeadBillingModel(processStage.RevenueHeadId);

            var fileUploadModelJson = JsonConvert.DeserializeObject<dynamic>(billing.FileUploadModel);

            var selectedTemplate = fileUploadModelJson.SelectedTemplate.Value as string;
            var selectedImpl = fileUploadModelJson.SelectedImplementation.Value as string;
            //lets get the FileUploadTemplate
            Template template = _fileUploadConfig.Templates(Util.GetAppXMLFilePath()).Where(templ => templ.Name == selectedTemplate).FirstOrDefault();
            if(template == null) { throw new NoBillingTypeSpecifiedException("No template found " + selectedTemplate); }

            UploadImplInterface impl = template.ListOfUploadImplementations.Where(imple => imple.Value == selectedImpl).FirstOrDefault();
            if (impl == null) { throw new NoBillingTypeSpecifiedException("No template found " + selectedImpl); }


            ////Comment out this part where it is getting the State and LGAs from xml file////
            ////now let the lib do work
            //IFileUploadImplementations implementingClass = (IFileUploadImplementations)Activator.CreateInstance(Type.GetType(impl.ClassName), _coreCollectionService);
            ////////////IFileUploadImplementations implementingClass = (IFileUploadImplementations)Activator.CreateInstance(Type.GetType(impl.ClassName));

            //TODO: abstract this parameter to return a generic file path
            //////////var viewModel = implementingClass.GetViewModelForFileUpload(Util.GetOSGOFXMLFilePath());
            //End of where it is getting the State and LGAs from xml file////

            ///Beginning of where it is getting the State and LGAs from db///
            //StateAndLGAsVM viewModel = new StateAndLGAsVM();
            //viewModel.ListOfStates = _stateRepo.GetStatesAndLGAs(); ;
            ///End of where it is getting the State and LGAs from db///

            var viewModel = new OSGOFSites { };
            viewModel.ListOfStates = _stateRepo.GetStatesAndLGAsForOSGOF();

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
                Id = entity.Id,
            };
            return new ViewToShowVM { ViewToShow = template.View, ViewModel = new InvoiceProceedVMForOSGOF { InvoiceProceedVM = model, ViewModel = viewModel }  };
        }


        /// <summary>
        /// After a new user profile has been created or one has been retrieved from storage, lets determine the route to get this user to
        /// <para>For example, direct assessment with file upload should redirect the user to a page where the user can add payes/upload a file</para>
        /// </summary>
        /// <returns>ProfileRouteToVM</returns>
        public RouteToVM ConfirmingInvoiceRoute() { return new RouteToVM { RouteName = "C.InvoiceProceed", Stage = InvoiceGenerationStage.InvoiceGenerated }; }



        public virtual dynamic ConfirmingInvoice(GenerateInvoiceStepsModel processStage, UserDetailsModel user)
        {
            Int64 batchRecordId = 0;
            DirectAssessmentReportVM report = null;
            var token = processStage.ProcessingDirectAssessmentReportVM.RequestToken;

            var entityAndHeaderObj = GetTaxEntity(processStage, user);
            TaxEntity entity = entityAndHeaderObj.TaxEntity;

            if (entity == null)
            { throw new AuthorizedUserNotFoundException(); }

            DirectAssessmentBatchRecord record = _coreCollectionService.GetBatchRecord(batchRecordId);

            if (processStage.ProcessingDirectAssessmentReportVM.Type == PayeAssessmentType.FileUpload)
            { report = GetProcessingReportForFileUpload(record, entity); }
            else if (processStage.ProcessingDirectAssessmentReportVM.Type == PayeAssessmentType.OnScreen)
            { report = GetPayeAssessmentReportForOnscreen(record, entity); }
            else { throw new NoBillingTypeSpecifiedException("Could not find the PayeAssessment Type " + processStage.ProcessingDirectAssessmentReportVM.Type); }

            report.MDAName = processStage.ProceedWithInvoiceGenerationVM.MDAName;
            report.RevenueHeadName = processStage.ProceedWithInvoiceGenerationVM.RevenueHeadName;
            report.Category = processStage.ProceedWithInvoiceGenerationVM.CategoryName;
            report.Token = token;
            report.AdapterValue = processStage.ProcessingDirectAssessmentReportVM.AdapterValue;
            report.HeaderObj = entityAndHeaderObj.HeaderObj;

            return new { ViewToShow = "PayeeAssessmentReport", ViewModel = report };
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


        public virtual void ValidateConfirmingInvoice(GenerateInvoiceStepsModel processStage, ConfirmInvoiceVM model, ref List<ErrorModel> errors)
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
            //pageModel.ViewModel.ExternalRef = model.ExternalRef;
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
        public virtual CreateInvoiceModel GenerateInvoice(GenerateInvoiceStepsModel processStage, TaxEntity entity)
        {
            var stringObj = Util.LetsDecrypt(processStage.InvoiceConfirmedModel.Token, AppSettingsConfigurations.EncryptionSecret);
            FileProcessModel fileObj = JsonConvert.DeserializeObject<FileProcessModel>(stringObj);
            //lets get the record for the database
            DirectAssessmentBatchRecord record = _coreCollectionService.GetBatchRecord(fileObj.Id);

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
                    Amount = record.Amount,
                    AdditionalDetails = GetAdditionalFormFields(processStage),
                    CategoryId = processStage.CategoryId
                },
                FileUploadModel = fileObj,
                ExternalRefNumber = processStage.InvoiceConfirmedModel.ExternalRef,
            };
        }


    }
}