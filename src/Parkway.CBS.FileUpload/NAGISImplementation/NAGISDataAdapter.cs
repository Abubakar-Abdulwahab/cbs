using ExcelDataReader;
using Parkway.CBS.FileUpload.NAGISImplementation.Contracts;
using Parkway.CBS.FileUpload.NAGISImplementation.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Parkway.CBS.FileUpload.NAGISImplementation
{
    public class NAGISDataAdapter : INAGISDataAdapter
    {
        private const int templateHeaderCount = 15;
        protected readonly string[] templateHeaderNames =
                {
            "CustomerName".ToLower() , "PhoneNumber".ToLower(), "Address".ToLower(), "CustomerId".ToLower(),"Amount".ToLower(), "Tin".ToLower(), "InvoiceNumber".ToLower(), "CreationDate".ToLower(), "RevenueHeadId".ToLower(),
            "ExternalRefId".ToLower(), "InvoiceDescription".ToLower(), "AmountDue".ToLower(), "Quantity".ToLower(), "Status".ToLower(),"GroupID".ToLower()
        };


        /// <summary>
        /// Get the response to reading the file and processing the file from the file path
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public NAGISDataResponse GetNAGISDataResponseModels(string filePath)
        {
            return GetNAGISDataRecords(filePath);
        }

        /// <summary>
        /// Validate the headers and read the file.
        /// <para>
        /// this method validates the headers of the file, if headers are ok, 
        /// it proceeds to read each row on the excel file
        /// </para>
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        protected NAGISDataResponse GetNAGISDataRecords(string filePath)
        {
            DataSet result = new DataSet();
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                { result = reader.AsDataSet(); }
            }

            var sheet1 = result.Tables[0];
            var rows = sheet1.Rows;

            Dictionary<string, NAGISTemplateHeaderValidation> headers = new Dictionary<string, NAGISTemplateHeaderValidation>();
            for (int i = 0; i < templateHeaderCount; i++)
            {
                headers.Add(templateHeaderNames[i], new NAGISTemplateHeaderValidation { });
            }


            ValidateTemplateHeaders(rows[0], ref headers);

            var invalidsHeaders = headers.Where(k => k.Value.HeaderPresent == false);
            if (invalidsHeaders.Count() > 0)
            {
                var msg = invalidsHeaders.Select(x => x.Key + " header not found").ToArray();
                return new NAGISDataResponse { HeaderValidateObject = new HeaderValidateObject { Error = true, ErrorMessage = string.Join("\n", msg) } };
            }

            rows.RemoveAt(0);
            var col = sheet1.Columns;
            ConcurrentStack<NAGISDataLineRecordModel> nagisDataRecords = new ConcurrentStack<NAGISDataLineRecordModel> { };
            IEnumerable<DataRow> irows = rows.OfType<DataRow>();
            Parallel.ForEach(irows, (item) =>
            {
                List<string> lineValues = new List<string>();
                for (int i = 0; i < templateHeaderCount; i++)
                {
                    lineValues.Add(item.ItemArray[headers[templateHeaderNames[i]].IndexOnFile].ToString());
                }

                var referenceDataLineRecord = GetNAGISDataRecords(lineValues);
                nagisDataRecords.Push(referenceDataLineRecord);
            });

            return new NAGISDataResponse { NAGISDataLineRecords = nagisDataRecords, HeaderValidateObject = new HeaderValidateObject { } };
        }


        /// <summary>
        /// validate the file for the header information
        /// <para>we loop through the pre-defined headers given, then we set the index of the header to where on the file the header apppears</para>
        /// </summary>
        /// <param name="header">header object from file</param>
        /// <param name="headers">pre-defined headers that we are expecting</param>
        protected void ValidateTemplateHeaders(DataRow header, ref Dictionary<string, NAGISTemplateHeaderValidation> headers)
        {
            string errorMessage = string.Empty;
            int counter = -1;
            foreach (object item in header.ItemArray)
            {
                if (item is DBNull) { break; }
                counter++;
                string sItem = ((string)item).Trim().ToLower();

                for (int i = 0; i < templateHeaderCount; i++)
                {
                    if (templateHeaderNames[i].Equals(sItem)) { headers[templateHeaderNames[i]] = new NAGISTemplateHeaderValidation { HeaderPresent = true, IndexOnFile = counter }; continue; }
                }
            }
        }

        /// <summary>
        /// Read the value on each row of the excel
        /// </summary>
        /// <param name="lineValues"></param>
        /// <returns>ReferenceDataLineRecordModel</returns>
        protected NAGISDataLineRecordModel GetNAGISDataRecords(List<string> lineValues)
        {
            var nagisData = new NAGISDataLineRecordModel();

            var customerName = GetStringValue(lineValues.ElementAt(0), "CustomerName");
            nagisData.CustomerName = customerName;

            var phoneNumber = GetStringValue(lineValues.ElementAt(1), "PhoneNumber");
            nagisData.PhoneNumber = phoneNumber;

            var address = GetStringValue(lineValues.ElementAt(2), "Address");
            nagisData.Address = address;

            var customerId = GetStringValue(lineValues.ElementAt(3), "CustomerId");
            nagisData.CustomerId = customerId;

            var amount = GetDecimalValue(lineValues.ElementAt(4), "Amount", false);
            nagisData.Amount = amount;

            var tin = GetStringValue(lineValues.ElementAt(5), "Tin");
            nagisData.Tin = tin;

            var invoiceNumber = GetStringValue(lineValues.ElementAt(6), "InvoiceNumber");
            nagisData.InvoiceNumber = invoiceNumber;

            var creationDate = GetStringValue(lineValues.ElementAt(7), "CreationDate");
            nagisData.CreationDate = creationDate;

            var revenueHeadId = GetIntValue(lineValues.ElementAt(8), "RevenueHeadId");
            nagisData.RevenueHeadId = revenueHeadId;

            var externalRefId = GetStringValue(lineValues.ElementAt(9), "ExternalRefId");
            nagisData.ExternalRefId = externalRefId;

            var invoiceDescription = GetStringValue(lineValues.ElementAt(10), "InvoiceDescription");
            nagisData.InvoiceDescription = invoiceDescription;

            var amountDue = GetDecimalValue(lineValues.ElementAt(11), "AmountDue", true);
            nagisData.AmountDue = amountDue;

            var quantity = GetIntValue(lineValues.ElementAt(12), "Quantity");
            nagisData.Quantity = quantity;

            var status = GetIntValue(lineValues.ElementAt(13), "Status");
            nagisData.Status = status;

            var groupID = GetIntValue(lineValues.ElementAt(14), "GroupID");
            nagisData.GroupID = groupID;

            string categoryId = nagisData.CustomerId.ToString()[0].ToString();
            nagisData.TaxEntityCategoryID = categoryId.ToUpper().Equals("P") ? GetIntValue(ConfigurationManager.AppSettings["IndividualTaxCategoryId"], "TaxEntityCategoryID") : GetIntValue(ConfigurationManager.AppSettings["CorporateTaxCategoryId"], "TaxEntityCategoryID");

            return nagisData;
        }

        /// <summary>
        /// Get the trim string value
        /// </summary>
        /// <param name="stringValue"></param>
        /// <param name="headerValue"></param>
        /// <returns></returns>
        protected virtual NAGISDataStringValue GetStringValue(string stringValue, string headerValue)
        {
            return new NAGISDataStringValue { Value = string.IsNullOrEmpty(stringValue) ? stringValue : stringValue.Trim() };
        }


        /// <summary>
        /// Get the integer value
        /// </summary>
        /// <param name="stringValue"></param>
        /// <param name="headerValue"></param>
        /// <returns></returns>
        protected virtual NAGISDataIntValue GetIntValue(string stringValue, string headerValue, bool allowZero = false)
        {
            if (string.IsNullOrEmpty(stringValue))
            {
                return new NAGISDataIntValue
                { StringValue = String.Format("{0:n0}", 0), ErrorMessage = string.Format("{0} is empty.", headerValue), HasError = true, Value = 0 };
            }


            int intValue = 0;
            //try converting
            if (!Int32.TryParse(stringValue.Trim(), out intValue))
            {
                return new NAGISDataIntValue { ErrorMessage = string.Format("{0} is not a valid number.", headerValue), HasError = true, StringValue = stringValue };
            }

            if (intValue <= 0)
            {
                return new NAGISDataIntValue { StringValue = intValue.ToString(), HasError = true, ErrorMessage = string.Format("{0} is not valid.", headerValue) };
            }

            return new NAGISDataIntValue { Value = intValue, StringValue = intValue.ToString() };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringValue"></param>
        /// <param name="headerValue"></param>
        /// <param name="allowZero"></param>
        /// <param name="minValidLength"></param>
        /// <param name="maxValidLength"></param>
        /// <returns></returns>
        protected NAGISDecimalValue GetDecimalValue(string stringValue, string headerValue, bool allowZero = false)
        {
            if (string.IsNullOrEmpty(stringValue))
            {
                return new NAGISDecimalValue
                { StringValue = String.Format("{0:n0}", 0.00m), ErrorMessage = string.Format("{0} is empty.", headerValue), HasError = true, Value = 0.00m };
            }

            //trim and remove comma
            stringValue = stringValue.Trim().Replace(",", "");
            //remove white space between digits
            decimal decimalValue = 0.00m;
            stringValue = Regex.Replace(stringValue, @"\s+", "");
            //try converting
            if (!decimal.TryParse(stringValue, out decimalValue))
            {
                return new NAGISDecimalValue { StringValue = stringValue, ErrorMessage = string.Format("{0} is not a valid amount.", headerValue), HasError = true, };
            }

            if (decimalValue <= 0 && !allowZero)
            {
                return new NAGISDecimalValue { StringValue = stringValue, HasError = true, ErrorMessage = string.Format("{0} is not a valid amount.", headerValue) };
            }
            decimalValue = Math.Round(decimalValue, 2);
            return new NAGISDecimalValue { Value = decimalValue, StringValue = string.Format("{0:n}", decimalValue) };
        }
    }

    public class NAGISDataStringValue
    {
        public string Value { get; internal set; }
    }

    public class NAGISDataIntValue : BaseValue
    {
        public int Value { get; internal set; }

        public string StringValue { get; internal set; }

    }

    public abstract class BaseValue
    {
        public bool HasError { get; internal set; }

        public string ErrorMessage { get; internal set; }
    }

    public class NAGISDecimalValue : BaseValue
    {
        public decimal Value { get; internal set; }

        public string StringValue { get; internal set; }
    }


}
