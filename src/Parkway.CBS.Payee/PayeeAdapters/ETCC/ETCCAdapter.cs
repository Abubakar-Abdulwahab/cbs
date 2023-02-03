using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Parkway.CBS.Payee.PayeeAdapters.Contracts;
using System.Data;
using System.IO;
using ExcelDataReader;

namespace Parkway.CBS.Payee.PayeeAdapters.ETCC
{
    class ETCCAdapter : BasePayeeAdapter, IPayeeAdapter
    {

        /// <summary>
        ///  Tax Payer Id - 0, 
        ///  Gross Annual Earning - 1,
        ///  Exemptions (Annual) - 2, 
        ///  Paye Month - 3, 
        ///  Paye Year - 4,
        /// </summary>
        protected readonly new string[] headerNames = { "Tax Payer Id".ToLower(), "Gross Annual Earning".ToLower(),
            "Exemptions (Annual)".ToLower(), "Paye Month (e.g Jan)".ToLower(), "Paye Year (e.g 2018)".ToLower() };

        internal List<TaxRuleSet> Rules = new List<TaxRuleSet>
        { { new TaxRuleSet { Amount = 300000, Percentage = 7 } }, { new TaxRuleSet { Amount = 300000, Percentage = 11 } }, { new TaxRuleSet { Amount = 500000, Percentage = 15 } }, { new TaxRuleSet { Amount = 500000, Percentage = 19 } }, { new TaxRuleSet{ Amount = 1600000, Percentage = 21} }, { new TaxRuleSet { Amount = 3200000, Percentage = 24 } }  };


        public GetPayeResponse GetPayeeModels(string filePath, string LGAFilePath, string stateName)
        { return GetPAYEResponse(filePath); }



        /// <summary>
        /// Header check and items transformation into PayeeAssessmentLineRecordModel for the file upload
        /// </summary>
        /// <param name="filePath"></param>
        private GetPayeResponse GetPAYEResponse(string filePath)
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
                { headerNames[0], new HeaderValidationModel { } }, { headerNames[1], new HeaderValidationModel { } }, { headerNames[2], new HeaderValidationModel { } }, { headerNames[3], new HeaderValidationModel { } }, { headerNames[4], new HeaderValidationModel { } }
            };


