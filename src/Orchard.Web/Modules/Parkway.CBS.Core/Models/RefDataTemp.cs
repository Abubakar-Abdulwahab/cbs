using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models
{
    public class RefDataTemp : CBSBaseModel
    {
        public virtual long Id { get; set; }

        public virtual string Recipient { get; set; }

        public virtual string Address { get; set; }

        public virtual int TaxEntityCategoryId { get; set; }

        public virtual int BillingModelId { get; set; }

        public virtual int RevenueHeadId { get; set; }

        public virtual string TaxIdentificationNumber { get; set; }

        public virtual string Email { get; set; }

        public virtual string BatchNumber { get; set; }

        public virtual int Status { get; set; }

        public virtual string AdditionalDetails { get; set; }

        public virtual string ErrorLog { get; set; }

        public virtual string StatusDetail { get; set; }

        public virtual decimal Amount { get; set; }

        /// <summary>
        /// Concat of tax identification number
        /// </summary>
        public virtual string UniqueIdentifier { get; set; }


        public static string GetStatusDetails(ScheduleInvoiceProcessingStatus status)
        {
            switch (status)
            {
                case ScheduleInvoiceProcessingStatus.AquiredData:
                    return "Acquired ref data. Process level 1.";
                case ScheduleInvoiceProcessingStatus.ErrorInGettingJoinerWithTaxEntity:
                    return "Error getting a joiner between tax entity table and ref data table. Check query";
                case ScheduleInvoiceProcessingStatus.ErrorGettingDistinctRefDataEntitesWithoutCashflowRecord:
                    return "Error getting distinct ref data values and their duplicates";
                default:
                    return "No detail found";
            }
        }
    }    
}
