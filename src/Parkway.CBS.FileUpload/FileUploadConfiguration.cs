using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Serialization;
using Parkway.CBS.FileUpload.Implementations.Contracts;
using System.Data;
using ExcelDataReader;
using Parkway.CBS.FileUpload.BModels;
using System.Text;
using System.Globalization;

namespace Parkway.CBS.FileUpload
{
    public class FileUploadConfiguration : IFileUploadConfiguration
    {
        /// <summary>
        ///  CELL SITE ID - 0, 
        ///  Year - 1, 
        ///  Reference - 2,
        /// </summary>
        protected readonly string[] headerNames = { "CELL SITE ID".ToLower() , "Year".ToLower(), "Reference".ToLower() };


        /// <summary>
        /// Get the implementing class for this template
        /// </summary>
        /// <param name="selectedTemplate"></param>
        /// <param name="selectedImpl"></param>
        /// <param name="xmlFilePathForTemplates"></param>
        /// <returns>IFileUploadImplementations</returns>
        public IFileUploadImplementations GetImplementation(string selectedTemplate, string selectedImpl, string xmlFilePathForTemplates)
        {
            Template template = Templates(xmlFilePathForTemplates).Where(templ => templ.Name == selectedTemplate).FirstOrDefault();
            if (template == null) { throw new Exception("No template found " + selectedTemplate); }

            UploadImplInterface impl = template.ListOfUploadImplementations.Where(imple => imple.Value == selectedImpl).FirstOrDefault();
            if (impl == null) { throw new Exception("No template imple found " + selectedImpl); }

            return (IFileUploadImplementations)Activator.CreateInstance(Type.GetType(impl.ClassName));
        }


        public List<Template> Templates(string xmlFilePath)
        {
            string xmlString = GetXMLString(xmlFilePath);

            XmlSerializer serializer = new XmlSerializer(typeof(FileUploadTemplates));
            FileUploadTemplates templateObj = new FileUploadTemplates { };
            using (StringReader reader = new StringReader(xmlString))
            {
                templateObj = (FileUploadTemplates)serializer.Deserialize(reader);
            }
            if (templateObj == null)
            {
                var noTemplatesFound = new List<Template> { };
                noTemplatesFound.Select(impls => new List<UploadImplInterface> { });
                return noTemplatesFound;
            }
            return templateObj.ListOfTemplates;
        }


        private string GetXMLString(string xmlFilePath)
        {
            try
            {
                string xmlstring = string.Empty;
                
                foreach (XElement elements in XElement.Load(xmlFilePath).Elements("FileUploadTemplates"))
                {
                    xmlstring = elements.ToString();
                }
                return xmlstring;
            }
            catch (Exception) { throw new Exception("Could not validate LGA"); }
        }


