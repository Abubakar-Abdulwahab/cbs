using Parkway.CBS.TaxPayerEnumerationService.Implementations.Contracts;
using Parkway.CBS.TaxPayerEnumerationService.Models;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.TaxPayerEnumerationService.Implementations
{
    public class TaxPayerEnumerationValidation : ITaxPayerEnumerationValidation
    {


        /// <summary>
        /// Returns validated and unique enumeration items.
        /// </summary>
        /// <param name="lineItems"></param>
        /// <returns></returns>
        public dynamic GetValidatedEnumerationItems(dynamic lineItems)
        {
            List<TaxPayerEnumerationLine> validatedItems = new List<TaxPayerEnumerationLine>();
            foreach (var lineItem in lineItems)
            {
                List<string> lineValue = new List<string>
                {
                    lineItem.Name,
                    lineItem.PhoneNumber,
                    lineItem.Email,
                    lineItem.TIN,
                    lineItem.LGA,
                    lineItem.Address,
                };
                //validate line items and then add to validatedItems list.
                validatedItems.Add(ValidateEnumerationLineItem(lineValue));
            }
            return GetUniqueEnumerationItems(validatedItems); //ensure there are no duplicate line items
        }


        /// <summary>
        /// Validates enumeration line items
        /// </summary>
        /// <param name="lineValues"></param>
        /// <returns></returns>
        public TaxPayerEnumerationLine ValidateEnumerationLineItem(List<string> lineValues)
        {
            string errorMessage = string.Empty;

            TaxPayerEnumerationLine lineItem = new TaxPayerEnumerationLine();

            var taxPayerName = ValidateStringLength(lineValues.ElementAt(0), nameof(TaxPayerEnumerationLine.Name), false, ref errorMessage, 5, 255);
            lineItem.Name = taxPayerName;
            if (!string.IsNullOrEmpty(errorMessage)) { lineItem.ErrorMessages = errorMessage + "\n |"; }

            var phoneNumber = ValidateStringLength(lineValues.ElementAt(1), nameof(TaxPayerEnumerationLine.PhoneNumber), false, ref errorMessage, 11, 14);
            lineItem.PhoneNumber = phoneNumber;
            if (string.IsNullOrEmpty(errorMessage)) { if (!DoPhoneNumberValidation(lineItem.PhoneNumber)) { lineItem.ErrorMessages += "Phone Number is not valid. \n |"; } }
            else { lineItem.ErrorMessages += $"{errorMessage} \n |"; }

            var email = ValidateStringLength(lineValues.ElementAt(2), nameof(TaxPayerEnumerationLine.Email), false, ref errorMessage, 3, 100);
            lineItem.Email = email;
            if (!string.IsNullOrEmpty(errorMessage)) { lineItem.ErrorMessages += errorMessage + "\n |"; }

            var tin = ValidateStringLength(lineValues.ElementAt(3), nameof(TaxPayerEnumerationLine.TIN), true, ref errorMessage, 5, 100);
            lineItem.TIN = tin;
            if (!string.IsNullOrEmpty(errorMessage)) { lineItem.ErrorMessages += errorMessage + "\n |"; }

            var lga = ValidateStringLength(lineValues.ElementAt(4), nameof(TaxPayerEnumerationLine.LGA), false, ref errorMessage, 3, 100);
            lineItem.LGA = lga;
            if (!string.IsNullOrEmpty(errorMessage)) { lineItem.ErrorMessages += errorMessage + "\n |"; }

            var address = ValidateStringLength(lineValues.ElementAt(5), nameof(TaxPayerEnumerationLine.Address), false, ref errorMessage, 10, 200);
            lineItem.Address = address;
            if (!string.IsNullOrEmpty(errorMessage)) { lineItem.ErrorMessages += errorMessage + "\n |"; }

            if (!string.IsNullOrEmpty(lineItem.ErrorMessages)) { lineItem.HasError = true; }

            return lineItem;
        }


        /// <summary>
        /// Ensures there are no duplicate entries in the enumertion collection.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public List<TaxPayerEnumerationLine> GetUniqueEnumerationItems(List<TaxPayerEnumerationLine> items)
        {
            return items.Distinct(new TaxPayerEnumerationLineComparer()).ToList();
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
                    errorMsg = string.Format("{0} value is too small. Enter a valid {0}. Minimum number of characters is {1}", headerValue, minValidLength);
                    return stringValue;
                }
            }

            if (maxValidLength > 0)
            {
                if (stringValue.Length > maxValidLength)
                {
                    errorMsg = string.Format("{0} value is too long. Enter a valid {0}. Maximum number of characters is {1}", headerValue, maxValidLength);
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
        private bool DoPhoneNumberValidation(string sPhoneNumber)
        {
            if (string.IsNullOrEmpty(sPhoneNumber))
            { return false; }
            //validate phone number
            if (sPhoneNumber.Substring(0) == "+") { sPhoneNumber = sPhoneNumber.Replace("+", string.Empty); }
            long phoneNumber = 0;
            bool isANumber = long.TryParse(sPhoneNumber, out phoneNumber);
            if (!isANumber || (sPhoneNumber.Length != 13 && sPhoneNumber.Length != 11))
            { return false; }
            return true;
        }
    }
}
