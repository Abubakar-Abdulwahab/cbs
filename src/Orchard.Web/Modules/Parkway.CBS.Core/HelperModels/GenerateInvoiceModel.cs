using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.Core.HelperModels
{
    public class GenerateInvoiceModel
    {
        public TaxEntity Entity { get; set; }

        public ExpertSystemSettings ExpertSystem { get; set; }

        public TenantCBSSettings StateSettings { get; set; }

        public MDA Mda { get; set; }

        public RevenueHead RevenueHead { get; set; }

        public BillingModel Billing { get; set; }


        public TaxEntityCategory Category { get; set; }

        public decimal Amount { get; set; }

        public List<AdditionalDetails> AddtionalDetails { get; set; }

        public DateTime InvoiceDate { get; set; }

        public bool PaymentStatus { get; set; }

        public string RequestReference { get; set; }

        public string ExternalRefNumber { get; set; }

        public UserPartRecord AdminUser { get; set; }

        public string InvoiceDescription { get; set; }

        public string InvoiceTitle { get; set; }

        public string CallBackURL { get; set; }

        public int Quantity { get; set; }

        public decimal VAT { get; set; }

        public decimal Surcharge { get; set; }

        public List<UserFormDetails> FormValues { get; set; }
    }
}