using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Module.ViewModels
{
    public class RevenueHeadAndBillingInformation
    {
        public int RevennueHeadId { get; set; }
        public string RevennueHeadName { get; set; }
        public string RevennueHeadSlug { get; set; }
        public string RevennueHeadCode { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsActive { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public decimal Amount { get; set; }
        public bool IsPrepaid { get; set; }
    }
}