using System.Linq;
using System.Dynamic;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models;
using System.Collections.Generic;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Module.Web.Controllers.Handlers.BillingImpl.Contracts;
using System;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;

namespace Parkway.CBS.Module.Web.Controllers.Handlers.BillingImpl
{
    public class OneOffAssessmentBillingImpl : BaseBillingImpl, IBillingImpl
    {
        private readonly IFormControlRevenueHeadManager<FormControlRevenueHead> _formRevenueHeadRepository;

        public OneOffAssessmentBillingImpl(IFormControlRevenueHeadManager<FormControlRevenueHead> formRevenueHeadRepository, ICoreCollectionService coreCollectionService) : base(coreCollectionService)
        {
            _formRevenueHeadRepository = formRevenueHeadRepository;
        }

        public BillingType BillingType { get => BillingType.OneOff; }


        public TaxProfileFormVM GetModelForFormInput(RevenueHeadDetails revenueHeadDetails, IEnumerable<FormControl> controls, TaxEntityCategory category, GenerateInvoiceStepsModel processStage, TenantCBSSettings tenant)
        {
            return new TaxProfileFormVM
            {
                HeaderObj = new HeaderObj { ShowSignin = true },
                RevenueHeadName = revenueHeadDetails.RevenueHead.Name,
                MDAName = revenueHeadDetails.Mda.Name,
                CategoryName = GetCatText(category),
                TaxEntityViewModel = new TaxEntityViewModel { },
                BillingType = BillingType,
                Amount = revenueHeadDetails.Billing.Amount,
                Surcharge = revenueHeadDetails.Billing.Surcharge
            };
        }


        /// <summary>
        /// After a new user profile has been created or one has been retrieved from storage, lets determine the route to get this user to
        /// <para>For example, direct assessment with file upload should redirect the user to a page where the user can add payes/upload a file</para>
        /// </summary>
        /// <returns>string</returns>
        public RouteToVM ConfirmingInvoiceRoute() { return new RouteToVM { RouteName = "Confirm.Bill", Stage = InvoiceGenerationStage.ShowInvoiceConfirm }; }


        /// <summary>
        /// Model called when you want to confirm an invoice
        /// <para>Dynamic model includes ViewToShow and ViewModel properties</para>
        /// </summary>
        /// <param name="processStage"></param>
        /// <param name="user"></param>
        /// <returns>dynamic</returns>
        public dynamic ConfirmingInvoice(GenerateInvoiceStepsModel processStage, UserDetailsModel user)
        {
            if (processStage.InvoiceGenerationStage != InvoiceGenerationStage.ShowInvoiceConfirm) { throw new NoBillingTypeSpecifiedException(); }

            var entityAndHeaderObj = GetTaxEntity(processStage, user);
            TaxEntity entity = entityAndHeaderObj.TaxEntity;

            if (entity == null)
            { throw new AuthorizedUserNotFoundException(); }

            HeaderObj headerObj = entityAndHeaderObj.HeaderObj;
            dynamic returnOBJ = new ExpandoObject();
            returnOBJ.ViewToShow = "ConfirmInvoice";
            ConfirmInvoiceVM viewModel = new ConfirmInvoiceVM
            {
                Amount = processStage.ProceedWithInvoiceGenerationVM.Amount,
                Surcharge = processStage.ProceedWithInvoiceGenerationVM.Surcharge,
                BillingType = BillingType,
                CategoryName = processStage.ProceedWithInvoiceGenerationVM.CategoryName,
                ExternalRef = processStage.ExternalRef,
                HeaderObj = headerObj,
                MDAName = processStage.ProceedWithInvoiceGenerationVM.MDAName,
                RevenueHeadName = processStage.ProceedWithInvoiceGenerationVM.RevenueHeadName,
                TaxEntityViewModel = new TaxEntityViewModel { Address = entity.Address, Email = entity.Email, ExternalBillNumber = processStage.ExternalRef, PhoneNumber = entity.PhoneNumber, Recipient = entity.Recipient, TaxPayerIdentificationNumber = entity.TaxPayerIdentificationNumber, PayerId = entity.PayerId },
                HasMessage = processStage.ProceedWithInvoiceGenerationVM.HasMessage,
                Message = processStage.ProceedWithInvoiceGenerationVM.Message,
                IsVisibleSurcharge = processStage.ProceedWithInvoiceGenerationVM.Surcharge == 0.0m ? false : true
            };

            viewModel.Forms = processStage.ValidatedFormFields ? new List<FormControlViewModel> { } : _coreCollectionService.GetFormDetailsFromDB(processStage.RevenueHeadId, processStage.CategoryId);
            returnOBJ.ViewModel = viewModel;
            return returnOBJ;
        }


