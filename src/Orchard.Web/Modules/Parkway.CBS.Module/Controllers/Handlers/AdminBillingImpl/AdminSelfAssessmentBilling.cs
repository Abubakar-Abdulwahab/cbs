using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Module.Controllers.Handlers.AdminBillingImpl.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Module.ViewModels;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Module.Controllers.Handlers.AdminBillingImpl
{
    public class AdminSelfAssessmentBilling : BaseAdminBillingImpl, IAdminBillingImpl
    {
        public BillingType BillingType => BillingType.SelfAssessment;
        public string PartialToShow => "AddAmount";


        public ViewToShowVM ViewForDataInput(RevenueHeadDetails revenueHeadDetails, TaxPayerWithDetails taxPayer)
        {
            return new ViewToShowVM { ViewModel = new AdminGenerateInvoiceVM { Amount = revenueHeadDetails.Billing.Amount, MDAName = revenueHeadDetails.Mda.NameAndCode(), RevenueHeadName = revenueHeadDetails.RevenueHead.Name, TaxPayerId = taxPayer.Id, TaxPayerWithDetails = taxPayer,RevenueHeadId = revenueHeadDetails.RevenueHead.Id, BillingType = (BillingType)revenueHeadDetails.Billing.BillingType, PartialToShow = PartialToShow } };
        }


        /// <summary>
        /// Validate the data input for invoice generation
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public void ValidateInvoiceDataInput(AdminGenerateInvoiceVM model, ref List<ErrorModel> errors)
        {
            //if self assessment we want to ensure that an amount greater than 0 has been added
            if(model.Amount <= 0) { errors.Add(new ErrorModel { ErrorMessage = "Amount value is too small", FieldName = "Amount" }); }
        }


        /// <summary>
        /// If the ValidateInvoiceDataInput method call has errors, use this method to get back the view model and view to show for the billing type
        /// </summary>
        /// <param name="model"></param>
        /// <param name="revenueHeadDetails"></param>
        /// <param name="taxPayer"></param>
        /// <returns>ViewToShowVM</returns>
        public ViewToShowVM GetModelForDataInputCallBack(AdminGenerateInvoiceVM model, RevenueHeadDetails revenueHeadDetails, TaxPayerWithDetails taxPayer)
        {
            var vm = ViewForDataInput(revenueHeadDetails, taxPayer);
            vm.ViewModel.Amount = model.Amount;
            return vm;
        }


        /// <summary>
        /// If validation is good, get the model for invoice confirmation
        /// </summary>
        /// <param name="model"></param>
        /// <param name="revenueHeadDetails"></param>
        /// <param name="taxPayer"></param>
        /// <returns>AdminConfirmingInvoiceVM</returns>
        public AdminConfirmingInvoiceVM GetModelForInvoiceConfirmation(AdminGenerateInvoiceVM model, RevenueHeadDetails revenueHeadDetails)
        {
            return new AdminConfirmingInvoiceVM { Amount = model.Amount };
        }

        public CreateInvoiceModel GetCreateInvoiceModel(AdminConfirmingInvoiceVM model, TaxPayerWithDetails taxPayer)
        {
            return new CreateInvoiceModel
            {
                TaxEntityInvoice = new TaxEntityInvoice
                {
                    Amount = model.Amount,
                    AdditionalDetails = new List<AdditionalDetails> { },
                },
            };
        }
    }
}