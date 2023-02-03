using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Parkway.CBS.Payee.PayeeAdapters
{
    public abstract class BasePayeeAdapter
    {
        /// <summary>
        ///  Tax Payer Name - 0, 
        ///  Tax Payer Tin - 1, 
        ///  Gross Annual Earning - 2,
        ///  Exemptions (Annual) - 3, 
        ///  Paye Month - 4, 
        ///  Paye Year - 5,
        ///  Email - 6,
        ///  Phone - 7,
        ///  Paye address - 8,
        ///  LGA - 9
        /// </summary>
        protected readonly string[] headerNames = { "Tax Payer Name".ToLower() , "Tax Payer Tin".ToLower(), "Gross Annual Earning".ToLower(),
            "Exemptions (Annual)".ToLower(), "Paye Month (e.g Jan)".ToLower(), "Paye Year (e.g 2018)".ToLower(), "Email".ToLower(), "Phone".ToLower(), "Paye address".ToLower(), "LGA".ToLower() };
        
        private Dictionary<string, int> months = new Dictionary<string, int> { { "Jan", 1 }, { "Feb", 2 }, { "Mar", 3 }, { "Apr", 4 }, { "May", 5 }, { "Jun", 6 }, { "Jul", 7 }, { "Aug", 8 }, { "Sep", 9 }, { "Oct", 10 }, { "Nov", 11 }, { "Dec", 12 } };


        /// <summary>
        /// Get the list of payeea
        /// <para>for onscreen assessment</para>
        /// </summary>
        /// <param name="directAssessmentPayeeLines"></param>
        /// <param name="LGAFilePath"></param>
        /// <param name="stateName"></param>
        /// <returns>List{PayeeAssessmentLineRecordModel}</returns>
        protected virtual List<PayeeAssessmentLineRecordModel> GetPayees(ICollection<DirectAssessmentPayeeLine> directAssessmentPayeeLines, string LGAFilePath, string stateName)
        {
            List<PayeeAssessmentLineRecordModel> payes = new List<PayeeAssessmentLineRecordModel> { };
            foreach (var item in directAssessmentPayeeLines)
            {
                List<string> lineValues = new List<string>
                {
                    { item.TaxPayerName },//tax payer name
                    { item.TIN }, //tax payer tin
                    { item.GrossAnnualEarning }, //gross annual
                    { item.Exemptions }, //exemptions
                    { item.Month }, //month
                    { item.Year }, //year
                    { item.Email }, // email
                    { item.PhoneNumber }, //phone number
                    { item.Address }, //Paye address
                    { item.LGA }, //LGA
                };

                payes.Add(ValidatePayeModel(lineValues, LGAFilePath, stateName));
            }
            return payes;
        }


        protected PayeeStringValue ValidateLGA(string LGAFilePath, string stateName, string userLGAInput)
        {
            if (string.IsNullOrEmpty(userLGAInput))
            {
                return new PayeeStringValue { };
            }

            if (userLGAInput.Length > 100)
            {
                return new PayeeStringValue { Value = Truncate(userLGAInput, 100), HasError = true, ErrorMessage = "LGA value is too long. This field allows a maximum of 100 characters" };
            }

            return new PayeeStringValue { Value = userLGAInput };
            //blame Fregene for this, he told me this field was compulsory. 
            //Whenever he changes his mind again, just uncommment the commented below and test. Always test!
            //{ insert expletive } Fregene

            //Dictionary<string, string> lgaAndValue = new Dictionary<string, string>();
            //try
            //{
            //    foreach (XElement stateElement in XElement.Load($"{LGAFilePath}\\LGAs.xml").Elements(stateName))
            //    {
            //        foreach (XElement lgaElement in stateElement.Elements("lga"))
            //        {
            //            lgaAndValue.Add(lgaElement.Attribute("name").Value, lgaElement.Attribute("value").Value);
            //        }
            //    }
            //}
            //catch (Exception) { throw new Exception("Could not validate LGA"); }

            //var result = lgaAndValue.Where(lga => lga.Key == userLGAInput).FirstOrDefault();
            //if (result.Key == null) { return new PayeeStringValue { ErrorMessage = "Could not find LGA value", HasError = true, Value = userLGAInput }; }
        }


        /// <summary>
        /// Validate excel headers
        /// </summary>
        /// <param name="header"></param>
        /// <returns>HeaderValidateObject</returns>
        protected virtual void ValidateHeaders(DataRow header, ref Dictionary<string, HeaderValidationModel> headers)
        {
            string errorMessage = string.Empty;
            int counter = -1;
            foreach (object item in header.ItemArray)
            {
                if(item is DBNull) { break; }
                counter++;
                string sItem = ((string)item).Trim().ToLower();

                if (headerNames[0].Contains(sItem)) { headers[headerNames[0]] = new HeaderValidationModel { HeaderPresent = true, IndexOnFile = counter }; continue; }
                if (headerNames[1].Contains(sItem)) { headers[headerNames[1]] = new HeaderValidationModel { HeaderPresent = true, IndexOnFile = counter }; continue; }
                if (headerNames[2].Contains(sItem)) { headers[headerNames[2]] = new HeaderValidationModel { HeaderPresent = true, IndexOnFile = counter }; continue; }
                if (headerNames[3].Contains(sItem)) { headers[headerNames[3]] = new HeaderValidationModel { HeaderPresent = true, IndexOnFile = counter }; continue; }
                if (headerNames[4].Contains(sItem)) { headers[headerNames[4]] = new HeaderValidationModel { HeaderPresent = true, IndexOnFile = counter }; continue; }
                if (headerNames[5].Contains(sItem)) { headers[headerNames[5]] = new HeaderValidationModel { HeaderPresent = true, IndexOnFile = counter }; continue; }
                if (headerNames[6].Contains(sItem)) { headers[headerNames[6]] = new HeaderValidationModel { HeaderPresent = true, IndexOnFile = counter }; continue; }
                if (headerNames[7].Contains(sItem)) { headers[headerNames[7]] = new HeaderValidationModel { HeaderPresent = true, IndexOnFile = counter }; continue; }
                if (headerNames[8].Contains(sItem)) { headers[headerNames[8]] = new HeaderValidationModel { HeaderPresent = true, IndexOnFile = counter }; continue; }
                if (headerNames[9].Contains(sItem)) { headers[headerNames[9]] = new HeaderValidationModel { HeaderPresent = true, IndexOnFile = counter }; }
            }
        }

        

        protected GetPayeResponse GetPayees(string filePath, string LGAFilePath, string stateName)
        {
            DataSet result = new DataSet();
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                { result = reader.AsDataSet(); }
            }
            var sheet1 = result.Tables[0];
            var rows = sheet1.Rows;

            Dictionary<string, HeaderValidationModel> headers = new Dictionary<string, HeaderValidationModel>
            {
                { headerNames[0], new HeaderValidationModel { } }, { headerNames[1], new HeaderValidationModel { } }, { headerNames[2], new HeaderValidationModel { } }, { headerNames[3], new HeaderValidationModel { } },
                 { headerNames[4], new HeaderValidationModel { } }, { headerNames[5], new HeaderValidationModel { } }, { headerNames[6], new HeaderValidationModel { } }, { headerNames[7], new HeaderValidationModel { } }
            };
            
            ValidateHeaders(rows[0], ref headers);
            var invalidsHeaders = headers.Where(k => k.Value.HeaderPresent == false);
            if(invalidsHeaders.Count() > 0)
            {
                var msg = invalidsHeaders.Select(x => x.Key + " header not found").ToArray();
                return new GetPayeResponse { HeaderValidateObject = new HeaderValidateObject { Error = true, ErrorMessage = string.Join("\n", msg) } };
            }

            rows.RemoveAt(0);
            var col = sheet1.Columns;
            List<PayeeAssessmentLineRecordModel> payees = new List<PayeeAssessmentLineRecordModel> { };

            foreach (DataRow item in rows)
            {
                List<string> lineValues = new List<string>
                {
                    { item.ItemArray[headers[headerNames[0]].IndexOnFile].ToString()},//tax payer name
                    { item.ItemArray[headers[headerNames[1]].IndexOnFile].ToString() }, //tax payer tin
                    { item.ItemArray[headers[headerNames[2]].IndexOnFile].ToString() }, //gross annual
                    { item.ItemArray[headers[headerNames[3]].IndexOnFile].ToString() }, //exemptions
                    { item.ItemArray[headers[headerNames[4]].IndexOnFile].ToString()}, //month
                    { item.ItemArray[headers[headerNames[5]].IndexOnFile].ToString()}, //year
                    { item.ItemArray[headers[headerNames[6]].IndexOnFile].ToString()}, // email
                    { item.ItemArray[headers[headerNames[7]].IndexOnFile].ToString()}, //phone number
                    { item.ItemArray[headers[headerNames[8]].IndexOnFile].ToString() }, //Paye address
                    { item.ItemArray[headers[headerNames[9]].IndexOnFile].ToString()}, //LGA
                };
                payees.Add(ValidatePayeModel(lineValues, LGAFilePath, stateName));
            }
            return new GetPayeResponse { Payes = payees, HeaderValidateObject = new HeaderValidateObject { } };
        }        

        //protected GetPayeResponse GetPayees(string filePath, string LGAFilePath, string stateName)
        //{
        //    DataSet result = new DataSet();
        //    using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        //    {
        //        using (var reader = ExcelReaderFactory.CreateReader(stream))
        //        { result = reader.AsDataSet(); }
        //    }
        //    var sheet1 = result.Tables[0];
        //    var rows = sheet1.Rows;
        //    HeaderValidateObject headerIsValid = ValidateHeaders(rows[0]);

        //    if (headerIsValid.Error)
        //    {
        //        return new GetPayeResponse { HeaderValidateObject = headerIsValid, Payes = new List<PayeeAssessmentLineRecordModel> { } };
        //    }

        //    rows.RemoveAt(0);
        //    var col = sheet1.Columns;
        //    List<PayeeAssessmentLineRecordModel> payees = new List<PayeeAssessmentLineRecordModel> { };

        //    foreach (DataRow item in rows)
        //    {
        //        List<string> lineValues = new List<string>
        //        {
        //            { item.ItemArray[0].ToString()},//tax payer name
        //            { item.ItemArray[1].ToString() }, //tax payer tin
        //            { item.ItemArray[2].ToString() }, //gross annual
        //            { item.ItemArray[3].ToString() }, //exemptions
        //            { item.ItemArray[4].ToString()}, //month
        //            { item.ItemArray[5].ToString()}, //year
        //            { item.ItemArray[6].ToString()}, // email
        //            { item.ItemArray[7].ToString()}, //phone number
        //            { item.ItemArray[8].ToString() }, //Paye address
        //            { item.ItemArray[9].ToString()}, //LGA
        //        };

        //        payees.Add(ValidatePayeModel(lineValues, LGAFilePath, stateName));
        //    }
        //    return new GetPayeResponse { Payes = payees, HeaderValidateObject = headerIsValid };
        //}


        
        protected virtual PayeeAssessmentLineRecordModel ValidatePayeModel(List<string> lineValues, string LGAFilePath, string stateName)
        {
            var errorMessages = string.Empty;
            var hasError = false;

            var paye = new PayeeAssessmentLineRecordModel();

            //            `assessment date` (dd / MM / YYYY)
            //            `paye name`
            //            `phone number`
            //            `tax amount` (edited)

            //validate tax payer name
            var taxPayerName = ConvertToString(lineValues.ElementAt(0), "Paye Name", false, 0, 200);
            if (taxPayerName.HasError) { hasError = true; errorMessages = taxPayerName.ErrorMessage + "\n | "; }
            paye.TaxPayerName = taxPayerName;

            //validate TIN
            var taxTin = ConvertToString(lineValues.ElementAt(1), "Tax payer TIN", true, 0, 50);
            if (taxTin.HasError) { hasError = true; errorMessages += taxTin.ErrorMessage + "\n | "; }
            paye.TaxPayerTIN = taxTin;

            var grossEarning = ConvertToDecimal(lineValues.ElementAt(2), "Gross annual earnings", false, 1, 29);
            if (grossEarning.HasError) { hasError = true; errorMessages += grossEarning.ErrorMessage + "\n | "; }
            paye.GrossAnnualEarnings = grossEarning;

            var exemptions = ConvertToDecimal(lineValues.ElementAt(3), "Exemptions", true, 0, 29);
            if (exemptions.HasError) { hasError = true; errorMessages += exemptions.ErrorMessage + "\n | "; };
            paye.Exemptions = exemptions;

            var month = ValidateMonth(lineValues.ElementAt(4));
            if (month.HasError) { hasError = true; errorMessages += month.ErrorMessage + "\n | "; }
            paye.Month = month;

            var year = ValidateYear(lineValues.ElementAt(5), "Paye year", 4, 11);
            if (year.HasError) { hasError = true; errorMessages += year.ErrorMessage + "\n | "; }
            paye.Year = year;

            var email = ConvertToString(lineValues.ElementAt(6), "Tax payer email", true, 0, 50);
            if (email.HasError) { hasError = true; errorMessages += email.ErrorMessage + "\n | "; }
            paye.Email = email;

            var phoneNumber = ValidatePhoneNumber(lineValues.ElementAt(7));
            if (phoneNumber.HasError) { hasError = true; errorMessages += phoneNumber.ErrorMessage + "\n | "; }
            paye.Phone = phoneNumber;

            var address = ConvertToString(lineValues.ElementAt(8), "Paye address", true, 0, 1000);
            if (address.HasError) { hasError = true; errorMessages += address.ErrorMessage + "\n | "; }
            paye.Address = address;

            paye.LGA = ValidateLGA(LGAFilePath, stateName, lineValues.ElementAt(9));// new PayeeStringValue { Value = item.LGA };
            if (paye.LGA.HasError) { hasError = true; errorMessages += paye.LGA.ErrorMessage + "\n"; }

            if (!hasError)
            {
                paye.AssessmentDate = GetAssessmentDate(paye.Year, paye.Month);
                if (paye.AssessmentDate == null)
                {
                    if (!paye.Month.HasError && !paye.Year.HasError)
                    {
                        hasError = true;
                        errorMessages = "Could not compute a date from the paye month and year. Enter valid month and year.";
                    }
                }
            }
            paye.HasError = hasError;
            errorMessages = errorMessages.Trim().TrimEnd('|');
            paye.ErrorMessages = errorMessages;
            return paye;
        }


        /// <summary>
        /// Get assessment date this paye is for
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns>DateTime | null</returns>
        protected DateTime? GetAssessmentDate(PayeeIntValue year, PayeeIntValue month)
        {
            if (year.HasError) { return null; }
            if (month.HasError) { return null; }
            
            return new DateTime(year.Value, month.Value, 1);
        }


        /// <summary>
        /// Truncate string value
        /// </summary>
        /// <param name="stringValue"></param>
        /// <param name="length"></param>
        /// <returns>string</returns>
        private string Truncate(string stringValue, int length)
        {
            string endAppend = string.Empty;
            if (length > 6) { length -= 3; endAppend = "..."; }
            return stringValue.Substring(0, length) + endAppend;
        }


        /// <summary>
        /// Validate phonenumber
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns>PayeeStringValue</returns>
        protected virtual PayeeStringValue ValidatePhoneNumber(string phoneNumber, bool ignoreError = false)
        {
            if (string.IsNullOrEmpty(phoneNumber)) { return new PayeeStringValue { HasError = true, ErrorMessage = "Add a valid phone number." }; }
            phoneNumber = phoneNumber.Trim().Replace(" ", string.Empty);
            phoneNumber = phoneNumber.Replace("-", string.Empty).Trim();
            if (phoneNumber.Length > 20 || phoneNumber.Length < 6)
            { return new PayeeStringValue { HasError = true, ErrorMessage = "Enter a valid phone number." }; }
            long lphoneNumber = 0;
            bool isANumber = long.TryParse(phoneNumber, out lphoneNumber);
            if (!isANumber) { return new PayeeStringValue { ErrorMessage = "Paye phone number is not valid. Add a valid phone number.", HasError = true, Value = phoneNumber }; }
            return new PayeeStringValue { Value = phoneNumber };
        }


        ///// <summary>
        ///// Validate excel headers
        ///// </summary>
        ///// <param name="header"></param>
        ///// <returns>HeaderValidateObject</returns>
        //protected HeaderValidateObject ValidateHeaders(DataRow header)
        //{
        //    bool headerError = false;
        //    string errorMessage = string.Empty;
        //    if (!header.ItemArray[0].ToString().Trim().ToLower().Contains("Tax Payer Name".ToLower())) { headerError = true; errorMessage = "Tax Payer Name header not found \n"; }
        //    if (!header.ItemArray[1].ToString().Trim().ToLower().Contains("Tax Payer Tin".ToLower())) { headerError = true; errorMessage = "Tax Payer TIN header not found \n"; }
        //    if (!header.ItemArray[2].ToString().Trim().ToLower().Contains("Gross Annual Earning".ToLower())) { headerError = true; errorMessage = "Gross Annual Earning header not found \n"; }
        //    if (!header.ItemArray[3].ToString().Trim().ToLower().Contains("Exemptions (Annual)".ToLower())) { headerError = true; errorMessage = "Exemptions (Annual) header not found \n"; }
        //    if (!header.ItemArray[4].ToString().Trim().ToLower().Contains("Paye Month".ToLower())) { headerError = true; errorMessage = "Paye Month header not found \n"; }
        //    if (!header.ItemArray[5].ToString().Trim().ToLower().Contains("Paye Year".ToLower())) { headerError = true; errorMessage = "Paye Year header not found \n"; }
        //    if (!header.ItemArray[6].ToString().Trim().ToLower().Contains("Email".ToLower())) { headerError = true; errorMessage = "Email header not found \n"; }
        //    if (!header.ItemArray[7].ToString().Trim().ToLower().Contains("Phone".ToLower())) { headerError = true; errorMessage = "Phone header not found \n"; }
        //    //if (header.ItemArray[8].ToString().Trim().ToLower() != "Address".ToLower()) { headerError = true; errorMessage = "Address header not found \n"; }
        //    //if (header.ItemArray[9].ToString().Trim().ToLower() != "LGA".ToLower()) { headerError = true; errorMessage = "LGA header not found \n"; }
        //    return new HeaderValidateObject { Error = headerError, ErrorMessage = errorMessage };
        //}


        /// <summary>
        /// Validate month
        /// </summary>
        /// <param name="stringValue"></param>
        /// <returns>PayeeStringValue</returns>
        protected PayeeIntValue ValidateMonth(string stringValue)
        {
            var result = ConvertToString(stringValue, "Paye month", false, 0, 11);
            if (result.HasError) { return new PayeeIntValue { ErrorMessage = "Month value not found. Expected format are Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec.", HasError = true, StringValue = stringValue }; }

            var value = months.Where(m => m.Key.ToLower() == stringValue.Trim().ToLower()).FirstOrDefault();

            if (value.Key == null)
            { return new PayeeIntValue { ErrorMessage = "Month value not found. Expected format are Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec.", HasError = true, StringValue = stringValue }; }

            return new PayeeIntValue { StringValue = stringValue.Trim(), Value = value.Value };
        }


        /// <summary>
        /// Get paye break down
        /// </summary>
        /// <param name="annualTaxableIncome"></param>
        /// <param name="onePercent"></param>
        /// <param name="rules"></param>
        /// <returns>PayeeBreakDown</returns>
        internal PayeeBreakDown GetBreakDown(decimal annualTaxableIncome, decimal onePercent, List<TaxRuleSet> rules)
        {
            var grandTax = GetTaxableAmount(annualTaxableIncome, 0, rules.Count, rules);
            if (grandTax < onePercent) { grandTax = onePercent; }
            decimal tax = Math.Round((grandTax / 12), 2);
            return new PayeeBreakDown { Tax = tax, Taxable = annualTaxableIncome, TaxStringValue = string.Format("{0:n2}", tax) };
        }


        /// <summary>
        /// Calculate the taxable amount
        /// </summary>
        /// <param name="taxableIncome"></param>
        /// <param name="stopCrit"></param>
        /// <param name="rulesCount"></param>
        /// <param name="rules"></param>
        /// <returns>decimal</returns>
        private decimal GetTaxableAmount(decimal taxableIncome, int stopCrit, int rulesCount, List<TaxRuleSet> rules)
        {
            decimal amount = 0.00m;
            if (rulesCount == 0 || rules == null || rules.Count <= 0) { return 0.00m; }
            while (taxableIncome >= 0.00m)
            {
                decimal grade = 0.00m;
                if (taxableIncome < rules[stopCrit].Amount)
                { grade = Math.Round(((taxableIncome * rules[stopCrit].Percentage) / 100), 2); }
                else
                { grade = Math.Round(((rules[stopCrit].Amount * rules[stopCrit].Percentage) / 100), 2); }

                taxableIncome = Math.Round((taxableIncome - rules[stopCrit].Amount), 2);

                amount += grade;
                int stopIt = stopCrit + 1;
                if (stopIt < rulesCount) { stopCrit++; }
            }
            return amount;
        }


        /// <summary>
        /// Convert or validate string value to int
        /// </summary>
        /// <param name="stringValue">string value</param>
        /// <param name="headerValue">header value</param>
        /// <param name="minValidLength">minimum valid string length</param>
        /// <param name="maxValidLength">maximum string length</param>
        /// <returns>PayeeIntValue</returns>
        protected PayeeIntValue ValidateYear(string stringValue, string headerValue, int minValidLength = 0, int maxValidLength = 0)
        {
            //trim data
            if (string.IsNullOrEmpty(stringValue))
            { return new PayeeIntValue { ErrorMessage = string.Format("{0} is empty.", headerValue), HasError = true, StringValue = stringValue }; }

            if (minValidLength > 0)
            {
                if (stringValue.Length < minValidLength) { return new PayeeIntValue { ErrorMessage = string.Format("{0} value is small. Enter a valid {0}.", headerValue), HasError = true, StringValue = stringValue }; }
            }

            if (stringValue.Length > 4)
            {
                if (stringValue.Length >= 11)
                {
                    return new PayeeIntValue { ErrorMessage = string.Format("{0} value is too long. Enter a valid {0}.", headerValue), HasError = true, StringValue = Truncate(stringValue, 11) };
                }
                else
                {
                    return new PayeeIntValue { ErrorMessage = string.Format("{0} value is too long. Enter a valid {0}.", headerValue), HasError = true, StringValue = stringValue };
                }

            }

            int intValue = 0;
            //try converting
            if (!Int32.TryParse(stringValue.Trim(), out intValue))
            {
                return new PayeeIntValue { ErrorMessage = string.Format("{0} is not a valid number.", headerValue), HasError = true, StringValue = stringValue };
            }

            if (intValue <= 0)
            {
                return new PayeeIntValue { StringValue = intValue.ToString(), HasError = true, ErrorMessage = string.Format("{0} is not valid.", headerValue) };
            }

            return new PayeeIntValue { Value = intValue, StringValue = intValue.ToString() };
        }


        /// <summary>
        /// validate the length of a stringValue
        /// </summary>
        /// <param name="stringValue">string value</param>
        /// <param name="headerValue">excel header or label value </param>
        /// <param name="allowEmpty">allow empty value</param>
        /// <param name="minValidLength">minimum string value length</param>
        /// <param name="maxValidLength">maximum string value length</param>
        /// <returns>PayeeStringValue</returns>
        protected PayeeStringValue ValidateLength(string stringValue, string headerValue, bool allowEmpty, int minValidLength = 0, int maxValidLength = 0)
        {
            if (string.IsNullOrEmpty(stringValue))
            {
                if (allowEmpty) { return new PayeeStringValue { Value = stringValue }; }
                return new PayeeStringValue { ErrorMessage = string.Format("{0} is empty.", headerValue), HasError = true, Value = stringValue };
            }
            stringValue = stringValue.Trim();

            if (minValidLength > 0)
            {
                if (stringValue.Length < minValidLength)
                { return new PayeeStringValue { ErrorMessage = string.Format("{0} value is too small. Enter a valid {0}. Minimum number of characters is {1}", headerValue, minValidLength), HasError = true, Value = stringValue }; }
            }

            if (maxValidLength > 0)
            {
                if (stringValue.Length > maxValidLength)
                { return new PayeeStringValue { ErrorMessage = string.Format("{0} value is too long. Enter a valid {0}. Maximum number of characters is {1}", headerValue, maxValidLength), HasError = true, Value = Truncate(stringValue, maxValidLength) }; }
            }
            return new PayeeStringValue { };
        }


        /// <summary>
        /// Validate value to string
        /// </summary>
        /// <param name="stringValue">value to be validated against</param>
        /// <param name="headerValue">name of the excel header value</param>
        /// <param name="allowEmpty">allow empty string values</param>
        /// <param name="minValidLength">minimum length of the string</param>
        /// <param name="maxValidLength">maximum length</param>
        /// <param name="ignoreError">if the value validated has an error, this field indicates whether we should ignore the error or not</param>
        /// <returns>PayeeStringValue</returns>
        protected virtual PayeeStringValue ConvertToString(string stringValue, string headerValue, bool allowEmpty = false, int minValidLength = 0, int maxValidLength = 0, bool ignoreError = false)
        {
            var result = ValidateLength(stringValue, headerValue, allowEmpty, minValidLength, maxValidLength);
            if (result.HasError) { if (ignoreError) { result.HasError = false; } return result; }
            return new PayeeStringValue { Value = string.IsNullOrEmpty(stringValue) ? stringValue : stringValue.Trim() };
        }


        /// <summary>
        /// Convert and validate string value to decimal
        /// </summary>
        /// <param name="stringValue">string value</param>
        /// <param name="headerValue">excel header or label header</param>
        /// <param name="allowZero">allow zero</param>
        /// <param name="minValidLength">minimum valid length</param>
        /// <param name="maxValidLength">maximum valid length</param>
        /// <returns>PayeeDecimalValue</returns>
        protected PayeeDecimalValue ConvertToDecimal(string stringValue, string headerValue, bool allowZero = false, int minValidLength = 0, int maxValidLength = 0)
        {
            if (string.IsNullOrEmpty(stringValue))
            {
                return new PayeeDecimalValue
                { StringValue = String.Format("{0:n0}", 0.00m), ErrorMessage = string.Format("{0} is empty.", headerValue), HasError = true, Value = 0.00m };
            }

            if (minValidLength > 0)
            {
                if (stringValue.Length < minValidLength)
                {
                    if (!allowZero)
                    {
                        return new PayeeDecimalValue { HasError = true, StringValue = stringValue, Value = 0.00m, ErrorMessage = string.Format("{0} value is too short. Minimum characters allowed is {1}.", headerValue, minValidLength) };
                    }
                }
            }

            if (maxValidLength > 0)
            {
                if (stringValue.Length > maxValidLength)
                {
                    return new PayeeDecimalValue { ErrorMessage = string.Format("{0} value is too long. Maximum characters allowed is {1}.", headerValue, maxValidLength), HasError = true, StringValue = Truncate(stringValue, maxValidLength), Value = 0.00m };
                }
            }

            //trim and remove comma
            stringValue = stringValue.Trim().Replace(",", "");
            //remove white space between digits
            decimal decimalValue = 0.00m;
            stringValue = Regex.Replace(stringValue, @"\s+", "");
            //try converting
            if (!decimal.TryParse(stringValue, out decimalValue))
            {
                return new PayeeDecimalValue { StringValue = stringValue, ErrorMessage = string.Format("{0} is not a valid amount.", headerValue), HasError = true, };
            }

            if (decimalValue <= 0 && !allowZero)
            {
                return new PayeeDecimalValue { StringValue = stringValue, HasError = true, ErrorMessage = string.Format("{0} is not a valid amount.", headerValue) };
            }
            decimalValue = Math.Round(decimalValue, 2);
            return new PayeeDecimalValue { Value = decimalValue, StringValue = string.Format("{0:n}", decimalValue) };
        }
    }
}
