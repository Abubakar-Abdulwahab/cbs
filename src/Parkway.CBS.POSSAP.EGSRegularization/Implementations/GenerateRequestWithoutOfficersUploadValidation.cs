using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.POSSAP.EGSRegularization.HelperModels;
using Parkway.CBS.POSSAP.EGSRegularization.Implementations.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.POSSAP.EGSRegularization.Implementations
{
    public class GenerateRequestWithoutOfficersUploadValidation : IGenerateRequestWithoutOfficersUploadValidation
    {
        /// <summary>
        /// Validates PSSBranchSubUsers line items
        /// </summary>
        /// <param name="lineValues"></param>
        /// <returns></returns>
        public GenerateRequestWithoutOfficersUploadItemVM ValidateExtractedGenerateRequestWithoutOfficersLineItems(List<string> lineValues)
        {
            string errorMessage = string.Empty;
            List<ErrorModel> emailErrors = new List<ErrorModel>();

            GenerateRequestWithoutOfficersUploadItemVM lineItem = new GenerateRequestWithoutOfficersUploadItemVM();

            lineItem.BranchCode = ValidateStringLength(lineValues.ElementAt(0), "Branch Code", false, ref errorMessage, 3, 50);
            if (!string.IsNullOrEmpty(errorMessage)) { lineItem.ErrorMessage = errorMessage + "\n| "; }

            lineItem.NumberOfOfficers = ValidateNumberOfOfficers(lineValues.ElementAt(1), "Number of officers", false, ref errorMessage, 1, 6);
            lineItem.NumberOfOfficersValue = lineValues.ElementAt(1);
            if (!string.IsNullOrEmpty(errorMessage)) { lineItem.ErrorMessage += errorMessage + "\n| "; }

            lineItem.CommandCode = ValidateStringLength(lineValues.ElementAt(2), "Command Code", false, ref errorMessage, 3, 20);
            if (!string.IsNullOrEmpty(errorMessage)) { lineItem.ErrorMessage += errorMessage + "\n| "; }

            lineItem.CommandTypeValue = ValidateCommandType(lineValues.ElementAt(3), "Command Type", false, ref errorMessage);
            if (!string.IsNullOrEmpty(errorMessage)) { lineItem.ErrorMessage += errorMessage + "\n| "; } else { lineItem.CommandType = GetCommandType(lineItem.CommandTypeValue); }

            lineItem.DayTypeValue = ValidateDayType(lineValues.ElementAt(4), "Day Type", false, ref errorMessage);
            if (!string.IsNullOrEmpty(errorMessage)) { lineItem.ErrorMessage += errorMessage + "\n| "; } else { lineItem.DayType = GetDayType(lineItem.DayTypeValue); }

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

        private int ValidateNumberOfOfficers(string stringValue, string headerValue, bool allowEmpty, ref string errorMsg, int minValidLength = 0, int maxValidLength = 0)
        {
            errorMsg = string.Empty;
            int result = 0;
            if (string.IsNullOrEmpty(stringValue) || string.IsNullOrWhiteSpace(stringValue))
            {
                if (allowEmpty) { return 0; }
                errorMsg = string.Format("{0} is empty.", headerValue);
                return result;
            }
            stringValue = stringValue.Trim();

            if (minValidLength > 0)
            {
                if (stringValue.Length < minValidLength)
                {
                    errorMsg = string.Format("{0} value is too small. Enter a valid {0}. Minimum number of characters is {1}.", headerValue, minValidLength);
                    return result;
                }
            }

            if (maxValidLength > 0)
            {
                if (stringValue.Length > maxValidLength)
                {
                    errorMsg = string.Format("{0} value is too long. Enter a valid {0}. Maximum number of characters is {1}.", headerValue, maxValidLength);
                    return result;
                }
            }
            if (!int.TryParse(stringValue, out result))
            {
                errorMsg = string.Format($"{stringValue} is not a valid number");
                return result;
            }
            if (result <= 0)
            {
                errorMsg = string.Format($"{headerValue} must be greater than zero (0)");
            }
            return result;
        }

        private string ValidateCommandType(string stringValue, string headerValue, bool allowEmpty, ref string errorMsg)
        {
            int minValidLength = 1, maxValidLength = 1;
            errorMsg = string.Empty;
            if (string.IsNullOrEmpty(stringValue) || string.IsNullOrWhiteSpace(stringValue))
            {
                if (allowEmpty) { return stringValue; }
                errorMsg = string.Format("{0} is empty.", headerValue);
                return stringValue;
            }
            stringValue = stringValue.Trim();

            if (stringValue.Length < minValidLength)
            {
                errorMsg = string.Format("{0} value is too small. Enter a valid {0}. Minimum number of characters is {1}.", headerValue, minValidLength);
                return stringValue;
            }

            if (stringValue.Length > maxValidLength)
            {
                errorMsg = string.Format("{0} value is too long. Enter a valid {0}. Maximum number of characters is {1}.", headerValue, maxValidLength);
                return stringValue;
            }
            if (!Enum.IsDefined(typeof(EGSCommandType), stringValue))
            {
                errorMsg = string.Format($"{headerValue} must be T or C");
            }
            return stringValue;
        }

        private string ValidateDayType(string stringValue, string headerValue, bool allowEmpty, ref string errorMsg)
        {
            int minValidLength = 1, maxValidLength = 1;
            errorMsg = string.Empty;
            if (string.IsNullOrEmpty(stringValue) || string.IsNullOrWhiteSpace(stringValue))
            {
                if (allowEmpty) { return stringValue; }
                errorMsg = string.Format("{0} is empty.", headerValue);
                return stringValue;
            }
            stringValue = stringValue.Trim();

            if (stringValue.Length < minValidLength)
            {
                errorMsg = string.Format("{0} value is too small. Enter a valid {0}. Minimum number of characters is {1}.", headerValue, minValidLength);
                return stringValue;
            }

            if (stringValue.Length > maxValidLength)
            {
                errorMsg = string.Format("{0} value is too long. Enter a valid {0}. Maximum number of characters is {1}.", headerValue, maxValidLength);
                return stringValue;
            }

            if (!Enum.IsDefined(typeof(DayType), stringValue))
            {
                errorMsg = string.Format($"{headerValue} must be H or F");
            }
            return stringValue;
        }

        private int GetCommandType(string stringValue)
        {
            return (int)Enum.Parse(typeof(EGSCommandType),stringValue, true);
        }

        private int GetDayType(string stringValue)
        {
            return (int)Enum.Parse(typeof(DayType), stringValue, true);
        }
    }
}
