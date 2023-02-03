using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.POSSAP.EGSRegularization.HelperModel;
using Parkway.CBS.POSSAP.EGSRegularization.Implementations.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.POSSAP.EGSRegularization.Implementations
{
    public class PSSBranchSubUsersFileUploadValidation : IPSSBranchSubUsersFileUploadValidation
    {

        /// <summary>
        /// Validates PSSBranchSubUsers line items
        /// </summary>
        /// <param name="lineValues"></param>
        /// <returns></returns>
        public PSSBranchSubUsersItemVM ValidateExtractedPSSBranchSubUsersLineItems(List<string> lineValues)
        {
            string errorMessage = string.Empty;
            List<ErrorModel> emailErrors = new List<ErrorModel>();

            PSSBranchSubUsersItemVM lineItem = new PSSBranchSubUsersItemVM() { BranchLGA = new Core.HelperModels.LGAVM(), BranchState = new Core.HelperModels.StateModelVM() };

            lineItem.BranchStateCode = ValidateStringLength(lineValues.ElementAt(0), "Branch state code", false, ref errorMessage, 3, 50);
            if (!string.IsNullOrEmpty(errorMessage)) { lineItem.ErrorMessage = errorMessage + "\n| "; }

            lineItem.BranchLGACode = ValidateStringLength(lineValues.ElementAt(1), "Branch LGA code", false, ref errorMessage, 3, 50);
            if (!string.IsNullOrEmpty(errorMessage)) { lineItem.ErrorMessage += errorMessage + "\n| "; }

            lineItem.BranchName = ValidateStringLength(lineValues.ElementAt(2), "Branch name", false, ref errorMessage, 5, 255);
            if (!string.IsNullOrEmpty(errorMessage)) { lineItem.ErrorMessage += errorMessage + "\n| "; }

            lineItem.BranchAddress = ValidateStringLength(lineValues.ElementAt(3), "Branch address", false, ref errorMessage, 10, 255);
            if (!string.IsNullOrEmpty(errorMessage)) { lineItem.ErrorMessage += errorMessage + "\n| "; }

            lineItem.SubUserName = ValidateStringLength(lineValues.ElementAt(4), "Sub user name", false, ref errorMessage, 5, 200);
            if (!string.IsNullOrEmpty(errorMessage)) { lineItem.ErrorMessage += errorMessage + "\n| "; }

            lineItem.SubUserEmail = ValidateStringLength(lineValues.ElementAt(5), "Sub user email", false, ref errorMessage, 10, 200);
            if (!string.IsNullOrEmpty(errorMessage)) { lineItem.ErrorMessage += errorMessage + "\n| "; }
            if (!DoEmailValidation(lineItem.SubUserEmail, ref emailErrors, "Sub user email")) { lineItem.ErrorMessage += emailErrors[0].ErrorMessage + "\n| "; }

            string phoneNumber = ValidateStringLength(lineValues.ElementAt(6), "Sub user phone number", false, ref errorMessage, 11, 14);
            if (string.IsNullOrEmpty(errorMessage)) { if (!DoPhoneNumberValidation(ref phoneNumber)) { lineItem.ErrorMessage += "Phone Number is not valid.\n| "; } else { lineItem.SubUserPhoneNumber = phoneNumber; } }
            else { lineItem.ErrorMessage += $"{errorMessage}\n| "; }
            if (!string.IsNullOrEmpty(lineItem.ErrorMessage))
            {
                lineItem.ErrorMessage = lineItem.ErrorMessage.Trim().TrimEnd('|');
            }

            if (!string.IsNullOrEmpty(lineItem.ErrorMessage)) { lineItem.HasError = true; }

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
        private string ValidateStringLength(string stringValue, string headerValue, bool allowEmpty, ref string errorMsg, int minValidLength = 0, int maxValidLength = 0)
        {
            errorMsg = string.Empty;
            if (string.IsNullOrEmpty(stringValue) || string.IsNullOrWhiteSpace(stringValue))
            {
                if (allowEmpty) { return stringValue; }
                errorMsg = string.Format("{0} is empty.", headerValue);
                return stringValue;
            }
            stringValue = stringValue.Trim();

            if (minValidLength > 0)
            {
                if (stringValue.Length < minValidLength)
                {
                    errorMsg = string.Format("{0} value is too small. Enter a valid {0}. Minimum number of characters is {1}.", headerValue, minValidLength);
                    return stringValue;
                }
            }

            if (maxValidLength > 0)
            {
                if (stringValue.Length > maxValidLength)
                {
                    errorMsg = string.Format("{0} value is too long. Enter a valid {0}. Maximum number of characters is {1}.", headerValue, maxValidLength);
                    return stringValue;
                }
            }
            return stringValue;
        }

        /// <summary>
        /// Check if this phone number is valid
        /// </summary>
        /// <param name="sPhoneNumber"></param>
        /// <returns>bool</returns>
        private bool DoPhoneNumberValidation(ref string sPhoneNumber)
        {
            if (string.IsNullOrEmpty(sPhoneNumber))
            { return false; }
            sPhoneNumber = Util.NormalizePhoneNumber(sPhoneNumber);
            //validate phone number
            bool isANumber = long.TryParse(sPhoneNumber, out _);
            if (!isANumber || (sPhoneNumber.Length != 13 && sPhoneNumber.Length != 11))
            { return false; }
            return true;
        }

        /// <summary>
        /// Do email validation
        /// </summary>
        /// <param name="email"></param>
        /// <param name="errors"></param>
        /// <param name="fieldName"></param>
        public bool DoEmailValidation(string email, ref List<ErrorModel> errors, string fieldName, bool compulsory = true)
        {
            Util.DoEmailValidation(email, ref errors, fieldName, compulsory);
            return errors.Count == 0;
        }
    }
}
