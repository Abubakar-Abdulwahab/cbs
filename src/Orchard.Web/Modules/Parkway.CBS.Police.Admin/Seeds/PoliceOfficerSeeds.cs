using ExcelDataReader;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Police.Core.Utilities;
using Parkway.CBS.Police.Admin.Seeds.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Parkway.CBS.Police.Admin.Seeds
{
    public class PoliceOfficerSeeds : IPoliceOfficerSeeds
    {
        private const int templateHeaderCount = 8;
        protected readonly string[] templateHeaderNames = {
            "Identification Number".ToLower() , "Name".ToLower(), "Gender".ToLower(), "IPPIS Number".ToLower(),"Rank Id".ToLower(), "Command Id".ToLower(), "Account Number".ToLower(), "Bank Code".ToLower() };
        private readonly ICommandManager<Command> _commandManager;
        private readonly IPoliceOfficerManager<PoliceOfficer> _policeOfficerManager;

        public PoliceOfficerSeeds(ICommandManager<Command> commandManager, IPoliceOfficerManager<PoliceOfficer> policeOfficerManager)
        {
            _commandManager = commandManager;
            _policeOfficerManager = policeOfficerManager;
        }

        public PoliceOfficerStatVM UploadOfficers()
        {
            try
            {
                int countersuccessful = 0;
                int counterunsuccessful = 0;
                List<PoliceOfficerSeedingVM> unsuccessfulList = new List<PoliceOfficerSeedingVM>();
                List<PoliceOfficer> officerList = new List<PoliceOfficer>();
                PoliceOfficerResponse response = ProcessFile();

                foreach (var officer in response.OfficerRecords)
                {
                    var command = _commandManager.GetCommandDetails(officer.CommandId);

                    if (officer.Error || command == null)
                    {
                        counterunsuccessful++;
                        unsuccessfulList.Add(officer);
                    }
                    else
                    {
                        PoliceOfficer officerObj = new PoliceOfficer
                        {
                            IdentificationNumber = officer.IdentificationNumber,
                            IPPISNumber = officer.IPPISNumber,
                            Rank = new PoliceRanking { Id = officer.RankId },
                            Name = officer.Name,
                            Command = new Command { Id = officer.CommandId },
                            IsActive = true,
                            Gender_Id = (int)officer.Gender,
                            AccountNumber = officer.AccountNumber,
                            BankCode = officer.BankCode
                        };
                        officerList.Add(officerObj);
                        countersuccessful++;
                    }
                }
                _policeOfficerManager.SaveBundleUnCommitStateless(officerList);
                return new PoliceOfficerStatVM { NumberofSuccessfulRecords = countersuccessful, NumberofUnSuccessfulRecords = counterunsuccessful, UnSuccessfulRecords = unsuccessfulList };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public PoliceOfficerResponse ProcessFile()
        {
            try
            {
                string filePath = PSSUtil.GetOfficerExcelFilePath();
                DataSet result = new DataSet();
                using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    { result = reader.AsDataSet(); }
                }

                var sheet1 = result.Tables[0];
                var rows = sheet1.Rows;

                Dictionary<string, TemplateHeaderValidation> headers = new Dictionary<string, TemplateHeaderValidation>();
                for (int i = 0; i < templateHeaderCount; i++)
                {
                    headers.Add(templateHeaderNames[i], new TemplateHeaderValidation { });
                }

                ValidateTemplateHeaders(rows[0], ref headers);
                PoliceOfficerResponse officerData = new PoliceOfficerResponse();

                var invalidsHeaders = headers.Where(k => k.Value.HeaderPresent == false);
                if (invalidsHeaders.Count() > 0)
                {
                    var msg = invalidsHeaders.Select(x => x.Key.ToUpper() + " header not found").ToArray();
                    officerData.HeaderValidateObject = new HeaderValidateObject { Error = true, ErrorMessage = string.Join(". ", msg) };
                }

                ConcurrentStack<PoliceOfficerSeedingVM> officerRecords = new ConcurrentStack<PoliceOfficerSeedingVM> { };
                if (officerData.HeaderValidateObject == null)
                {
                    rows.RemoveAt(0);
                    var col = sheet1.Columns;
                    IEnumerable<DataRow> irows = rows.OfType<DataRow>();

                    Parallel.ForEach(irows, (item) =>
                    {
                        List<string> lineValues = new List<string>();
                        for (int i = 0; i < templateHeaderCount; i++)
                        {
                            lineValues.Add(item.ItemArray[headers[templateHeaderNames[i]].IndexOnFile].ToString());
                        }

                        var officerLineRecord = GetOfficerLineRecord(lineValues);
                        officerRecords.Push(officerLineRecord);
                    });
                    officerData.OfficerRecords = officerRecords;
                }

                if (officerData.HeaderValidateObject != null)
                {
                    return officerData;
                }

                return officerData;
            }
            catch (Exception ex)
            {
                //Log error
                throw;
            }
        }

        protected void ValidateTemplateHeaders(DataRow header, ref Dictionary<string, TemplateHeaderValidation> headers)
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
                    if (templateHeaderNames[i].Equals(sItem)) { headers[templateHeaderNames[i]] = new TemplateHeaderValidation { HeaderPresent = true, IndexOnFile = counter }; continue; }
                }
            }
        }

        protected PoliceOfficerSeedingVM GetOfficerLineRecord(List<string> lineValues)
        {
            var officer = new PoliceOfficerSeedingVM();

            officer.IdentificationNumber = lineValues.ElementAt(0).Trim();

            officer.Name = lineValues.ElementAt(1).Trim();

            bool parsed = Enum.TryParse(lineValues.ElementAt(2).Trim(), true, out Gender gender);
            if (!parsed)
            {
                officer.Error = true;
                officer.ErrorMessage = "Unable to parse Gender";
                officer.Gender = 0;
            }
            else { officer.Gender = gender; }      
            
            officer.IPPISNumber = lineValues.ElementAt(3).Trim();
            int rankId = 0;
            parsed = Int32.TryParse(lineValues.ElementAt(4), out rankId);
            if (!parsed)
            {
                officer.Error = true;
                officer.ErrorMessage = "Unable to parse Rank Id";
                officer.RankId = 0;
            }
            else { officer.RankId = rankId; }

            int commandId = 0;
            parsed = Int32.TryParse(lineValues.ElementAt(5), out commandId);
            if (!parsed)
            {
                officer.Error = true;
                officer.ErrorMessage = "Unable to parse Command Id";
                officer.CommandId = 0;
            }
            else { officer.CommandId = commandId; }

            officer.AccountNumber = lineValues.ElementAt(6).Trim();

            officer.BankCode = lineValues.ElementAt(7).Trim();

            return officer;
        }

    }
}