using ExcelDataReader;
using Parkway.CBS.Entities.DTO;
using Parkway.CBS.FileUpload.OSGOFImplementation.Contracts;
using Parkway.CBS.FileUpload.OSGOFImplementation.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Parkway.CBS.FileUpload.OSGOFImplementation
{
    public class OSGOFCellSitesAdapter : IOSGOFCellSitesAdapter
    {
        protected readonly string[] templateHeaderNames = {"S/N".ToLower(), "OPERATOR SITE ID".ToLower() , "YEAR OF DEPLOYMENT".ToLower(), "HEIGHT OF TOWER/MAST".ToLower(), "LONGITUDE".ToLower(),
            "LATITUDE".ToLower(), "SITE ADDRESS".ToLower(), "REGION".ToLower(), "STATE".ToLower(), "LGA".ToLower(), "TOWER/MAST TYPE".ToLower() };

        public OSGOFCellSitesResponse GetCellSitesResponseModels(List<StatesAndLGAs> statesAndLGAs, string filePath)
        {
            return GetCellSites(statesAndLGAs, filePath);
        }


        /// <summary>
        /// This takes the Excel file path and perform the header validation
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        protected OSGOFCellSitesResponse GetCellSites(List<StatesAndLGAs> statesAndLGAs, string filePath)
        {
            DataSet result = new DataSet();
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                { result = reader.AsDataSet(); }
            }

            var sheet1 = result.Tables[0];
            var rows = sheet1.Rows;

            Dictionary<string, OSGOFTemplateHeaderValidation> headers = new Dictionary<string, OSGOFTemplateHeaderValidation>
            {
                { templateHeaderNames[0], new OSGOFTemplateHeaderValidation { } }, { templateHeaderNames[1], new OSGOFTemplateHeaderValidation { } }, { templateHeaderNames[2], new OSGOFTemplateHeaderValidation { } }, { templateHeaderNames[3], new OSGOFTemplateHeaderValidation { } },
                 { templateHeaderNames[4], new OSGOFTemplateHeaderValidation { } }, { templateHeaderNames[5], new OSGOFTemplateHeaderValidation { } }, { templateHeaderNames[6], new OSGOFTemplateHeaderValidation { } }, { templateHeaderNames[7], new OSGOFTemplateHeaderValidation { } },
                 { templateHeaderNames[8], new OSGOFTemplateHeaderValidation { } }, { templateHeaderNames[9], new OSGOFTemplateHeaderValidation { } }, { templateHeaderNames[10], new OSGOFTemplateHeaderValidation { } }
            };

            ValidateTemplateHeaders(rows[0], ref headers);

            var invalidsHeaders = headers.Where(k => k.Value.HeaderPresent == false);
            if (invalidsHeaders.Count() > 0)
            {
                var msg = invalidsHeaders.Select(x => x.Key + " header not found").ToArray();
                return new OSGOFCellSitesResponse { HeaderValidateObject = new OSGOFValidationObject { Error = true, ErrorMessage = string.Join("\n", msg) } };
            }

            rows.RemoveAt(0);
            var col = sheet1.Columns;
            ConcurrentStack<OSGOFCellSitesExcelModel> cellSite = new ConcurrentStack<OSGOFCellSitesExcelModel> { };
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
                };
                cellSite.Push(ValidatePayeModel(lineValues, statesAndLGAs));
            });

            return new OSGOFCellSitesResponse { CellSites = cellSite, HeaderValidateObject = new OSGOFValidationObject { } };
        }

        /// <summary>
        /// validate the file for the header information
        /// <para>we loop through the pre-defined headers given, then we set the index of the header to where on the file the header apppears</para>
        /// </summary>
        /// <param name="header">header object from file</param>
        /// <param name="headers">pre-defined headers that we are expecting</param>
        protected void ValidateTemplateHeaders(DataRow header, ref Dictionary<string, OSGOFTemplateHeaderValidation> headers)
        {
            string errorMessage = string.Empty;
            int counter = -1;
            foreach (object item in header.ItemArray)
            {
                if (item is DBNull) { break; }
                counter++;
                string sItem = ((string)item).Trim().ToLower();

                if (templateHeaderNames[0].Equals(sItem)) { headers[templateHeaderNames[0]] = new OSGOFTemplateHeaderValidation { HeaderPresent = true, IndexOnFile = counter }; continue; }
                if (templateHeaderNames[1].Equals(sItem)) { headers[templateHeaderNames[1]] = new OSGOFTemplateHeaderValidation { HeaderPresent = true, IndexOnFile = counter }; continue; }
                if (templateHeaderNames[2].Equals(sItem)) { headers[templateHeaderNames[2]] = new OSGOFTemplateHeaderValidation { HeaderPresent = true, IndexOnFile = counter }; continue; }
                if (templateHeaderNames[3].Equals(sItem)) { headers[templateHeaderNames[3]] = new OSGOFTemplateHeaderValidation { HeaderPresent = true, IndexOnFile = counter }; continue; }
                if (templateHeaderNames[4].Equals(sItem)) { headers[templateHeaderNames[4]] = new OSGOFTemplateHeaderValidation { HeaderPresent = true, IndexOnFile = counter }; continue; }
                if (templateHeaderNames[5].Equals(sItem)) { headers[templateHeaderNames[5]] = new OSGOFTemplateHeaderValidation { HeaderPresent = true, IndexOnFile = counter }; continue; }
                if (templateHeaderNames[6].Equals(sItem)) { headers[templateHeaderNames[6]] = new OSGOFTemplateHeaderValidation { HeaderPresent = true, IndexOnFile = counter }; continue; }
                if (templateHeaderNames[7].Equals(sItem)) { headers[templateHeaderNames[7]] = new OSGOFTemplateHeaderValidation { HeaderPresent = true, IndexOnFile = counter }; continue; }
                if (templateHeaderNames[8].Equals(sItem)) { headers[templateHeaderNames[8]] = new OSGOFTemplateHeaderValidation { HeaderPresent = true, IndexOnFile = counter }; continue; }
                if (templateHeaderNames[9].Equals(sItem)) { headers[templateHeaderNames[9]] = new OSGOFTemplateHeaderValidation { HeaderPresent = true, IndexOnFile = counter }; continue; }
                if (templateHeaderNames[10].Equals(sItem)) { headers[templateHeaderNames[10]] = new OSGOFTemplateHeaderValidation { HeaderPresent = true, IndexOnFile = counter }; }
            }
        }

        /// <summary>
        /// do validation of model
        /// </summary>
        /// <param name="lineValues"></param>
        /// <returns>IPPISAssessmentLineRecordModel</returns>
        protected OSGOFCellSitesExcelModel ValidatePayeModel(List<string> lineValues, List<StatesAndLGAs> statesAndLGAs)
        {
            var errorMessages = string.Empty;
            var hasError = false;

            var cellSite = new OSGOFCellSitesExcelModel();

            var SN = ConvertToInteger(lineValues.ElementAt(0), "S/N", false, 0, Int32.MaxValue, true);
            if (SN.HasError) { hasError = true; errorMessages = SN.ErrorMessage + "\n | "; }
            cellSite.SN = SN;

            var operatorSiteId = ConvertToString(lineValues.ElementAt(1), "OPERATOR SITE ID", false, 0, 250);
            if (operatorSiteId.HasError) { hasError = true; errorMessages += operatorSiteId.ErrorMessage + "\n | "; }
            else { operatorSiteId.Value = operatorSiteId.Value.ToUpper(); }
            cellSite.OperatorSiteId = operatorSiteId;

            var yearofDeployment = ConvertToInteger(lineValues.ElementAt(2), "YEAR OF DEPLOYMENT", false, 4, 4);
            if (yearofDeployment.HasError) { hasError = true; errorMessages += yearofDeployment.ErrorMessage + "\n | "; }
            cellSite.YearofDeployment = yearofDeployment;

            //if (!string.IsNullOrEmpty(lineValues.ElementAt(3)))
            {
                var heighofTower = ConvertToDecimal(lineValues.ElementAt(3), "HEIGHT OF TOWER/MAST", false, 0, 100, true);
                if (heighofTower.HasError) { hasError = true; errorMessages += heighofTower.ErrorMessage + "\n | "; }
                cellSite.HeightofTower = heighofTower;
            }

            var longitude = ConvertToString(lineValues.ElementAt(4), "LONGITUDE", false, 0, 250);
            if (longitude.HasError) { hasError = true; errorMessages += longitude.ErrorMessage + "\n | "; }
            cellSite.Longitude = longitude;

            var latitude = ConvertToString(lineValues.ElementAt(5), "Latitude", false, 0, 250);
            if (latitude.HasError) { hasError = true; errorMessages += latitude.ErrorMessage + "\n | "; }
            cellSite.Latitude = latitude;

            var siteAddress = ConvertToString(lineValues.ElementAt(6), "SITE ADDRESS", false, 3, 1000);
            if (siteAddress.HasError) { hasError = true; errorMessages += siteAddress.ErrorMessage + "\n | "; }
            else { siteAddress.Value = siteAddress.Value.ToUpper(); }
            cellSite.SiteAddress = siteAddress;

            var region = ConvertToString(lineValues.ElementAt(7), "REGION", true, 0, 250, true);
            if (region.HasError) { hasError = true; errorMessages += region.ErrorMessage + "\n | "; }
            else { region.Value = region.Value.ToUpper(); }
            cellSite.Region = region;


            //validate the state
            //find state
            StatesAndLGAs stateAndLGAs = null;
            var state = ConvertToString(lineValues.ElementAt(8), "STATE", false, 3, 100);
            if (state.HasError) { hasError = true; errorMessages += state.ErrorMessage + "\n | "; }
            else
            {
                stateAndLGAs = statesAndLGAs.Where(s => s.Name.Equals(state.Value.Trim(), StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (stateAndLGAs == null)
                { hasError = true; errorMessages += "State not found " + "\n | "; state.HasError = true; state.ErrorMessage = "No State found"; }
                else
                { state.ValueId = stateAndLGAs.StateId; }
            }
            cellSite.State = state;

            var lga = ConvertToString(lineValues.ElementAt(9), "LGA", false, 2, 100);
            if (lga.HasError) { hasError = true; errorMessages += lga.ErrorMessage + "\n | "; }
            else
            {
                if (stateAndLGAs == null)
                {
                    lga.HasError = true; lga.ErrorMessage = "No LGA found for given state " + "\n | ";
                    hasError = true; errorMessages += lga.ErrorMessage;
                }
                else
                {
                    var lgov = stateAndLGAs.LGAs.Where(l => l.Name.Equals(lga.Value, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                    if (lgov == null)
                    {
                        lga.HasError = true; lga.ErrorMessage = "No LGA found for given state " + "\n | ";
                        hasError = true; errorMessages += lga.ErrorMessage;
                    }
                    else { lga.ValueId = lgov.LGAId; }
                }
            }
            cellSite.LGA = lga;


            var towerMastType = ConvertToString(lineValues.ElementAt(10), "TOWER/MAST TYPE", false, 3, 250);
            if (towerMastType.HasError) { hasError = true; errorMessages += towerMastType.ErrorMessage + "\n | "; }
            else { towerMastType.Value = towerMastType.Value.ToUpper(); }
            cellSite.TowerMastType = towerMastType;

            cellSite.HasError = hasError;
            errorMessages = errorMessages.Trim().TrimEnd('|');
            cellSite.ErrorMessages = errorMessages;
            //
            return cellSite;
        }

        protected virtual CellSiteStringValue ConvertToString(string stringValue, string headerValue, bool allowEmpty = false, int minValidLength = 0, int maxValidLength = 0, bool ignoreError = false)
        {
            var result = ValidateLength(stringValue, headerValue, allowEmpty, minValidLength, maxValidLength);
            if (result.HasError) { if (ignoreError) { result.HasError = false; } return result; }
            return new CellSiteStringValue { Value = string.IsNullOrEmpty(stringValue) ? stringValue : stringValue.Trim() };
        }

        protected CellSiteStringValue ValidateLength(string stringValue, string headerValue, bool allowEmpty, int minValidLength = 0, int maxValidLength = 0)
        {
            if (string.IsNullOrEmpty(stringValue))
            {
                if (allowEmpty) { return new CellSiteStringValue { Value = stringValue }; }
                return new CellSiteStringValue { ErrorMessage = string.Format("{0} is empty.", headerValue), HasError = true, Value = stringValue };
            }
            stringValue = stringValue.Trim();

            if (minValidLength > 0)
            {
                if (stringValue.Length < minValidLength)
                { return new CellSiteStringValue { ErrorMessage = string.Format("{0} value is too small. Enter a valid {0}. Minimum number of characters is {1}", headerValue, minValidLength), HasError = true, Value = stringValue }; }
            }

            if (maxValidLength > 0)
            {
                if (stringValue.Length > maxValidLength)
                { return new CellSiteStringValue { ErrorMessage = string.Format("{0} value is too long. Enter a valid {0}. Maximum number of characters is {1}", headerValue, maxValidLength), HasError = true, Value = Truncate(stringValue, maxValidLength) }; }
            }
            return new CellSiteStringValue { };
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
        /// Convert and validate string value to decimal
        /// </summary>
        /// <param name="stringValue">string value</param>
        /// <param name="headerValue">excel header or label header</param>
        /// <param name="allowZero">allow zero</param>
        /// <param name="minValidLength">minimum valid length</param>
        /// <param name="maxValidLength">maximum valid length</param>
        /// <returns>PayeeDecimalValue</returns>
        protected CellSiteDecimalValue ConvertToDecimal(string stringValue, string headerValue, bool allowZero = false, int minValidLength = 0, int maxValidLength = 0, bool ignoreError = false)
        {
            if (string.IsNullOrEmpty(stringValue))
            {
                if (ignoreError)
                { return new CellSiteDecimalValue { StringValue = stringValue, Value = 0.00m }; }

                return new CellSiteDecimalValue
                { StringValue = stringValue, ErrorMessage = string.Format("{0} is empty.", headerValue), HasError = true, Value = 0.00m };
            }

            if (minValidLength > 0)
            {
                if (stringValue.Length < minValidLength)
                {
                    if (!allowZero)
                    {
                        return new CellSiteDecimalValue { HasError = ignoreError ? false : true, StringValue = stringValue, Value = 0.00m, ErrorMessage = string.Format("{0} value is too short. Minimum value allowed is {1}.", headerValue, minValidLength) };
                    }
                }
            }

            if (maxValidLength > 0)
            {
                if (stringValue.Length > maxValidLength)
                {
                    return new CellSiteDecimalValue { ErrorMessage = string.Format("{0} value is too long. Maximum value allowed is {1}.", headerValue, maxValidLength), HasError = ignoreError ? false : true, StringValue = Truncate(stringValue, maxValidLength), Value = 0.00m };
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
                return new CellSiteDecimalValue { StringValue = stringValue, ErrorMessage = string.Format("{0} is not valid.", headerValue), HasError = true, };
            }

            if (decimalValue <= 0 && !allowZero)
            {
                return new CellSiteDecimalValue { StringValue = stringValue, HasError = true, ErrorMessage = string.Format("{0} is not valid.", headerValue) };
            }
            decimalValue = Math.Round(decimalValue, 2);
            return new CellSiteDecimalValue { Value = decimalValue, StringValue = string.Format("{0:n}", decimalValue) };
        }

        protected CellSiteIntValue ConvertToInteger(string stringValue, string headerValue, bool allowEmpty = false, int minValidLength = 0, int maxValidLength = 0, bool ignoreError = false)
        {
            var result = ValidateLength(stringValue, headerValue, allowEmpty, minValidLength, maxValidLength);
            if (result.HasError)
            {
                if (ignoreError)
                {
                    result.HasError = false; result.ErrorMessage = string.Empty;
                }
                //Return zero
                return new CellSiteIntValue { Value = 0, StringValue = result.Value, HasError = result.HasError, ErrorMessage = result.ErrorMessage };
            }

            int userValue = 0;
            bool parsed = Int32.TryParse(stringValue, out userValue);

            if (!parsed)
            {
                return new CellSiteIntValue { Value = 0, ErrorMessage = string.Format("{0} should be an integer value.", headerValue), HasError = ignoreError ? false : true };
            }
            return new CellSiteIntValue { Value = userValue, StringValue = userValue.ToString().Trim() };
        }

    }



}
