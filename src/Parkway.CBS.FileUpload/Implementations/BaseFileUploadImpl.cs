using Parkway.CBS.FileUpload;
using Parkway.CBS.FileUpload.Implementations.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Parkway.CBS.FileUpload.Implementations
{
    public abstract class BaseFileUploadImpl
    {
        private Dictionary<string, int> months = new Dictionary<string, int> { { "Jan", 1 }, { "Feb", 2 }, { "Mar", 3 }, { "Apr", 4 }, { "May", 5 }, { "Jun", 6 }, { "Jul", 7 }, { "Aug", 8 }, { "Sep", 9 }, { "Oct", 10 }, { "Nov", 11 }, { "Dec", 12 } };

        protected OSGOFSites GetCellSiteStates(string xmlFilePath)
        {
            string xmlString = GetXMLString(xmlFilePath);

            XmlSerializer serializer = new XmlSerializer(typeof(OSGOFSites));
            OSGOFSites OSGOFSites = new OSGOFSites { };
            using (StringReader reader = new StringReader(xmlString))
            {
                OSGOFSites = (OSGOFSites)serializer.Deserialize(reader);
            }
            if (OSGOFSites == null)
            {
                var noSitesFound = new OSGOFSites { };
                noSitesFound.ListOfStates.Select(impls => new List<OSGOFState> { });
                return noSitesFound;
            }
            return OSGOFSites;
        }


        private string GetXMLString(string xmlFilePath)
        {
            try
            {
                string xmlstring = string.Empty;
                var vvv = XElement.Load(xmlFilePath);
                foreach (XElement elements in XElement.Load(xmlFilePath).Elements("OSGOFSites"))
                {
                    xmlstring = elements.ToString();
                }
                return xmlstring;
            }
            catch (Exception) { throw new Exception("Could not validate OSGOFSites"); }
        }


        protected ValidationObject ValidateString(FileUploadCellSites site)
        {
            bool hasError = false;
            string errorMessages = string.Empty;
            //if (string.IsNullOrEmpty(site.Month)) { hasError = true; errorMessages = "Month of payment is required \n | "; }

            if (string.IsNullOrEmpty(site.Year)) { hasError = true; errorMessages = "Year of payment is required \n | "; }

            //if (string.IsNullOrEmpty(site.State)) { hasError = true; errorMessages = "The state value is required \n | "; }

            //if (string.IsNullOrEmpty(site.LGA)) { hasError = true; errorMessages = "The LGA value is required \n | "; }

            if (string.IsNullOrEmpty(site.CellSiteId)) { hasError = true; errorMessages = "A cell site value is required \n | "; }

            return new ValidationObject { HasError = hasError, ErrorMessages = errorMessages };
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="site"></param>
        /// <returns>OSGOFCellSiteModel</returns>
        protected ValidationObject ValidateString(CellSite site)
        {
            bool hasError = false;
            string errorMessages = string.Empty;
            //if (string.IsNullOrEmpty(site.Month)) { hasError = true; errorMessages = "Month of payment is required \n | "; }

            if (string.IsNullOrEmpty(site.Year)) { hasError = true; errorMessages = "Year of payment is required \n | "; }

            //if (string.IsNullOrEmpty(site.State)) { hasError = true; errorMessages = "The state value is required \n | "; }

            //if (string.IsNullOrEmpty(site.LGA)) { hasError = true; errorMessages = "The LGA value is required \n | "; }

            if (string.IsNullOrEmpty(site.CellSiteId)) { hasError = true; errorMessages = "A cell site value is required \n | "; }

            return new ValidationObject { HasError = hasError , ErrorMessages = errorMessages};
        }



        /// <summary>
        /// Validate value to string
        /// </summary>
        /// <param name="stringValue">value to be validated against</param>
        /// <param name="headerValue">name of the excel header value</param>
        /// <param name="allowEmpty">allow empty string values</param>
        /// <param name="minValidLength">minimum length of the string</param>
        /// <param name="maxValidLength">maximum length</param>
        /// <returns>PayeeStringValue</returns>
        protected ValidationObject ConvertToString(string stringValue, string headerValue, bool allowEmpty = false, int minValidLength = 0, int maxValidLength = 0)
        {
            var result = ValidateLength(stringValue, headerValue, allowEmpty, minValidLength, maxValidLength);
            if (result.HasError) { return result; }
            return new ValidationObject { Value = string.IsNullOrEmpty(stringValue) ? stringValue : stringValue.Trim() };
        }

        
        protected ValidationObject ValidateLength(string stringValue, string headerValue, bool allowEmpty, int minValidLength = 0, int maxValidLength = 0)
        {
            if (string.IsNullOrEmpty(stringValue))
            {
                if (allowEmpty) { return new ValidationObject { Value = stringValue }; }
                return new ValidationObject { ErrorMessages = string.Format("{0} is empty.", headerValue), HasError = true, Value = stringValue };
            }

            stringValue = stringValue.Trim();

            if (minValidLength > 0)
            {
                if (stringValue.Length < minValidLength)
                { return new ValidationObject { ErrorMessages = string.Format("{0} value is too small. Enter a valid {0}. Minimum number of characters is {1}", headerValue, minValidLength), HasError = true, Value = stringValue }; }
            }

            if (maxValidLength > 0)
            {
                if (stringValue.Length > maxValidLength)
                { return new ValidationObject { ErrorMessages = string.Format("{0} value is too long. Enter a valid {0}. Maximum number of characters is {1}", headerValue, maxValidLength), HasError = true, Value = Truncate(stringValue, maxValidLength) }; }
            }
            return new ValidationObject { Value = stringValue };
        }


        /// <summary>
        /// Convert or validate string value to int
        /// </summary>
        /// <param name="year">string value</param>
        /// <param name="headerValue">header value</param>
        /// <param name="minValidLength">minimum valid string length</param>
        /// <param name="maxValidLength">maximum string length</param>
        /// <returns>PayeeIntValue</returns>
        protected ValidationObject ValidateYear(string year, string headerValue, int minValidLength = 0, int maxValidLength = 0)
        {
            if (string.IsNullOrEmpty(year))
            { return new ValidationObject { ErrorMessages = string.Format("{0} is empty.", headerValue), HasError = true, Value = year }; }

            if (minValidLength > 0)
            {
                if (year.Length < minValidLength) { return new ValidationObject { ErrorMessages = string.Format("{0} value is small. Enter a valid {0}.", headerValue), HasError = true, Value = year }; }
            }

            if (year.Length > 4)
            {
                if (year.Length >= 11)
                {
                    return new ValidationObject { ErrorMessages = string.Format("{0} value is too long. Enter a valid {0}.", headerValue), HasError = true, Value = Truncate(year, 11) };
                }
                else
                {
                    return new ValidationObject { ErrorMessages = string.Format("{0} value is too long. Enter a valid {0}.", headerValue), HasError = true, Value = year };
                }
            }

            int intValue = 0;
            //try converting
            if (!Int32.TryParse(year.Trim(), out intValue))
            {
                return new ValidationObject { ErrorMessages = string.Format("{0} is not a valid year.", headerValue), HasError = true, Value = year };
            }

            if (intValue <= 0)
            {
                return new ValidationObject { Value = year.Trim(), HasError = true, ErrorMessages = string.Format("{0} is not valid.", headerValue) };
            }
            return new ValidationObject { Value = intValue.ToString(), IntValue = intValue };
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
        /// Validate month
        /// </summary>
        /// <param name="month"></param>
        /// <returns>PayeeStringValue</returns>
        protected ValidationObject ValidateMonth(string month)
        {
            return new ValidationObject { Value = "", HasError = false };
            var result = ConvertToString(month, "Month", false, 0, 11);
            if (result.HasError) { return result; }

            var value = months.Where(m => m.Key.ToLower() == month.Trim().ToLower()).FirstOrDefault();

            if (value.Key == null)
            { return new ValidationObject { ErrorMessages = "Month value not found. Expected format are Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec.", HasError = true, Value = month }; }

            return new ValidationObject { Value = month.Trim() };
        }


        /// <summary>
        /// Validate the cell sites
        /// </summary>
        /// <param name="fileUploadCellSites"></param>
        /// <param name="xmlFilePath"></param>
        /// <returns>CellSitesBreakDown</returns>
        protected CellSitesBreakDown ValidateCellSites(List<FileUploadCellSites> fileUploadCellSites, string xmlFilePath)
        {
            ConcurrentStack<OSGOFCellSiteModelV2> validatedInput = new ConcurrentStack<OSGOFCellSiteModelV2> { };
            Parallel.ForEach(fileUploadCellSites, (item) =>
            //foreach (var item in fileUploadCellSites)
            {
                StringBuilder errorMessages = new StringBuilder();
                var hasError = false;

                var validationResponse = ValidateString(item);
                if (validationResponse.HasError) { hasError = true; errorMessages.Append(validationResponse.ErrorMessages); }
                //validate the cell site Id
                var cellVal = ConvertToString(item.CellSiteId, "Cell Site Id", false, 2, 100);
                if (cellVal.HasError)
                {
                    hasError = true;
                    errorMessages.Append(cellVal.ErrorMessages + "\n | ");
                }

                //validate year
                var valObj = ValidateYear(item.Year, "Year", 4, 11);
                if (valObj.HasError)
                {
                    hasError = true;
                    errorMessages.Append(valObj.ErrorMessages + "\n | ");
                }


                //validate ref
                var rvalObj = ConvertToString(item.Ref, "Reference", true, 0, 100);
                if (rvalObj.HasError)
                {
                    hasError = true;
                    errorMessages.Append(rvalObj.ErrorMessages + "\n | ");
                }

                DateTime? assessmentDate = null;
                if (!hasError)
                {
                    assessmentDate = GetAssessmentDate(valObj);
                    if (assessmentDate == null)
                    {
                        if (!valObj.HasError)
                        {
                            hasError = true;
                            errorMessages.Append("Could not compute a date from the cell site month and year. Enter a valid year.");
                        }
                    }
                }
                //do a break down summary
                validatedInput.Push(new OSGOFCellSiteModelV2 { ErrorMessages = errorMessages.ToString().Trim().TrimEnd('|'), HasError = hasError, AssessmentDate = assessmentDate, CellSiteValue = cellVal.Value, Ref = rvalObj.Value, Year = valObj.IntValue, YearStringValue = valObj.Value });
            });
            return new CellSitesBreakDown { CellSiteModelV2 = validatedInput };
        }
        

        protected CellSitesBreakDown ValidateCellSites(ICollection<CellSite> cellSites, string xmlFilePath)
        {
            OSGOFSites sites = GetCellSiteStates(xmlFilePath);
            ConcurrentStack<OSGOFCellSiteModel> validatedInput = new ConcurrentStack<OSGOFCellSiteModel> { };

            Parallel.ForEach(cellSites, (item) =>
            {
                StringBuilder errorMessages = new StringBuilder();
                var hasError = false;
                decimal amount = 0.00m;

                var validationResponse = ValidateString(item);
                if (validationResponse.HasError) { hasError = true; errorMessages.Append(validationResponse.ErrorMessages); }

                //validate state
                //get state from list of states
                string stateId = null;
                var stateResult = sites.ListOfStates.FirstOrDefault(state => state.Id == item.State);
                if (stateResult == null)
                { hasError = true; errorMessages.Append("State Id could not be found " + "\n | "); }
                else { stateId = stateResult.Name; }

                //validate LGA
                string lgaId = null;
                OSGOFStateLGA lgaResult = null;
                if(stateResult == null)
                {
                    hasError = true;
                    errorMessages.Append("LGA Id could not be found " + "\n | ");
                }
                else
                {
                    lgaResult = stateResult.ListOfLGAs.FirstOrDefault(lga => lga.Id == item.LGA);
                    if (lgaResult == null)
                    {
                        hasError = true;
                        errorMessages.Append("LGA Id could not be found " + "\n | ");
                    }
                    else
                    {
                        lgaId = lgaResult.Name;
                    }
                }


                //validate site
                string cellSiteName = null;
                string coords = null;
                string address = null;
                OSGOFCellSites sitesResult = null;
                if (lgaResult == null)
                {
                    hasError = true;
                    errorMessages.Append("Cell Site could not be found " + "\n | ");
                }
                else {
                    sitesResult = lgaResult.ListOfSites.FirstOrDefault(cell => cell.Id == item.CellSiteId);
                    if (sitesResult == null)
                    {
                        hasError = true;
                        errorMessages.Append("Cell Site could not be found " + "\n | ");
                    }
                    else
                    {
                        amount = sitesResult.Amount;
                        cellSiteName = sitesResult.Name;
                        coords = sitesResult.Coors;
                        address = sitesResult.Address;
                    }
                }                

                //validate year
                var valObj = ValidateYear(item.Year, "Year", 4, 11);
                if (valObj.HasError)
                {
                    hasError = true;
                    errorMessages.Append(valObj.ErrorMessages + "\n | ");
                }

                //validate month
                var mvalObj = ValidateMonth(item.Month);
                if (mvalObj.HasError)
                {
                    hasError = true;
                    errorMessages.Append(mvalObj.ErrorMessages + "\n | ");
                }

                //validate ref
                var rvalObj = ConvertToString(item.Ref, "Reference", true, 0, 100);
                if (rvalObj.HasError)
                {
                    hasError = true;
                    errorMessages.Append(rvalObj.ErrorMessages + "\n | ");
                }

                DateTime? assessmentDate = null;
                if (!hasError)
                {
                    assessmentDate = GetAssessmentDate(valObj, mvalObj);
                    if (assessmentDate == null)
                    {
                        if (!mvalObj.HasError && !valObj.HasError)
                        {
                            hasError = true;
                            errorMessages.Append("Could not compute a date from the cell site month and year. Enter valid month and year.");
                        }
                    }
                }
                //do a break down summary
                validatedInput.Push(new OSGOFCellSiteModel { StateValue = stateId, LGAValue = lgaId, CellSiteValue = cellSiteName, Amount = amount, AmountValue = string.Format("{0:n2}", amount), Month = string.IsNullOrEmpty(mvalObj.Value)? mvalObj.Value : mvalObj.Value.ToUpper(), Year = valObj.Value, AssessmentDate = assessmentDate, ErrorMessages = errorMessages.ToString().Trim().TrimEnd('|'), HasError = hasError, Ref = rvalObj.Value, StateName = item.State, LGAId = item.LGA, Address = address, CellSiteCode = item.CellSiteId, Coords = coords });
            });

            var totalAmount = validatedInput.Sum(x => x.Amount);
            return new CellSitesBreakDown { CellSiteModel = validatedInput, TotalAmount = totalAmount };
        }


        private DateTime? GetAssessmentDate(ValidationObject yearVal)
        {
            if (yearVal.HasError) { return null; }
            var lastDayOfMonth = DateTime.DaysInMonth(yearVal.IntValue, 12);
            return new DateTime(yearVal.IntValue, 12, lastDayOfMonth);
        }

        /// <summary>
        /// Get assessment date this paye is for
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns>DateTime | null</returns>
        private DateTime? GetAssessmentDate(ValidationObject yearVal, ValidationObject monthVal)
        {
            if (yearVal.HasError) { return null; }
            //if (monthVal.HasError) { return null; }
            //var value = months.Where(m => m.Key.ToLower() == monthVal.Value.ToLower()).FirstOrDefault();
            //if (value.Key == null)
            //{ return null; }
            var lastDayOfMonth = DateTime.DaysInMonth(yearVal.IntValue, 12);
            return new DateTime(yearVal.IntValue, 12, lastDayOfMonth);
        }
    }
}
