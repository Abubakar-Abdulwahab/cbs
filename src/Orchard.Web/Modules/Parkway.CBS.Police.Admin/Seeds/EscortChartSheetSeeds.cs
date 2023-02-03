using Parkway.CBS.Police.Admin.Seeds.Contracts;
using ExcelDataReader;
using Parkway.CBS.Police.Core.Utilities;
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
using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Admin.Seeds
{
    public class EscortChartSheetSeeds : IEscortChartSheetSeeds
    {
        private const int templateHeaderCount = 5;
        protected readonly string[] templateHeaderNames = {
            "Rank Id".ToLower() , "Sector Sub Category".ToLower(), "State Id".ToLower(), "LGA Id".ToLower(),"Rate".ToLower() };

        private readonly IEscortAmountChartSheetManager<EscortAmountChartSheet> _escortRateSheetManager;
        private readonly IOrchardServices _orchardServices;
        private readonly ILGAManager<LGA> _lgaManager;
        private readonly IPoliceRankingManager<PoliceRanking> _policeRankingManager;
        private readonly ITaxEntitySubSubCategoryManager<TaxEntitySubSubCategory> _taxEntitySubSubCategoryManager;


        public EscortChartSheetSeeds(IEscortAmountChartSheetManager<EscortAmountChartSheet> escortRateSheetManager, IOrchardServices orchardServices, ILGAManager<LGA> lgaManager, IPoliceRankingManager<PoliceRanking> policeRankingManager, ITaxEntitySubSubCategoryManager<TaxEntitySubSubCategory> taxEntitySubSubCategoryManager)
        {
            _escortRateSheetManager = escortRateSheetManager;
            _orchardServices = orchardServices;
            _lgaManager = lgaManager;
            _policeRankingManager = policeRankingManager;
            _taxEntitySubSubCategoryManager = taxEntitySubSubCategoryManager;
        }

        public EscortChartSheetStatVM UploadChartSheet()
        {
            try
            {
                int countersuccessful = 0;
                int counterunsuccessful = 0;
                List<EscortChartSheetSeedingVM> unsuccessfulList = new List<EscortChartSheetSeedingVM>();
                List<EscortAmountChartSheet> chartSheetList = new List<EscortAmountChartSheet>();
                EscortChartSheetResponse response = ProcessFile();

                foreach (var chartItem in response.ChartSheetRecords)
                {
                    if (chartItem.Error)
                    {
                        counterunsuccessful++;
                        unsuccessfulList.Add(chartItem);
                    }
                    else
                    {
                        EscortAmountChartSheet chartSheetObj = new EscortAmountChartSheet
                        {
                            Rank = new PoliceRanking { Id = chartItem.RankId },
                            PSSEscortServiceCategory = new PSSEscortServiceCategory { Id = chartItem.PSSEscortServiceCategoryId },
                            State = new StateModel { Id = chartItem.StateId },
                            LGA = new LGA { Id = chartItem.LGAId },
                            IsActive = true,
                            AddedBy = new Orchard.Users.Models.UserPartRecord { Id = _orchardServices.WorkContext.CurrentUser.Id },
                            Rate = chartItem.RateAmount
                        };
                        chartSheetList.Add(chartSheetObj);
                        countersuccessful++;
                    }
                }
                bool saveResponse = _escortRateSheetManager.SaveBundle(chartSheetList);
                if (!saveResponse)
                {
                    throw new Exception("Error!!");
                }
                return new EscortChartSheetStatVM { NumberofSuccessfulRecords = response.ChartSheetRecords.Count - counterunsuccessful, NumberofUnSuccessfulRecords = counterunsuccessful };
            }
            catch (Exception ex)
            {
                _escortRateSheetManager.RollBackAllTransactions();
                throw ex;
            }
        }

        public EscortChartSheetResponse ProcessFile()
        {
            try
            {
                string filePath = PSSUtil.GetChartSheetExcelFilePath();
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
                EscortChartSheetResponse chartSheetData = new EscortChartSheetResponse();

                var invalidsHeaders = headers.Where(k => k.Value.HeaderPresent == false);
                if (invalidsHeaders.Count() > 0)
                {
                    var msg = invalidsHeaders.Select(x => x.Key.ToUpper() + " header not found").ToArray();
                    chartSheetData.HeaderValidateObject = new HeaderValidateObject { Error = true, ErrorMessage = string.Join(". ", msg) };
                }

                ConcurrentStack<EscortChartSheetSeedingVM> chartSheetRecords = new ConcurrentStack<EscortChartSheetSeedingVM> { };
                if (chartSheetData.HeaderValidateObject == null)
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

                        var officerLineRecord = GetChartSheetLineRecord(lineValues);
                        chartSheetRecords.Push(officerLineRecord);
                    });
                    chartSheetData.ChartSheetRecords = chartSheetRecords;
                }

                if (chartSheetData.HeaderValidateObject != null)
                {
                    return chartSheetData;
                }

                return chartSheetData;
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

        protected EscortChartSheetSeedingVM GetChartSheetLineRecord(List<string> lineValues)
        {
            var chartSheet = new EscortChartSheetSeedingVM();

            bool parsed = int.TryParse(lineValues.ElementAt(0), out int rankId);
            if (!parsed)
            {
                chartSheet.Error = true;
                chartSheet.ErrorMessage = "Unable to parse Rank Id";
                chartSheet.RankId = 0;
            }
            else { chartSheet.RankId = rankId; }

            parsed = int.TryParse(lineValues.ElementAt(1), out int pssEscortServiceCategoryId);
            if (!parsed)
            {
                chartSheet.Error = true;
                chartSheet.ErrorMessage = "Unable to parse Sector Id";
                chartSheet.PSSEscortServiceCategoryId = 0;
            }
            else { chartSheet.PSSEscortServiceCategoryId = pssEscortServiceCategoryId; }

            parsed = int.TryParse(lineValues.ElementAt(2), out int stateId);
            if (!parsed)
            {
                chartSheet.Error = true;
                chartSheet.ErrorMessage = "Unable to parse State Id";
                chartSheet.StateId = 0;
            }
            else { chartSheet.StateId = stateId; }

            parsed = int.TryParse(lineValues.ElementAt(3), out int lgaId);
            if (!parsed)
            {
                chartSheet.Error = true;
                chartSheet.ErrorMessage = "Unable to parse LGA Id";
                chartSheet.LGAId = 0;
            }
            else { chartSheet.LGAId = lgaId; }

            parsed = decimal.TryParse(lineValues.ElementAt(4), out decimal rateAmount);
            if (!parsed)
            {
                chartSheet.Error = true;
                chartSheet.ErrorMessage = "Unable to parse Rate Amount";
                chartSheet.RateAmount = 0;
            }
            else { chartSheet.RateAmount = rateAmount; }


            return chartSheet;
        }

        /// <summary>
        /// Process chart sheet
        /// </summary>
        /// <returns></returns>
        public bool ProcessChartSheet()
        {
            try
            {
                //Get LGAs, Ranks and TaxEntitySubSubCategories
                List<LGAVM> lgas = _lgaManager.GetLGAs();
                //Get ranks that are not equal or less than traffic warden rank level. 1 to 23 is for ranks level that are not traffic warden
                List<PoliceRankingVM> ranks = _policeRankingManager.GetPoliceRanks().Where(x => x.RankLevel < 24).ToList();
                List<PSSEscortServiceCategoryChartSheetVM> pssEscortServiceCategories = new List<PSSEscortServiceCategoryChartSheetVM>
                {
                    { new PSSEscortServiceCategoryChartSheetVM { PSSEscortServiceCategoryId = 19, CommandTypeId = 2, AmountRate = 4000, Name = "Orderly" } },
                    { new PSSEscortServiceCategoryChartSheetVM { PSSEscortServiceCategoryId = 20, CommandTypeId = 2, AmountRate = 4000, Name = "Intra-state Escort" } },
                    { new PSSEscortServiceCategoryChartSheetVM { PSSEscortServiceCategoryId = 21, CommandTypeId = 2, AmountRate = 4000, Name = "Inter-state Escort" } },
                    { new PSSEscortServiceCategoryChartSheetVM { PSSEscortServiceCategoryId = 22, CommandTypeId = 2, AmountRate = 4000, Name = "Bank" } },
                    { new PSSEscortServiceCategoryChartSheetVM { PSSEscortServiceCategoryId = 23, CommandTypeId = 2, AmountRate = 10000, Name = "Private Company" } },
                    { new PSSEscortServiceCategoryChartSheetVM { PSSEscortServiceCategoryId = 24, CommandTypeId = 2, AmountRate = 5000, Name = "Religious Body" } },
                    { new PSSEscortServiceCategoryChartSheetVM { PSSEscortServiceCategoryId = 26, CommandTypeId = 2, AmountRate = 8000, Name = "Individual & Private Residence" } },
                    { new PSSEscortServiceCategoryChartSheetVM { PSSEscortServiceCategoryId = 28, CommandTypeId = 2, AmountRate = 12000, Name = "Multinational Corporation" } },
                    { new PSSEscortServiceCategoryChartSheetVM { PSSEscortServiceCategoryId = 31, CommandTypeId = 2, AmountRate = 9000, Name = "Non-Governmental Organizations" } },
                    { new PSSEscortServiceCategoryChartSheetVM { PSSEscortServiceCategoryId = 33, CommandTypeId = 2, AmountRate = 12000, Name = "Oil Protection" } },
                    { new PSSEscortServiceCategoryChartSheetVM { PSSEscortServiceCategoryId = 35, CommandTypeId = 2, AmountRate = 20000, Name = "Policing Event" } },
                    { new PSSEscortServiceCategoryChartSheetVM { PSSEscortServiceCategoryId = 19, CommandTypeId = 3, AmountRate = 4000, Name = "Orderly" } },
                    { new PSSEscortServiceCategoryChartSheetVM { PSSEscortServiceCategoryId = 20, CommandTypeId = 3, AmountRate = 4000, Name = "Intra-state Escort" } },
                    { new PSSEscortServiceCategoryChartSheetVM { PSSEscortServiceCategoryId = 21, CommandTypeId = 3, AmountRate = 4000, Name = "Inter-state Escort" } },
                    { new PSSEscortServiceCategoryChartSheetVM { PSSEscortServiceCategoryId = 22, CommandTypeId = 3, AmountRate = 4000, Name = "Bank" } },
                    { new PSSEscortServiceCategoryChartSheetVM { PSSEscortServiceCategoryId = 23, CommandTypeId = 3, AmountRate = 10000, Name = "Private Company" } },
                    { new PSSEscortServiceCategoryChartSheetVM { PSSEscortServiceCategoryId = 24, CommandTypeId = 3, AmountRate = 5000, Name = "Religious Body" } },
                    { new PSSEscortServiceCategoryChartSheetVM { PSSEscortServiceCategoryId = 26, CommandTypeId = 3, AmountRate = 8000, Name = "Individual & Private Residence" } },
                    { new PSSEscortServiceCategoryChartSheetVM { PSSEscortServiceCategoryId = 28, CommandTypeId = 3, AmountRate = 12000, Name = "Multinational Corporation" } },
                    { new PSSEscortServiceCategoryChartSheetVM { PSSEscortServiceCategoryId = 31, CommandTypeId = 3, AmountRate = 9000, Name = "Non-Governmental Organizations" } },
                    { new PSSEscortServiceCategoryChartSheetVM { PSSEscortServiceCategoryId = 33, CommandTypeId = 3, AmountRate = 12000, Name = "Oil Protection" } },
                    { new PSSEscortServiceCategoryChartSheetVM { PSSEscortServiceCategoryId = 35, CommandTypeId = 3, AmountRate = 20000, Name = "Policing Event" } }
                  
                };

                List<EscortAmountChartSheet> chartSheetList = new List<EscortAmountChartSheet>();

                foreach (LGAVM lga in lgas)
                {
                    foreach (PSSEscortServiceCategoryChartSheetVM subCategory in pssEscortServiceCategories)
                    {
                        foreach (PoliceRankingVM rank in ranks)
                        {
                            EscortAmountChartSheet chartSheetObj = new EscortAmountChartSheet
                            {
                                Rank = new PoliceRanking { Id = rank.Id },
                                PSSEscortServiceCategory = new PSSEscortServiceCategory { Id = subCategory.PSSEscortServiceCategoryId },
                                State = new StateModel { Id = lga.StateId },
                                LGA = new LGA { Id = lga.Id },
                                IsActive = true,
                                AddedBy = new Orchard.Users.Models.UserPartRecord { Id = _orchardServices.WorkContext.CurrentUser.Id },
                                Rate = subCategory.AmountRate,
                                CommandType = new Core.Models.CommandType { Id = subCategory.CommandTypeId }
                            };
                            chartSheetList.Add(chartSheetObj);
                        }
                    }
                }

                bool saveResponse = _escortRateSheetManager.SaveBundle(chartSheetList);
                if (!saveResponse)
                {
                    throw new Exception("Error!!");
                }
                return true;
            }
            catch (Exception ex)
            {
                _escortRateSheetManager.RollBackAllTransactions();
                throw ex;
            }
        }
    }
}