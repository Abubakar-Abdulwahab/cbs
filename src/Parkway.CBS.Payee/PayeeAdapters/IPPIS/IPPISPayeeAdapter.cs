using ExcelDataReader;
using Parkway.CBS.Payee.Models;
using Parkway.CBS.Payee.PayeeAdapters.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Parkway.CBS.Payee.PayeeAdapters.IPPIS
{
    public class IPPISPayeeAdapter : BasePayeeAdapter, IIPPISPayeeAdapter
    {
        protected readonly string[] templateHeaderNames = { "MINISTRY_NAME".ToLower() , "ORG_CODE".ToLower(), "PERIOD_NAME".ToLower(), "EMPLOYEE_NUMBER".ToLower(),
            "EMPLOYEE_NAME".ToLower(), "GRADE_LEVEL".ToLower(), "STEP".ToLower(), "TAX_STATE".ToLower(), "CONTACT_ADDRESS".ToLower(), "EMAIL_ADDRESS".ToLower(), "MOBILE_PHONE".ToLower(), "TAX".ToLower() };

        private readonly Dictionary<string, int> months = new Dictionary<string, int> { { "1", 1 }, { "2", 2 }, { "3", 3 }, { "4", 4 }, { "5", 5 }, { "6", 6 }, { "7", 7 }, { "8", 8 }, { "9", 9 }, { "10", 10 }, { "11", 11 }, { "12", 12 } };


        /// <summary>
        /// Get the response to reading the file and processing the file from the file path
        /// </summary>
        /// <typeparam name="IR">the Payee model 
        /// <see cref="IPPISPayeeResponse"/>
        /// <see cref="PayeeResponseModel"/>
        /// </typeparam>
        /// <param name="filePath"></param>
        /// <param name="LGAFilePath"></param>
        /// <param name="stateName"></param>
        /// <returns>IR</returns>
        public IR GetPayeeResponseModels<IR>(string filePath, string LGAFilePath, string stateName, int month = 0, int year = 0) where IR : GetPayeResponse
        {
            return GetPayees(filePath, stateName, month, year) as IR;
        }


        /// <summary>
        /// Validate the headers and read the file.
        /// <para>
        /// this method validates the headers of the file, if headers are ok, 
        /// it proceeds to validate each row on the excel file
        /// </para>
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="stateName"></param>
        /// <returns></returns>
        protected IPPISPayeeResponse GetPayees(string filePath, string stateName, int month, int year)
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
                { templateHeaderNames[0], new HeaderValidationModel { } }, { templateHeaderNames[1], new HeaderValidationModel { } }, { templateHeaderNames[2], new HeaderValidationModel { } }, { templateHeaderNames[3], new HeaderValidationModel { } },
                 { templateHeaderNames[4], new HeaderValidationModel { } }, { templateHeaderNames[5], new HeaderValidationModel { } }, { templateHeaderNames[6], new HeaderValidationModel { } }, { templateHeaderNames[7], new HeaderValidationModel { } },
                 { templateHeaderNames[8], new HeaderValidationModel { } }, { templateHeaderNames[9], new HeaderValidationModel { } }, { templateHeaderNames[10], new HeaderValidationModel { } }, { templateHeaderNames[11], new HeaderValidationModel { } }
            };

            ValidateTemplateHeaders(rows[0], ref headers);

            var invalidsHeaders = headers.Where(k => k.Value.HeaderPresent == false);
            if (invalidsHeaders.Count() > 0)
            {
                var msg = invalidsHeaders.Select(x => x.Key + " header not found").ToArray();
                return new IPPISPayeeResponse { HeaderValidateObject = new HeaderValidateObject { Error = true, ErrorMessage = string.Join("\n", msg) } };
            }

            rows.RemoveAt(0);
            var col = sheet1.Columns;
            //List<IPPISAssessmentLineRecordModel> payeexs = new List<IPPISAssessmentLineRecordModel> { };
            ConcurrentStack<IPPISAssessmentLineRecordModel> payees = new ConcurrentStack<IPPISAssessmentLineRecordModel> { };
            IEnumerable<DataRow> irows = rows.OfType<DataRow>();
            Parallel.ForEach(irows, (item) =>
            {
                List<string> lineValues = new List<string>
                {
                    { item.ItemArray[headers[templateHeaderNames[0]].IndexOnFile].ToString()},
                    { item.ItemArray[headers[templateHeaderNames[1]].IndexOnFile].ToString() },
                    { item.ItemArray[headers[templateHeaderNames[2]].IndexOnFile].ToString() },
                    { item.ItemArray[headers[templateHeaderNames[3]].IndexOnFile].ToString() },
                    { item.ItemArray[headers[templateHeaderNames[4]].IndexOnFile].ToString()},
                    { item.ItemArray[headers[templateHeaderNames[5]].IndexOnFile].ToString()},
                    { item.ItemArray[headers[templateHeaderNames[6]].IndexOnFile].ToString()},
                    { item.ItemArray[headers[templateHeaderNames[7]].IndexOnFile].ToString()},
                    { item.ItemArray[headers[templateHeaderNames[8]].IndexOnFile].ToString() },
                    { item.ItemArray[headers[templateHeaderNames[9]].IndexOnFile].ToString()},
                    { item.ItemArray[headers[templateHeaderNames[10]].IndexOnFile].ToString()},
                    { item.ItemArray[headers[templateHeaderNames[11]].IndexOnFile].ToString()},
                };
                payees.Push(ValidatePayeModel(lineValues, month, year));
            });

            //foreach (DataRow item in rows)
            //{
            //    List<string> lineValues = new List<string>
            //    {
            //        { item.ItemArray[headers[templateHeaderNames[0]].IndexOnFile].ToString()},
            //        { item.ItemArray[headers[templateHeaderNames[1]].IndexOnFile].ToString() },
            //        { item.ItemArray[headers[templateHeaderNames[2]].IndexOnFile].ToString() },
            //        { item.ItemArray[headers[templateHeaderNames[3]].IndexOnFile].ToString() },
            //        { item.ItemArray[headers[templateHeaderNames[4]].IndexOnFile].ToString()},
            //        { item.ItemArray[headers[templateHeaderNames[5]].IndexOnFile].ToString()},
            //        { item.ItemArray[headers[templateHeaderNames[6]].IndexOnFile].ToString()},
            //        { item.ItemArray[headers[templateHeaderNames[7]].IndexOnFile].ToString()},
            //        { item.ItemArray[headers[templateHeaderNames[8]].IndexOnFile].ToString() },
            //        { item.ItemArray[headers[templateHeaderNames[9]].IndexOnFile].ToString()},
            //        { item.ItemArray[headers[templateHeaderNames[10]].IndexOnFile].ToString()},
            //        { item.ItemArray[headers[templateHeaderNames[11]].IndexOnFile].ToString()},
            //    };
            //    payeexs.Add(ValidatePayeModel(lineValues, month, year));
            //}
            return new IPPISPayeeResponse { Payees = payees, HeaderValidateObject = new HeaderValidateObject { } };
        }


        /// <summary>
        /// validate the file for the header information
        /// <para>we loop through the pre-defined headers given, then we set the index of the header to where on the file the header apppears</para>
        /// </summary>
        /// <param name="header">header object from file</param>
        /// <param name="headers">pre-defined headers that we are expecting</param>
        protected void ValidateTemplateHeaders(DataRow header, ref Dictionary<string, HeaderValidationModel> headers)
        {
            string errorMessage = string.Empty;
            int counter = -1;
            foreach (object item in header.ItemArray)
            {
                if (item is DBNull) { break; }
                counter++;
                string sItem = ((string)item).Trim().ToLower();

                if (templateHeaderNames[0].Equals(sItem)) { headers[templateHeaderNames[0]] = new HeaderValidationModel { HeaderPresent = true, IndexOnFile = counter }; continue; }
                if (templateHeaderNames[1].Equals(sItem)) { headers[templateHeaderNames[1]] = new HeaderValidationModel { HeaderPresent = true, IndexOnFile = counter }; continue; }
                if (templateHeaderNames[2].Equals(sItem)) { headers[templateHeaderNames[2]] = new HeaderValidationModel { HeaderPresent = true, IndexOnFile = counter }; continue; }
                if (templateHeaderNames[3].Equals(sItem)) { headers[templateHeaderNames[3]] = new HeaderValidationModel { HeaderPresent = true, IndexOnFile = counter }; continue; }
                if (templateHeaderNames[4].Equals(sItem)) { headers[templateHeaderNames[4]] = new HeaderValidationModel { HeaderPresent = true, IndexOnFile = counter }; continue; }
                if (templateHeaderNames[5].Equals(sItem)) { headers[templateHeaderNames[5]] = new HeaderValidationModel { HeaderPresent = true, IndexOnFile = counter }; continue; }
                if (templateHeaderNames[6].Equals(sItem)) { headers[templateHeaderNames[6]] = new HeaderValidationModel { HeaderPresent = true, IndexOnFile = counter }; continue; }
                if (templateHeaderNames[7].Equals(sItem)) { headers[templateHeaderNames[7]] = new HeaderValidationModel { HeaderPresent = true, IndexOnFile = counter }; continue; }
                if (templateHeaderNames[8].Equals(sItem)) { headers[templateHeaderNames[8]] = new HeaderValidationModel { HeaderPresent = true, IndexOnFile = counter }; continue; }
                if (templateHeaderNames[9].Equals(sItem)) { headers[templateHeaderNames[9]] = new HeaderValidationModel { HeaderPresent = true, IndexOnFile = counter }; continue; }
                if (templateHeaderNames[10].Equals(sItem)) { headers[templateHeaderNames[10]] = new HeaderValidationModel { HeaderPresent = true, IndexOnFile = counter }; continue; }
                if (templateHeaderNames[11].Equals(sItem)) { headers[templateHeaderNames[11]] = new HeaderValidationModel { HeaderPresent = true, IndexOnFile = counter }; }
            }
        }


        /// <summary>
        /// do validation of model
        /// </summary>
        /// <param name="lineValues"></param>
        /// <returns>IPPISAssessmentLineRecordModel</returns>
        protected IPPISAssessmentLineRecordModel ValidatePayeModel(List<string> lineValues, int month, int year)
        {
            var errorMessages = string.Empty;
            var hasError = false;

            var paye = new IPPISAssessmentLineRecordModel();

            //validate ministry name
            var ministryName = ConvertToString(lineValues.ElementAt(0), "MINISTRY_NAME", true, 0, 500, true);
            if (ministryName.HasError) { hasError = true; errorMessages = ministryName.ErrorMessage + "\n | "; }
            paye.Ministry_Name = ministryName;

            var orgCode = ConvertToString(lineValues.ElementAt(1), "ORG_CODE", false, 0, 250);
            if (orgCode.HasError) { hasError = true; errorMessages += orgCode.ErrorMessage + "\n | "; }
            paye.Org_Code = orgCode;

            //validate period name
            var periodName = ConvertToString(lineValues.ElementAt(2), "PERIOD_NAME", true, 0, 250, true);
            if (periodName.HasError) { hasError = true; errorMessages += periodName.ErrorMessage + "\n | "; }
            paye.Period_Name = periodName;

            //validate employee number
            var employeeNumber = ConvertToString(lineValues.ElementAt(3), "EMPLOYEE_NUMBER", false, 0, 100);
            if (employeeNumber.HasError) { hasError = true; errorMessages += employeeNumber.ErrorMessage + "\n | "; }
            paye.Employee_Number = employeeNumber;

            //validate employee name
            var employeeName = ConvertToString(lineValues.ElementAt(4), "EMPLOYEE_NAME", true, 0, 250, true);
            if (employeeName.HasError) { hasError = true; errorMessages += employeeName.ErrorMessage + "\n | "; }
            paye.Employee_Name = employeeName;

            var gradeLevel = ConvertToString(lineValues.ElementAt(5), "GRADE_LEVEL", true, 0, 250, true);
            if (gradeLevel.HasError) { hasError = true; errorMessages += gradeLevel.ErrorMessage + "\n | "; }
            paye.Grade_Level = gradeLevel;

            var step = ConvertToString(lineValues.ElementAt(6), "STEP", true, 0, 250, true);
            if (step.HasError) { hasError = true; errorMessages += step.ErrorMessage + "\n | "; }
            paye.Step = step;

            var taxState = ConvertToString(lineValues.ElementAt(7), "TAX_STATE", true, 0, 250, true);
            if (taxState.HasError) { hasError = true; errorMessages += taxState.ErrorMessage + "\n | "; }
            paye.Tax_State = taxState;

            var contactAddress = ConvertToString(lineValues.ElementAt(8), "CONTACT_ADDRESS", true, 0, 1000, true);
            if (contactAddress.HasError) { hasError = true; errorMessages += contactAddress.ErrorMessage + "\n | "; }
            paye.Contact_Address = contactAddress;

            var emailAddress = ConvertToString(lineValues.ElementAt(9), "EMAIL_ADDRESS", true, 1, 250, true);
            if (emailAddress.HasError) { hasError = true; errorMessages += emailAddress.ErrorMessage + "\n | "; }
            paye.Email_Address = emailAddress;

            var mobilePhone = ValidatePhoneNumber(lineValues.ElementAt(10), true);
            if (mobilePhone.HasError) { hasError = true; errorMessages += mobilePhone.ErrorMessage + "\n | "; }
            paye.Mobile_Phone = mobilePhone;

            var tax = ConvertToDecimal(lineValues.ElementAt(11), "TAX", false, 1, 29);
            if (tax.HasError) { hasError = true; errorMessages += tax.ErrorMessage + "\n | "; }
            paye.Tax = tax;

            paye.Month = new PayeeIntValue { Value = month };
            paye.Year = new PayeeIntValue { Value = year, StringValue = year.ToString() };
            try
            {
                paye.AssessmentDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));
            }
            catch (Exception)
            {
                paye.Month.HasError = true; paye.Month.ErrorMessage = "Bad date format";
                paye.Year.HasError = true; paye.Year.ErrorMessage = "Bad date format";
            }

            paye.HasError = hasError;
            errorMessages = errorMessages.Trim().TrimEnd('|');
            paye.ErrorMessages = errorMessages;
            return paye;
        }


        /// <summary>
        /// Validate phonenumber
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns>PayeeStringValue</returns>
        protected override PayeeStringValue ValidatePhoneNumber(string phoneNumber, bool ignoreError = false)
        {
            if (string.IsNullOrEmpty(phoneNumber)) { return new PayeeStringValue { HasError = ignoreError ? false : true, ErrorMessage = "Add a valid phone number." }; }
            phoneNumber = phoneNumber.Trim().Replace(" ", string.Empty);
            phoneNumber = phoneNumber.Replace("-", string.Empty).Trim();
            if (phoneNumber.Length > 20 || phoneNumber.Length < 6)
            { return new PayeeStringValue { HasError = ignoreError ? false : true, ErrorMessage = "Enter a valid phone number." }; }
            long lphoneNumber = 0;
            bool isANumber = long.TryParse(phoneNumber, out lphoneNumber);
            if (!isANumber) { return new PayeeStringValue { ErrorMessage = "Paye phone number is not valid. Add a valid phone number.", HasError = ignoreError ? false : true, Value = phoneNumber }; }
            return new PayeeStringValue { Value = phoneNumber };
        }


        public List<IPPISPayeeAmountMinistrySummary> GetIPPISPayeeMinistrySummary(List<IPPISAssessmentLineRecordModel> payees)
        {
            var ministryGrouping = from payeeList in payees
                                   group payeeList by payeeList.Org_Code.Value into summaryByOrgCode
                                   select new IPPISPayeeAmountMinistrySummary
                                   {
                                       OrgCode = summaryByOrgCode.Key,
                                       TotalAmount = summaryByOrgCode.Sum(x => x.Tax.Value),
                                   };

            return ministryGrouping.ToList();
        }


        /// <summary>
        /// Convert excel file to CSV
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>HeaderValidateObject</returns>
        public HeaderValidateObject ConvertExcelToCSV(string excelFilePath, string destinationPath, SettlementDetails settlementDetails)
        {
            using (var stream = new FileStream(excelFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream);

                if (reader == null)
                    return new HeaderValidateObject { Error = true, ErrorMessage = string.Format("File 404 {0}", excelFilePath) };

                DataSet result = reader.AsDataSet();

                DataTable sheet1 = result.Tables[0];
                DataRowCollection rows = sheet1.Rows;
                StringBuilder csvString = new StringBuilder();
                int counter = 0;
                var regex = new Regex(@"\r\n?|\n|\t|,", RegexOptions.Compiled);
                //
                decimal totalAmount = 0.00M;

                foreach (DataRow item in rows)
                {
                    string agencyCode = string.Empty;

                    List<string> rowConcat = new List<string>();
                    for (int i = 0; i < item.ItemArray.Length; i++)
                    {
                        string rowString = regex.Replace(item.ItemArray[i].ToString().Trim(), String.Empty);
                        string rowValue = rowString.Replace(Environment.NewLine, "");
                        if (i == 0)
                        { agencyCode = rowValue; }
                        else if(i == 6)
                        {
                            var samount = rowValue.Replace(",", string.Empty).Replace(" ", string.Empty);
                            if(decimal.TryParse(samount, out decimal amount))
                            {
                                totalAmount += amount;
                            }
                        }
                        else if(i == 9)
                        { rowValue += string.Format("{0}:::AGENCYCODE:{1}", rowValue, agencyCode); }

                        rowConcat.Add(rowValue);
                    }
                    csvString.Append(string.Join(",", rowConcat) + "\n");
                    counter++;
                }

                if(settlementDetails != null)
                {
                    //do spacing
                    //for (int i = 0; i < settlementDetails.Spacing; i++)
                    //{
                    //    csvString.AppendLine();
                    //}

                    //do account appends
                    foreach (var item in settlementDetails.Parties)
                    {
                        var settlementLine = string.Join(",", item.DetailRows.Values.Select(r => r).ToArray());
                        settlementLine = settlementLine.Replace("::AMOUNT::", ((totalAmount*item.Percentage)/100).ToString());
                        settlementLine = settlementLine.Replace("::NARRATION::", string.Format("{0} {1} SETTLEMENT", settlementDetails.Month, settlementDetails.Year.ToString()));
                        csvString.AppendLine(settlementLine);
                    }
                }
                StreamWriter csv = new StreamWriter(destinationPath, false);
                csv.Write(csvString.ToString());
                csv.Close();
                return new HeaderValidateObject { };
            }
        }
    }
}