            ValidateHeaders(rows[0], ref headers);
            var invalidsHeaders = headers.Where(k => k.Value.HeaderPresent == false);
            if (invalidsHeaders.Count() > 0)
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
                    { item.ItemArray[headers[headerNames[0]].IndexOnFile].ToString()},//Tax Payer Id
                    { item.ItemArray[headers[headerNames[1]].IndexOnFile].ToString() }, //Gross Annual Earning
                    { item.ItemArray[headers[headerNames[2]].IndexOnFile].ToString() }, //Exemptions (Annual)
                    { item.ItemArray[headers[headerNames[3]].IndexOnFile].ToString() }, //Paye Month (e.g Jan)
                    { item.ItemArray[headers[headerNames[4]].IndexOnFile].ToString()}, //Paye Year (e.g 2018)
                };

                payees.Add(ValidatePayeModel(lineValues, string.Empty, string.Empty));
            }
            return new GetPayeResponse { Payes = payees, HeaderValidateObject = new HeaderValidateObject { } };
        }


        /// <summary>
        /// Validate excel headers
        /// </summary>
        /// <param name="header"></param>
        /// <returns>HeaderValidateObject</returns>
        protected override void ValidateHeaders(DataRow header, ref Dictionary<string, HeaderValidationModel> headers)
        {
            string errorMessage = string.Empty;
            int counter = -1;
            foreach (object item in header.ItemArray)
            {
                if (item is DBNull) { break; }
                counter++;
                string sItem = ((string)item).Trim().ToLower();

                if (headerNames[0].Contains(sItem)) { headers[headerNames[0]] = new HeaderValidationModel { HeaderPresent = true, IndexOnFile = counter }; continue; }
                if (headerNames[1].Contains(sItem)) { headers[headerNames[1]] = new HeaderValidationModel { HeaderPresent = true, IndexOnFile = counter }; continue; }
                if (headerNames[2].Contains(sItem)) { headers[headerNames[2]] = new HeaderValidationModel { HeaderPresent = true, IndexOnFile = counter }; continue; }
                if (headerNames[3].Contains(sItem)) { headers[headerNames[3]] = new HeaderValidationModel { HeaderPresent = true, IndexOnFile = counter }; continue; }
                if (headerNames[4].Contains(sItem)) { headers[headerNames[4]] = new HeaderValidationModel { HeaderPresent = true, IndexOnFile = counter }; continue; }
            }
        }



        /// <summary>
        /// Get the break down of the payee tax amount
        /// <para>this value is let empty if the payee model prop HasError is true</para>
        /// </summary>
        /// <param name="payees"></param>
        /// <returns>PayeeAmountAndBreakDown</returns>
        public PayeeAmountAndBreakDown GetRequestBreakDown(List<PayeeAssessmentLineRecordModel> payees)
        {
            Parallel.ForEach(payees, (payee) =>
            {
                if (payee.TaxPayerId.HasError) { payee.PayeeBreakDown = new PayeeBreakDown { }; return; }
                if (payee.GrossAnnualEarnings.HasError) { payee.PayeeBreakDown = new PayeeBreakDown { }; return; }
                if (payee.Exemptions.HasError) { payee.PayeeBreakDown = new PayeeBreakDown { }; return; }
                if (payee.Month.HasError) { payee.PayeeBreakDown = new PayeeBreakDown { }; return; }
                if (payee.Year.HasError) { payee.PayeeBreakDown = new PayeeBreakDown { }; return; }
                if (payee.AssessmentDate == null) { payee.PayeeBreakDown = new PayeeBreakDown { }; return; }

                var onePercent = Math.Round((payee.GrossAnnualEarnings.Value / 100), 2);

                var consolidatedRelief = Math.Round(((payee.GrossAnnualEarnings.Value * 20) / 100) + 200000, 2);
                var totalDeductions = Math.Round(consolidatedRelief + payee.Exemptions.Value, 2);
                var annualTaxableIncome = payee.GrossAnnualEarnings.Value - totalDeductions;

                payee.PayeeBreakDown = GetBreakDown(annualTaxableIncome, onePercent, Rules);
            });

            return new PayeeAmountAndBreakDown { TotalAmount = payees.Sum(x => x.PayeeBreakDown.Tax), Payees = payees };
        }


        public List<PayeeAssessmentLineRecordModel> GetPAYEModels<T>(ICollection<T> lines, string LGAFilePath, string stateName)
        {
            return GetPayees(lines as ICollection<PAYEAssessmentLine>, LGAFilePath, stateName);
        }



        private List<PayeeAssessmentLineRecordModel> GetPayees(ICollection<PAYEAssessmentLine> directAssessmentPayeeLines, string LGAFilePath, string stateName)
        {
            List<PayeeAssessmentLineRecordModel> payes = new List<PayeeAssessmentLineRecordModel> { };
            foreach (var item in directAssessmentPayeeLines)
            {
                List<string> lineValues = new List<string>
                {
                    { item.PayerId },//payer Id
                    { item.GrossAnnualEarning }, //gross annual
                    { item.Exemptions }, //exemptions
                    { item.Month }, //month
                    { item.Year }, //year
                };
                payes.Add(ValidatePayeModel(lineValues, LGAFilePath, stateName));
            }
            return payes;
        }


        protected override PayeeAssessmentLineRecordModel ValidatePayeModel(List<string> lineValues, string LGAFilePath, string stateName)
        {
            var errorMessages = string.Empty;
            var hasError = false;

            var paye = new PayeeAssessmentLineRecordModel();
            //            `assessment date` (dd / MM / YYYY)
            //            `tax amount` (edited)

            //validate tax payer name
            var payerId = ConvertToString(lineValues.ElementAt(0), "Payer Id", false, 0, 200);
            if (payerId.HasError) { hasError = true; errorMessages = payerId.ErrorMessage + "\n | "; }
            paye.TaxPayerId = payerId;

            var grossEarning = ConvertToDecimal(lineValues.ElementAt(1), "Gross annual earnings", false, 1, 29);
            if (grossEarning.HasError) { hasError = true; errorMessages += grossEarning.ErrorMessage + "\n | "; }
            paye.GrossAnnualEarnings = grossEarning;

            var exemptions = ConvertToDecimal(lineValues.ElementAt(2), "Exemptions", true, 0, 29);
            if (exemptions.HasError) { hasError = true; errorMessages += exemptions.ErrorMessage + "\n | "; };
            paye.Exemptions = exemptions;

            var month = ValidateMonth(lineValues.ElementAt(3));
            if (month.HasError) { hasError = true; errorMessages += month.ErrorMessage + "\n | "; }
            paye.Month = month;

            var year = ValidateYear(lineValues.ElementAt(4), "Paye year", 4, 11);
            if (year.HasError) { hasError = true; errorMessages += year.ErrorMessage + "\n | "; }
            paye.Year = year;

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
        /// Get the response to reading the file and processing the file from the file path
        /// </summary>
        /// <typeparam name="PR">the Payee model 
        /// <see cref="IPPISPayeeResponse"/>
        /// <see cref="PayeeResponseModel"/>
        /// </typeparam>
        /// <param name="filePath"></param>
        /// <param name="LGAFilePath"></param>
        /// <param name="stateName"></param>
        /// <returns>IR</returns>
        public IR GetPayeeResponseModels<IR>(string filePath, string LGAFilePath, string stateName, int month = 0, int year = 0) where IR : GetPayeResponse
        {
            return GetPayees(filePath, LGAFilePath, stateName) as IR;
        }

    }
}