        public List<ScrapFileModel> ReadIndv()
        {
            //lets read the file first
            DataSet result = new DataSet();
            using (var stream = File.Open("C:\\Parkway\\Repository\\CBS\\src\\Orchard.Web\\scrap\\nasarawa_indv.xlsx", FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                { result = reader.AsDataSet(); }
            }
            var sheet1 = result.Tables[0];
            var rows = sheet1.Rows;
            //
            List<ScrapFileModel> scraps = new List<ScrapFileModel> { };
            List<ScrapFileModel> invalidscraps = new List<ScrapFileModel> { };

            int counter = 2;

            //0 - TIN //1 - Previous_TIN //2 - Surname //3 - first name //4 - middle name //5 - birth date //6 - gender //7 - reg date //8 - house no//9 - street//10 - city//11 - lga//12 - ward//13 - email//14 - phone// 15 - occupation
            //16 - source of income//17 - nationality//18 - state of residence//19 - state of origin//20 - tax auth
            rows.RemoveAt(0);
            foreach (DataRow item in rows)
            {
                StringBuilder recp = new StringBuilder { };

                if (!string.IsNullOrEmpty(item.ItemArray[2].ToString())) //surname
                {
                    recp.AppendFormat("{0} ", item.ItemArray[2].ToString());
                }

                if (!string.IsNullOrEmpty(item.ItemArray[3].ToString())) //first name
                {
                    recp.AppendFormat("{0} ", item.ItemArray[3].ToString());
                }

                if (!string.IsNullOrEmpty(item.ItemArray[4].ToString())) // middle name
                {
                    recp.AppendFormat("{0} ", item.ItemArray[4].ToString());
                }

                if (string.IsNullOrEmpty(recp.ToString()))
                {
                    invalidscraps.Add(new ScrapFileModel { LineNumber = counter, Message = "Recipient empty" });
                    continue;
                }


                if (string.IsNullOrEmpty(item.ItemArray[14].ToString())) //phone number
                {
                    invalidscraps.Add(new ScrapFileModel { LineNumber = counter, Message = "no phone number" });
                    continue;
                }

                if (item.ItemArray[14].ToString().Length < 11) //phone number
                {
                    invalidscraps.Add(new ScrapFileModel { LineNumber = counter, Message = "invalid phone number PHN " + item.ItemArray[14].ToString() });
                    continue;
                }

                //check if phone number already exists
                var alreadyExists = scraps.Where(x => x.Phone == item.ItemArray[14].ToString()).FirstOrDefault();

                if(alreadyExists != null)
                {
                    invalidscraps.Add(new ScrapFileModel { LineNumber = counter, Message = "Phone number duplicated" });
                    continue;
                }


                string gender = "None";

                if (!string.IsNullOrEmpty(item.ItemArray[6].ToString())) // gender
                {
                    if(item.ItemArray[6].ToString().ToLower() == "m") { gender = "Male"; }
                    else if(item.ItemArray[6].ToString().ToLower() == "f") { gender = "Female"; }
                    else { gender = "None"; }
                }

                //
                //0 - TIN //1 - Previous_TIN //2 - Surname //3 - first name //4 - middle name //5 - birth date //6 - gender //7 - reg date //8 - house no//9 - street//10 - city//11 - lga//12 - ward//13 - email//14 - phone// 15 - occupation
                //16 - source of income//17 - nationality//18 - state of residence//19 - state of origin//20 - tax auth
                StringBuilder addy = new StringBuilder { };
                if (!string.IsNullOrEmpty(item.ItemArray[8].ToString()))//house number
                {
                    if (item.ItemArray[8].ToString() != "N/A")
                    {
                        if (item.ItemArray[8].ToString() != "N\\A")
                        {
                            if (item.ItemArray[8].ToString() != "NIL")
                            {
                                if (item.ItemArray[8].ToString() != "NA")
                                {
                                    addy.AppendFormat("{0} ", item.ItemArray[8].ToString());
                                }
                            }
                        }
                    }
                }

                if (addy.ToString().Length < 5)
                {
                    invalidscraps.Add(new ScrapFileModel { LineNumber = counter, Message = "Address field no house number" });
                    continue;
                }

                string street = string.Empty;

                if (!string.IsNullOrEmpty(item.ItemArray[9].ToString()))//street
                {
                    if (item.ItemArray[9].ToString() != "N/A")
                    {
                        if (item.ItemArray[9].ToString() != "N\\A")
                        {
                            if (item.ItemArray[9].ToString() != "NIL")
                            {
                                if (item.ItemArray[9].ToString() != "NA")
                                {
                                    street = string.Format("{0}", item.ItemArray[9].ToString());
                                }
                            }
                        }
                    }
                }

                if(street.Length < 5)
                {
                    invalidscraps.Add(new ScrapFileModel { LineNumber = counter, Message = "Address field" });
                }

                addy.AppendFormat("{0} ", item.ItemArray[9].ToString());



                DateTime dobp = DateTime.Now;
                DateTime? dob = null;
                bool parsed = DateTime.TryParseExact(item.ItemArray[5].ToString(), "dd-MMM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dobp);

                if (parsed) { dob = dobp; }

                //
                DateTime regp = DateTime.Now;
                DateTime? reg = null;
                parsed = DateTime.TryParseExact(item.ItemArray[7].ToString(), "dd-MMM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out regp);

                if (parsed) { reg = regp; }

                //0 - TIN //1 - Previous_TIN //2 - Surname //3 - first name //4 - middle name //5 - birth date //6 - gender //7 - reg date //8 - house no//9 - street//10 - city//11 - lga//12 - ward//13 - email//14 - phone// 15 - occupation
                //16 - source of income//17 - nationality//18 - state of residence//19 - state of origin//20 - tax auth
                scraps.Add(new ScrapFileModel
                {
                    TIN = item.ItemArray[0].ToString(),
                    Prev_TIN = item.ItemArray[1].ToString(),
                    CombinedName = recp.ToString().Trim(),
                    //Sname = item.ItemArray[2].ToString(),
                    //Fname = item.ItemArray[3].ToString(),
                    //MName = item.ItemArray[4].ToString(),
                    BOD = dob,
                    Gender = gender,
                    RegDate = reg,
                    HouseNo = item.ItemArray[8].ToString(),
                    Street = item.ItemArray[9].ToString(),
                    City = item.ItemArray[10].ToString(),
                    Address = addy.ToString().Trim(),
                    Lga = item.ItemArray[11].ToString(),
                    Ward = item.ItemArray[12].ToString(),
                    Email = item.ItemArray[13].ToString(),
                    Phone = item.ItemArray[14].ToString(),
                    Occupation = item.ItemArray[15].ToString(),
                    Source = item.ItemArray[16].ToString(),
                    Nationality = item.ItemArray[17].ToString(),
                    State = item.ItemArray[18].ToString(),
                    StateO = item.ItemArray[19].ToString(),
                    TaxAuth = item.ItemArray[20].ToString(),
                });

                counter++;
            }
            var dfd = invalidscraps;
            return scraps;
        }


        /// <summary>
        /// validate the headers and reads the file contents
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>CellSiteFileProcessResponse</returns>
        public CellSiteFileProcessResponse ReadFile(string filePath)
        {
            //lets read the file first
            DataSet result = new DataSet();
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                { result = reader.AsDataSet(); }
            }
            var sheet1 = result.Tables[0];
            var rows = sheet1.Rows;

            Dictionary<string, OSGOFHeaderValidationModel> headers = new Dictionary<string, OSGOFHeaderValidationModel>
            {
                { headerNames[0], new OSGOFHeaderValidationModel { } }, { headerNames[1], new OSGOFHeaderValidationModel { } }, { headerNames[2], new OSGOFHeaderValidationModel { } }
            };

           ValidateHeaders(rows[0], ref headers);

            var invalidsHeaders = headers.Where(k => k.Value.HeaderPresent == false);
            if (invalidsHeaders.Count() > 0)
            {
                var msg = invalidsHeaders.Select(x => x.Key + " header not found").ToArray();
                return new CellSiteFileProcessResponse { HeaderValidationObject = new OSGOFCellSiteHeaderValidateObject { Error = true, ErrorMessage = string.Join("\n", msg) } };
            }

            rows.RemoveAt(0);

            //TODO add concurrent stack here
            var col = sheet1.Columns;
            List<FileUploadCellSites> fileUploadCellSites = new List<FileUploadCellSites> { };

            foreach (DataRow item in rows)
            {
                fileUploadCellSites.Add(new FileUploadCellSites { CellSiteId = item.ItemArray[headers[headerNames[0]].IndexOnFile].ToString(), Year = item.ItemArray[headers[headerNames[1]].IndexOnFile].ToString(), Ref = item.ItemArray[headers[headerNames[2]].IndexOnFile].ToString() });
            }
            return new CellSiteFileProcessResponse { HeaderValidationObject = new OSGOFCellSiteHeaderValidateObject { }, FileUploadCellSites = fileUploadCellSites };
        }



        /// <summary>
        /// validate the headers and reads the file contents
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        ////public CellSiteFileProcessResponse ReadFile(string filePath)
        ////{
        ////    //lets read the file first
        ////    DataSet result = new DataSet();
        ////    using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        ////    {
        ////        using (var reader = ExcelReaderFactory.CreateReader(stream))
        ////        { result = reader.AsDataSet(); }
        ////    }
        ////    var sheet1 = result.Tables[0];
        ////    var rows = sheet1.Rows;

        ////    OSGOFCellSiteHeaderValidateObject headerIsValid = ValidateHeaders(rows[0]);

        ////    if (headerIsValid.Error)
        ////    {
        ////        return new CellSiteFileProcessResponse { HeaderValidationObject = headerIsValid };
        ////    }

        ////    rows.RemoveAt(0);

        ////    var col = sheet1.Columns;
        ////    List<FileUploadCellSites> fileUploadCellSites = new List<FileUploadCellSites> { };

        ////    foreach (DataRow item in rows)
        ////    {
        ////        fileUploadCellSites.Add(new FileUploadCellSites { CellSiteId = item.ItemArray[0].ToString(), Ref = item.ItemArray[1].ToString(), Month = item.ItemArray[2].ToString(), Year = item.ItemArray[3].ToString() });
        ////    }

        ////    return new CellSiteFileProcessResponse { HeaderValidationObject = new OSGOFCellSiteHeaderValidateObject { }, FileUploadCellSites = fileUploadCellSites };
        ////}


        private void ValidateHeaders(DataRow header, ref Dictionary<string, OSGOFHeaderValidationModel> headers)
        {
            string errorMessage = string.Empty;
            int counter = -1;
            foreach (object item in header.ItemArray)
            {
                //we are expecting only 2 columns, hence this check
                if (counter > 2) { break; }

                if (item is DBNull) { break; }
                counter++;
                string sItem = ((string)item).Trim().ToLower();

                if (headerNames[0].Contains(sItem)) { headers[headerNames[0]] = new OSGOFHeaderValidationModel { HeaderPresent = true, IndexOnFile = counter }; continue; }
                if (headerNames[1].Contains(sItem)) { headers[headerNames[1]] = new OSGOFHeaderValidationModel { HeaderPresent = true, IndexOnFile = counter }; continue; }
                if (headerNames[2].Contains(sItem)) { headers[headerNames[2]] = new OSGOFHeaderValidationModel { HeaderPresent = true, IndexOnFile = counter }; continue; }

            }
            //bool headerError = false;
            //string errorMessage = string.Empty;
            //if (!header.ItemArray[0].ToString().Trim().ToLower().Contains("CELL SITE ID".ToLower())) { headerError = true; errorMessage = "Cell Site header not found\n"; }
            //if (!header.ItemArray[2].ToString().Trim().ToLower().Contains("MONTH".ToLower())) { headerError = true; errorMessage += ", Month header not found \n"; }
            //if (!header.ItemArray[3].ToString().Trim().ToLower().Contains("YEAR".ToLower())) { headerError = true; errorMessage += ", Year header not found \n"; }
            //return new OSGOFCellSiteHeaderValidateObject { Error = headerError, ErrorMessage = errorMessage.Trim(',').Trim() };
        }
    }



    public interface IFileUploadConfiguration
    {

        /// <summary>
        /// Get a list of templates available
        /// </summary>
        /// <param name="xmlFilePath"></param>
        /// <returns>List{Template}</returns>
        List<Template> Templates(string xmlFilePath);


        /// <summary>
        /// Get the implementing class for this template
        /// </summary>
        /// <param name="selectedTemplate"></param>
        /// <param name="selectedImpl"></param>
        /// <param name="xmlFilePathForTemplates"></param>
        /// <returns>IFileUploadImplementations</returns>
        IFileUploadImplementations GetImplementation(string selectedTemplate, string selectedImpl, string xmlFilePathForTemplates);

        /// <summary>
        /// validate the headers and reads the file contents
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        CellSiteFileProcessResponse ReadFile(string filePath);

        List<ScrapFileModel> ReadIndv();
    }
}
