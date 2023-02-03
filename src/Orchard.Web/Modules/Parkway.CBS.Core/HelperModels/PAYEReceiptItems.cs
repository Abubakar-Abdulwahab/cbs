using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class PAYEReceiptItems
    {
        public string PayerName { get; set; }

        public decimal AnnualEarnings { get; set; }

        public decimal Exemptions { get; set; }

        public decimal IncomeTaxValue { get; set; }

        public string PayerId { get; set; }

        public string ReceiptNumber { get; set; }

        public int MonthId { get; set; }

        public string Month { get { return ((Months)MonthId).ToString(); } }

        public int Year { get; set; }
    }
}