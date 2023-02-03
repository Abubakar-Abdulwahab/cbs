using Newtonsoft.Json;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Module.ViewModels
{
    public class AdminGenerateInvoiceVM
    {
        public bool DoesNotHaveInput { get; set; }

        public decimal Amount { get; set; }

        public TaxPayerWithDetails TaxPayerWithDetails { get; set; }

        public Int64 TaxPayerId { get; set; }

        public int RevenueHeadId { get; set; }

        public string RevenueHeadName { get; set; }

        public string MDAName { get; set; }

        public string Reference { get; set; }

        public BillingType BillingType { get; set; }

        public List<FormControlViewModel> Forms { get; set; }

        public string PartialToShow { get; set; }

        public string errors { get; set; }
    }


    public class AdminConfirmingInvoiceVM
    {
        public decimal Amount { get; set; }

        public TaxPayerWithDetails TaxPayerWithDetails { get; set; }

        public Int64 TaxPayerId { get; set; }

        public int RevenueHeadId { get; set; }

        public string RevenueHeadName { get; set; }

        public string MDAName { get; set; }

        public string Reference { get; set; }

        public List<FormControlViewModel> Forms { get; set; }

        public BillingType BillingType { get; set; }

        [JsonIgnore]
        public string SubToken { get; set; }

        [JsonIgnore]
        public string Token { get; set; }
    }
}