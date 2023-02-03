using Parkway.CBS.Payee.PayeeAdapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Payee
{

    public class PayeModel
    {
        public Dictionary<string, string> LineValues { get; set; }
    }


    /// <summary>
    /// Response to get paye response
    /// </summary>
    public class GetPayeResponse<PR> where PR : class
    {
        /// <summary>
        /// check to see if there are any errors here, that is if the header values have been read to be in correct order
        /// </summary>
        public HeaderValidateObject HeaderValidateObject { get; set; }

        /// <summary>
        /// if HeaderValidateObject.Error is false, get the list of pay roll from the file uploaded
        /// </summary>
        public List<PR> Payees { get; set; }
    }



    /// <summary>
    /// Response to get paye response
    /// </summary>
    public class GetPayeResponse
    {
        /// <summary>
        /// check to see if there are any errors here, that is if the header values have been read to be in correct order
        /// </summary>
        public HeaderValidateObject HeaderValidateObject { get; set; }

        /// <summary>
        /// if HeaderValidateObject.Error is false, get the list of PayeeAssessmentLineRecordModel
        /// </summary>
        public List<PayeeAssessmentLineRecordModel> Payes { get; set; }
    }


    public class PayeeAssessmentLineRecordModel : LineRecordModel
    {
        public PayeeStringValue TaxPayerName { get; internal set; }

        public PayeeStringValue TaxPayerTIN { get; internal set; }

        public PayeeStringValue TaxPayerId { get; internal set; }

        public PayeeStringValue LGA { get; internal set; }

        public PayeeStringValue Address { get; internal set; }

        public PayeeDecimalValue GrossAnnualEarnings { get; internal set; }

        public PayeeDecimalValue Exemptions { get; internal set; }        

        public PayeeStringValue Email { get; internal set; }

        public PayeeStringValue Phone { get; internal set; }
    }


    public abstract class BasePayeeValue
    {
        public bool HasError { get; internal set; }

        public string ErrorMessage { get; internal set; }
    }

    public class PayeeDecimalValue : BasePayeeValue
    {
        public decimal Value { get; internal set; }

        public string StringValue { get; internal set; }
    }


    public class PayeeIntValue : BasePayeeValue
    {
        public int Value { get; internal set; }

        public string StringValue { get; internal set; }
    }


    public class PayeeStringValue : BasePayeeValue
    {
        public string Value { get; internal set; }
    }


    public class HeaderValidateObject
    {
        public bool Error { get; set; }

        public string ErrorMessage { get; set; }
    }

    public class HeaderValidationModel
    {
        public bool HeaderPresent { get; internal set; }

        public string ErrorMessage { get; internal set; }

        public int IndexOnFile { get; internal set; }

    }
}
