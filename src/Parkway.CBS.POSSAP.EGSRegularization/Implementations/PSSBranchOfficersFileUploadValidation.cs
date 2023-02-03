using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.POSSAP.EGSRegularization.HelperModel;
using Parkway.CBS.POSSAP.EGSRegularization.Implementations.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.POSSAP.EGSRegularization.Implementations
{
    public class PSSBranchOfficersFileUploadValidation : IPSSBranchOfficersFileUploadValidation
    {

        /// <summary>
        /// Validates PSSBranchOfficers line items
        /// </summary>
        /// <param name="lineValues"></param>
        /// <returns></returns>
        public PSSBranchOfficersItemVM ValidateExtractedPSSBranchOfficersLineItems(List<string> lineValues)
        {
            string errorMessage = string.Empty;

            PSSBranchOfficersItemVM lineItem = new PSSBranchOfficersItemVM
            {
                Rank = new PoliceRankingVM(),
                OfficerCommand = new CommandVM(),
                BranchCode = ValidateStringLength(lineValues.ElementAt(0), "Branch Code", false, ref errorMessage, 3, 255)
            };

            if (!string.IsNullOrEmpty(errorMessage)) { lineItem.ErrorMessage = errorMessage + "\n| "; }

            lineItem.APNumber = ValidateStringLength(lineValues.ElementAt(1), "AP Number", false, ref errorMessage, 3, 100);
            if (!string.IsNullOrEmpty(errorMessage)) { lineItem.ErrorMessage += errorMessage + "\n| "; }

            if (!string.IsNullOrEmpty(lineItem.ErrorMessage)) 
            { 
                lineItem.HasError = true;
                lineItem.ErrorMessage = lineItem.ErrorMessage.Trim().TrimEnd('|');
            }

            return lineItem;
        }

        /// <summary>
        /// validate the length of a stringValue
        /// </summary>
        /// <param name="stringValue">string value</param>
        /// <param name="headerValue">excel header or label value </param>
        /// <param name="allowEmpty">allow empty value</param>
        /// <param name="errorMsg">error message</param>
        /// <param name="minValidLength">minimum string value length</param>
        /// <param name="maxValidLength">maximum string value length</param>
        /// <returns>validated string</returns>
        public static string ValidateStringLength(string stringValue, string headerValue, bool allowEmpty, ref string errorMsg, int minValidLength = 0, int maxValidLength = 0)
        {
            errorMsg = string.Empty;
            if (string.IsNullOrEmpty(stringValue) || string.IsNullOrWhiteSpace(stringValue))
            {
                if (allowEmpty) { return stringValue; }
                errorMsg = string.Format("{0} is empty.", headerValue);
                return stringValue;
            }
            stringValue = stringValue.Trim();

            if (minValidLength > 0 && stringValue.Length < minValidLength)
            {
                errorMsg = string.Format("{0} value is too small. Enter a valid {0}. Minimum number of characters is {1}", headerValue, minValidLength);
                return stringValue;
            }

            if (maxValidLength > 0 && stringValue.Length > maxValidLength)
            {
                errorMsg = string.Format("{0} value is too long. Enter a valid {0}. Maximum number of characters is {1}", headerValue, maxValidLength);
                return stringValue;
            }
            return stringValue;
        }
    }
}
