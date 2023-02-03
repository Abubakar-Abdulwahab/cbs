using Parkway.Cashflow.Ng.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.Utilities
{
    public static class InvoiceUtil
    {

        /// <summary>
        /// Build model for a customer on invoicing service
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="entity"></param>
        /// <param name="category"></param>
        /// <returns>CashFlowCreateCustomer</returns>
        public static CashFlowCreateCustomer CreateCashflowCustomer(int stateId, TaxEntity entity)
        {
            return new CashFlowCreateCustomer
            {
                Address = entity.Address,
                CountryID = 1,
                CustomerId = entity.CashflowCustomerId,
                Identifier = entity.Id.ToString(),
                Name = entity.Recipient,
                StateID = entity.StateLGA != null ? entity.StateLGA.State.Id : stateId,
                Type = entity.TaxEntityCategory.Id == 0 ? Cashflow.Ng.Models.Enums.CashFlowCustomerType.Individual : Cashflow.Ng.Models.Enums.CashFlowCustomerType.Business,
                PryContact = new CashFlowCreateCustomer.Contact
                {
                    Name = entity.Recipient,
                    Email = entity.Email,
                }
            };
        }


        /// <summary>
        /// Build the model for invoice generation on invoicing service
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="invoiceHelper"></param>
        /// <returns>CashFlowCreateInvoice</returns>
        public static CashFlowCreateInvoice CreateCashflowCustomerInvoice(CreateInvoiceHelper invoiceHelper)
        {
            return new CashFlowCreateInvoice
            {
                //Discount = invoiceHelper.DiscountModel != null ? invoiceHelper.DiscountModel.Discount : 0m,
                //DiscountType = invoiceHelper.DiscountModel != null ? invoiceHelper.DiscountModel.BillingDiscountType.ToString() : "",
                DueDate = invoiceHelper.DueDate,
                FootNote = invoiceHelper.FootNotes,
                InvoiceDate = invoiceHelper.InvoiceDate,
                Items = invoiceHelper.Items,
                Title = invoiceHelper.Title,
                Type = invoiceHelper.Type,
                VAT = invoiceHelper.VAT
            };
        }

    }
}