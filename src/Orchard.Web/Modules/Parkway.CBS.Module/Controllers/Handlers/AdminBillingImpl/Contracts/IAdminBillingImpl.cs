using Orchard;
using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Module.ViewModels;

namespace Parkway.CBS.Module.Controllers.Handlers.AdminBillingImpl.Contracts
{
    public interface IAdminBillingImpl : IDependency
    {
        BillingType BillingType { get; }

        string PartialToShow { get; }

        ViewToShowVM ViewForDataInput(RevenueHeadDetails revenueHeadDetails, TaxPayerWithDetails taxPayer);


        /// <summary>
        /// Validate the data input for invoice generation
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        void ValidateInvoiceDataInput(AdminGenerateInvoiceVM model, ref List<ErrorModel> errors);


        /// <summary>
        /// If the ValidateInvoiceDataInput method call has errors, use this method to get back the view model and view to show for the billing type
        /// </summary>
        /// <param name="model"></param>
        /// <param name="revenueHeadDetails"></param>
        /// <param name="taxPayer"></param>
        /// <returns>ViewToShowVM</returns>
        ViewToShowVM GetModelForDataInputCallBack(AdminGenerateInvoiceVM model, RevenueHeadDetails revenueHeadDetails, TaxPayerWithDetails taxPayer);


        /// <summary>
        /// If validation is good, get the model for invoice confirmation
        /// </summary>
        /// <param name="model"></param>
        /// <param name="revenueHeadDetails"></param>
        /// <param name="taxPayer"></param>
        /// <returns></returns>
        AdminConfirmingInvoiceVM GetModelForInvoiceConfirmation(AdminGenerateInvoiceVM model, RevenueHeadDetails revenueHeadDetails);



        CreateInvoiceModel GetCreateInvoiceModel(AdminConfirmingInvoiceVM model, TaxPayerWithDetails taxPayer);
    }
}
