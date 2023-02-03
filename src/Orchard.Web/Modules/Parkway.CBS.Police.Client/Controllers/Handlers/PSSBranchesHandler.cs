using Newtonsoft.Json;
using Orchard.Logging;
using Parkway.CBS.Core.DataFilters.TaxEntityProfileLocationReport.Contracts;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Parkway.CBS.Police.Client.Controllers.Handlers
{
    public class PSSBranchesHandler : IPSSBranchesHandler
    {
        private readonly ICoreTaxEntityProfileLocationService _coreTaxEntityProfileLocationService;
        private readonly ICoreStateAndLGA _coreStateAndLGAService;
        private readonly ITaxEntityProfileLocationFilter _taxEntityProfileLocationFilter;
        ILogger Logger { get; set; }
        public PSSBranchesHandler(ICoreTaxEntityProfileLocationService coreTaxEntityProfileLocationService, ICoreStateAndLGA coreStateAndLGAService, ITaxEntityProfileLocationFilter taxEntityProfileLocationFiler)
        {
            _coreTaxEntityProfileLocationService = coreTaxEntityProfileLocationService;
            _coreStateAndLGAService = coreStateAndLGAService;
            _taxEntityProfileLocationFilter = taxEntityProfileLocationFiler;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Gets PSSBranchVM
        /// </summary>
        /// <returns></returns>
        public PSSBranchVM GetCreateBranchVM()
        {
            try
            {
                return new PSSBranchVM
                {
                    BranchInfo = new TaxEntityProfileLocationVM { },
                    StateLGAs = _coreStateAndLGAService.GetStateVMs(),
                };
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Creates a new branch for currently logged in tax entity
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="errors"></param>
        public void CreateNewBranch(TaxEntityProfileLocationVM userInput, ref List<ErrorModel> errors)
        {
            try
            {

                if (userInput.State <= 0) { errors.Add(new ErrorModel { FieldName = "BranchInfo.State", ErrorMessage = "Please select a valid state" }); }
                if (userInput.LGA <= 0) { errors.Add(new ErrorModel { FieldName = "BranchInfo.LGA", ErrorMessage = "Please select a valid LGA" }); }
                if (string.IsNullOrEmpty(userInput.Name)) { errors.Add(new ErrorModel { FieldName = "BranchInfo.Name", ErrorMessage = "Please enter a valid name" }); }
                if (string.IsNullOrEmpty(userInput.Address)) { errors.Add(new ErrorModel { FieldName = "BranchInfo.Address", ErrorMessage = "Please enter a valid address" }); }

                if (errors.Count() > 0) { throw new DirtyFormDataException(); }

                if(userInput.Name.Trim().Length < 3 || userInput.Name.Trim().Length > 150)
                {
                    errors.Add(new ErrorModel { FieldName = "BranchInfo.Name", ErrorMessage = "The name field value should be between 3 and 150 characters." });
                }

                if(_coreTaxEntityProfileLocationService.CheckIfBranchWithNameExists(userInput.Name, userInput.TaxEntityId))
                {
                    errors.Add(new ErrorModel { FieldName = "BranchInfo.Name", ErrorMessage = "A branch with the specified name already exists." });
                }

                if(userInput.Address.Trim().Length < 10 || userInput.Address.Trim().Length > 300)
                {
                    errors.Add(new ErrorModel { FieldName = "BranchInfo.Address", ErrorMessage = "The address field value should be between 10 and 300 characters." });
                }

                if (_coreTaxEntityProfileLocationService.CheckIfBranchWithAddressExists(userInput.Address, userInput.TaxEntityId)) {
                    errors.Add(new ErrorModel { FieldName = "BranchInfo.Address", ErrorMessage = "A branch with the specified address already exists." });
                }

                if (_coreStateAndLGAService.GetStateIdForLGA(userInput.LGA, userInput.State) < 1) { 
                    errors.Add(new ErrorModel { FieldName = "BranchInfo.State", ErrorMessage = "Selected state is not valid" });
                    errors.Add(new ErrorModel { FieldName = "BranchInfo.LGA", ErrorMessage = "Selected LGA is not valid" });
                }

                if (errors.Count() > 0) { throw new DirtyFormDataException(); }

                _coreTaxEntityProfileLocationService.CreateBranch(userInput);
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Gets branch locations for currently logged in tax entity
        /// </summary>
        /// <param name="searchParams">search params</param>
        /// <returns>PSSBranchVM</returns>
        public PSSBranchVM GetBranches(TaxEntityProfileLocationReportSearchParams searchParams)
        {
            dynamic recordsAndAggregate = _taxEntityProfileLocationFilter.GetReportViewModel(searchParams);
            IEnumerable<TaxEntityProfileLocationVM> records = ((IEnumerable<TaxEntityProfileLocationVM>)recordsAndAggregate.ReportRecords);

            return new PSSBranchVM
            {
                //DateFilter = string.Format("{0} - {1}", searchParams.StartDate.ToString("dd'/'MM'/'yyyy"), searchParams.EndDate.ToString("dd'/'MM'/'yyyy")),
                FilteredAddress = searchParams.Address,
                FilteredName = searchParams.Name,
                FilteredState = searchParams.State,
                FilteredLGA = searchParams.LGA,
                Branches = (records == null || !records.Any()) ? new List<TaxEntityProfileLocationVM> { } : records.ToList(),
                TotalRecordCount = (int)(((IEnumerable<ReportStatsVM>)recordsAndAggregate.Aggregate).First().TotalRecordCount),
                Token = Util.LetsEncrypt(JsonConvert.SerializeObject(new { Address = searchParams.Address, StartDate = searchParams.StartDate, EndDate = searchParams.EndDate, Name = searchParams.Name, State = searchParams.State, LGA = searchParams.LGA, ChunkSize = 10, OperatorId = searchParams.TaxEntityId }), System.Configuration.ConfigurationManager.AppSettings["EncryptionSecret"]),

            };
        }


        /// <summary>
        /// Gets Paged PSS Branches
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="page">page</param>
        /// <param name="taxEntityId">tax entity id</param>
        /// <returns>APIResponse</returns>
        public APIResponse GetPagedBranchesData(string token, int? page, long taxEntityId)
        {
            try
            {
                dynamic tokenModel = new ExpandoObject();
                if (!string.IsNullOrEmpty(token))
                {
                    var decryptedValue = Util.LetsDecrypt(token);
                    tokenModel = JsonConvert.DeserializeObject(decryptedValue);
                }
                else { throw new Exception("Empty Token provided in GetPagedBranchesData for PSSBranchesHandler"); }
                //DateTime startDate = DateTime.Now.AddMonths(-3);
                //DateTime endDate = DateTime.Now;

                //if (tokenModel.StartDate != null && tokenModel.EndDate != null)
                //{
                //    startDate = tokenModel.StartDate;
                //    endDate = tokenModel.EndDate;
                //}

                ////get the date up until the final sec
                //endDate = endDate.Date.AddDays(1).AddMilliseconds(-1);

                int skip = page.HasValue ? (page.Value - 1) * ((int)tokenModel.ChunkSize) : 0;

                TaxEntityProfileLocationReportSearchParams searchParams = new TaxEntityProfileLocationReportSearchParams
                {
                    TaxEntityId = taxEntityId,
                    Take = tokenModel.ChunkSize,
                    Skip = skip,
                    //EndDate = endDate,
                    //StartDate = startDate,
                    Address = tokenModel.Address,
                    Name = tokenModel.Name,
                    State = tokenModel.State,
                    LGA = tokenModel.LGA
                };

                PSSBranchVM model = GetBranches(searchParams);

                return new APIResponse { ResponseObject = model };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return new APIResponse { Error = true, ResponseObject = new { Message = ErrorLang.genericexception().ToString() } };
            }
        }
    }
}