        /// <summary>
        /// validate model for invoices that have been confirmed
        /// </summary>
        /// <param name="model"></param>
        /// <param name="user"></param>
        /// <param name="errors"></param>
        /// <returns>ValidateInvoiceConfirmModel</returns>
        public void ValidateConfirmingInvoice(GenerateInvoiceStepsModel processStage, ConfirmInvoiceVM model, ref List<ErrorModel> errors)
        {
            if (processStage.ProceedWithInvoiceGenerationVM.Amount <= 0)
            { errors.Add(new ErrorModel { ErrorMessage = ErrorLang.invoiceamountistoosmall().ToString(), FieldName = "Amount" }); }
        }


        /// <summary>
        /// Get the view model for when there is an error when this invoice confirmation has errors
        /// </summary>
        /// <param name="processStage"></param>
        /// <param name="model"></param>
        /// <param name="user"></param>
        /// <returns>ValidateInvoiceConfirmModel</returns>
        public ValidateInvoiceConfirmModel ConfirmingInvoiceFailed(GenerateInvoiceStepsModel processStage, ConfirmInvoiceVM model, UserDetailsModel user, List<ErrorModel> errors)
        {
            var pageModel = ConfirmingInvoice(processStage, user);
            pageModel.ViewModel.Amount = processStage.ProceedWithInvoiceGenerationVM.Amount;
            pageModel.ViewModel.ExternalRef = model.ExternalRef;
            pageModel.ViewModel.Forms = model.Forms;
            string sErrors = string.Join("\r", errors.Select(e => e.ErrorMessage).ToArray());
            return new ValidateInvoiceConfirmModel { ViewModel = pageModel.ViewModel, ViewToShow = pageModel.ViewToShow, ErrorMessage = sErrors };
        }


        /// <summary>
        /// Get model invoice confirmed
        /// </summary>
        /// <param name="model"></param>
        /// <returns>InvoiceConfirmedModel</returns>
        public InvoiceConfirmedModel ConfirmedInvoice(GenerateInvoiceStepsModel processStage, ConfirmInvoiceVM model)
        { return new InvoiceConfirmedModel { Amount = processStage.ProceedWithInvoiceGenerationVM.Amount, ExternalRef = model.ExternalRef }; }



        /// <summary>
        /// Create model for invoice generation, after confirmation
        /// </summary>
        /// <param name="processStage"></param>
        /// <param name="user"></param>
        /// <returns>CreateInvoiceModel</returns>
        public CreateInvoiceModel GenerateInvoice(GenerateInvoiceStepsModel processStage, TaxEntity entity)
        {
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
                    Amount = processStage.InvoiceConfirmedModel.Amount,
                    AdditionalDetails = GetAdditionalFormFields(processStage),
                    CategoryId = processStage.CategoryId
                },
                ExternalRefNumber = processStage.InvoiceConfirmedModel.ExternalRef,
            };
        }

        /// <summary>
        /// Get invoice input page, is used for billing types that require additional input for invoice generation. Like a file upload page or an onscreen input page
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="processStage"></param>
        /// <returns>ViewToShowVM</returns>
        public ViewToShowVM InvoiceInputPage(TaxEntity entity, GenerateInvoiceStepsModel processStage, string siteName)
        {
            throw new NotImplementedException("One off do not support invoice input page");
        }

    }
}