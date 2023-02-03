using Newtonsoft.Json;
using Parkway.Cashflow.Ng.Models;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.ClientServices.Invoicing
{
    public abstract class BaseInvoiceGeneration
    {
        
        /// <summary>
        /// Get due date for this invoice
        /// </summary>
        /// <param name="dueDateJSONModel"></param>
        /// <param name="invoiceDate"></param>
        /// <param name="nextBillingDate"></param>
        /// <returns>DateTime</returns>
        protected DateTime GetDueDate(string dueDateJSONModel, DateTime invoiceDate, DateTime? nextBillingDate)
        {
            if (string.IsNullOrEmpty(dueDateJSONModel)) { throw new NoDueDateTypeFoundException("No due date found for billing"); }

            DueDateModel dueDate = JsonConvert.DeserializeObject<DueDateModel>(dueDateJSONModel);
            if (dueDate == null) { throw new NoDueDateTypeFoundException("No due date found for billing. Due date is null for billing"); }

            //if the due date is due on the next billing date
            if (dueDate.DueOnNextBillingDate) { return nextBillingDate.Value; }

            switch (dueDate.DueDateAfter)
            {
                case DueDateAfter.Days:
                    return invoiceDate.AddDays(dueDate.DueDateInterval);
                case DueDateAfter.Weeks:
                    return invoiceDate.AddDays(7 * dueDate.DueDateInterval);
                case DueDateAfter.Months:
                    return invoiceDate.AddMonths(dueDate.DueDateInterval);
                case DueDateAfter.Years:
                    return invoiceDate.AddYears(dueDate.DueDateInterval);
                default:
                    throw new NoDueDateTypeFoundException("No due date found for billing ");
            }
        }


        /// <summary>
        /// Get footnotes for the invoice. Footnotes are the full list of discount, penalties and invoice terms as they apply
        /// </summary>
        /// <param name="billing"></param>
        /// <returns>string</returns>
        public string GetFootNotes(string billingJSONModel, string penaltyJSONModel)
        {
            string discountConcat = ""; string penaltyConcat = "";
            if (!string.IsNullOrEmpty(billingJSONModel))
            {
                List<DiscountModel> discounts = JsonConvert.DeserializeObject<List<DiscountModel>>(billingJSONModel);
                if (discounts.Any())
                {
                    discountConcat = "Discounts:\r\n";
                    foreach (var item in discounts)
                    {
                        var rate = item.BillingDiscountType == BillingDiscountType.Flat ? "Naira flat rate" : "% percent";
                        discountConcat += string.Format("\u2022 {0} {1} discount is applicable {2} {3} after invoice generation \r\n", item.Discount, rate, item.EffectiveFrom, item.EffectiveFromType.ToString().ToLower());
                    }
                }
            }

            if (!string.IsNullOrEmpty(penaltyJSONModel))
            {
                List<PenaltyModel> penalties = JsonConvert.DeserializeObject<List<PenaltyModel>>(penaltyJSONModel);
                if (penalties.Any())
                {
                    penaltyConcat = "Penalties:\r\n";
                    foreach (var item in penalties)
                    {
                        var rate = item.PenaltyValueType == PenaltyValueType.FlatRate ? "Naira flat rate" : "% percent";
                        penaltyConcat += string.Format("\u2022 penalty is applicable {2} {3} after due date \r\n", item.Value, item.PenaltyValueType, item.EffectiveFrom, item.EffectiveFromType.ToString().ToLower());
                    }
                }
            }
            return discountConcat + penaltyConcat;
        }


        /// <summary>
        /// Get the list of invoice items for the invoice
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="productId"></param>
        /// <param name="description"></param>
        /// <returns>List{CashFlowProductModel}</returns>
        protected List<CashFlowCreateInvoice.CashFlowProductModel> GetListOfValidPayees(decimal amount, long productId, string description)
        {
            return new List<CashFlowCreateInvoice.CashFlowProductModel>
            {
                { new CashFlowCreateInvoice.CashFlowProductModel
                    {
                        Pos = 1,
                        Price = amount,
                        ProductId = productId,
                        ProductName = description,
                        Qty = 1,
                    }
                }
            };
        }
    }
}