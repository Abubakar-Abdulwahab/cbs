using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class TaxPaymentDetailVM
    {
        public string Month { get { return ((Months)MonthId).ToString(); } }

        public int MonthId { get; set; }

        public int Year { get; set; }

        public decimal TaxPaid { get; set; }
    }
}