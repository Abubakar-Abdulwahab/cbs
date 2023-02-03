using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class DiscountModel
    {
        public BillingDiscountType BillingDiscountType { get; set; }
        public EffectiveFromType EffectiveFromType { get; set; }
        public decimal Discount { get; set; }
        public int EffectiveFrom { get; set; }

        /// <summary>
        /// Set the discount value helper property
        /// </summary>
        public decimal DiscountValue { get; set; }
    }
}