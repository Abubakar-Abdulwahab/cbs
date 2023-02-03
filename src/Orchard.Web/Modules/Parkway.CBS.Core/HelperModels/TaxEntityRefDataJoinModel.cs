using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class ProcessResponseModel
    {
        public bool HasErrors { get; set; }

        public string ErrorMessage { get; set; }

        public dynamic MethodReturnObject { get; set; }
    }

    public class RefDataAndCashflowDetails
    {
        public Int64 CashflowCustomerId { get; set; }

        public Int64 CashflowPrimaryContactId { get; set; }

        public string TaxIdentificationNumber { get; set; }

        public string Recipient { get; set; }

        public string Address { get; set; }

        public int TaxEntityCategoryId { get; set; }

        public string Email { get; set; }

        public string AdditionalDetails { get; set; }


        public decimal Amount { get; set; }
    }
}