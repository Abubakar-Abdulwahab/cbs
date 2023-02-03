using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Parkway.CBS.Payee.PayeeAdapters
{
    public class FileReader : BasePayeeAdapter
    {

        public GetPayeResponse ReadFile(string filePath, string LGAFilePath, string stateName)
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
                    { item.ItemArray[0].ToString()},//tax payer name
                    { item.ItemArray[1].ToString() }, //tax payer tin
                    { item.ItemArray[2].ToString() }, //gross annual
                    { item.ItemArray[3].ToString() }, //exemptions
                    { item.ItemArray[4].ToString()}, //month
                    { item.ItemArray[5].ToString()}, //year
                    { item.ItemArray[6].ToString()}, // email
                    { item.ItemArray[7].ToString()}, //phone number
                    { item.ItemArray[8].ToString() }, //Paye address
                    { item.ItemArray[9].ToString()}, //LGA
                };

                payees.Add(ValidatePayeModel(lineValues, LGAFilePath, stateName));
            }
            return new GetPayeResponse { Payes = payees, HeaderValidateObject = new HeaderValidateObject { } };
        }
    }
}
