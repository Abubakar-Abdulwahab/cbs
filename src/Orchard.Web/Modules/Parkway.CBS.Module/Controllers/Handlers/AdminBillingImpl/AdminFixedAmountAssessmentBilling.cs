using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Module.Controllers.Handlers.AdminBillingImpl.Contracts;
using Parkway.CBS.Module.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Module.Controllers.Handlers.AdminBillingImpl
{
    public class AdminFixedAmountAssessmentBilling : BaseAdminBillingImpl, IAdminBillingImpl
    {
        public BillingType BillingType => BillingType.FixedAmountAssessment;
        public string PartialToShow => "";


        /// <summary>
        /// Get the view for one off payments
        /// </summary>
        /// <param name="revenueHeadDetails"></param>
        /// <returns>ViewToShowVM</returns>
        public ViewToShowVM ViewForDataInput(RevenueHeadDetails revenueHeadDetails, TaxPayerWithDetails taxPayer)
        {
            return new ViewToShowVM { ViewModel = new AdminGenerateInvoiceVM { DoesNotHaveInput = true, Amount = revenueHeadDetails.Billing.Amount, BillingType = (BillingType)revenueHeadDetails.Billing.BillingType } };
        }


        /// <summary>
        /// Validate the data input for invoice generation
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public void ValidateInvoiceDataInput(AdminGenerateInvoiceVM model, ref List<ErrorModel> errors)
        { }

        public ViewToShowVM GetModelForDataInputCallBack(AdminGenerateInvoiceVM model, RevenueHeadDetails revenueHeadDetails, TaxPayerWithDetails taxPayer)
        {
            var vm = ViewForDataInput(revenueHeadDetails, taxPayer);
            vm.ViewModel.Amount = model.Amount;
            return vm;
        }

        public AdminConfirmingInvoiceVM GetModelForInvoiceConfirmation(AdminGenerateInvoiceVM model, RevenueHeadDetails revenueHeadDetails)
        {
            return new AdminConfirmingInvoiceVM { Amount = revenueHeadDetails.Billing.Amount };
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