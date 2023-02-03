using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.DataFilters.FeePartyReport.Contracts;
using Parkway.CBS.Police.Core.DataFilters.SettlementFeePartiesReport.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers
{
    public class SettlementFeePartiesHandler : ISettlementFeePartiesHandler
    {
        private readonly IHandlerComposition _handlerComposition;
        private readonly IPSSFeePartyAdapterConfigurationManager<PSSFeePartyAdapterConfiguration> _feePartyAdapterConfigurationManager;
        private readonly Lazy<IPSSSettlementFeePartyManager<PSSSettlementFeeParty>> _pssSettlementFeePartyManager;
        private readonly Lazy<IPSSSettlementFeePartyStagingManager<PSSSettlementFeePartyStaging>> _pssSettlementFeePartyStagingManager;
        private readonly IPSSFeePartyManager<PSSFeeParty> _feePartyManager;
        private readonly IBankManager<Bank> _bankManager;
        private readonly Lazy<IFeePartyReportFilter> _settlementFeePartyReportFilter;
        private readonly IOrchardServices _orchardServices;
        private readonly Lazy<ISettlementFeePartiesReportFilter> _settlementFeePartiesReportFilter;
        private readonly Lazy<IPSSSettlementManager<PSSSettlement>> _pssSettlementManager;

        public ILogger Logger { get; set; }

        public SettlementFeePartiesHandler(IHandlerComposition handlerComposition, Lazy<IFeePartyReportFilter> settlementFeePartyReportFilter, IPSSFeePartyAdapterConfigurationManager<PSSFeePartyAdapterConfiguration> feePartyAdapterConfigurationManager, IPSSFeePartyManager<PSSFeeParty> feePartyManager, IOrchardServices orchardServices, IBankManager<Bank> bankManager, Lazy<ISettlementFeePartiesReportFilter> settlementFeePartiesReportFilter, Lazy<IPSSSettlementFeePartyManager<PSSSettlementFeeParty>> pssSettlementFeePartyManager, Lazy<IPSSSettlementFeePartyStagingManager<PSSSettlementFeePartyStaging>> pssSettlementFeePartyStagingManager, Lazy<IPSSSettlementManager<PSSSettlement>> pssSettlementManager)
        {
            _handlerComposition = handlerComposition;
            Logger = NullLogger.Instance;
            _settlementFeePartyReportFilter = settlementFeePartyReportFilter;
            _feePartyAdapterConfigurationManager = feePartyAdapterConfigurationManager;
            _feePartyManager = feePartyManager;
            _orchardServices = orchardServices;
            _bankManager = bankManager;
            _settlementFeePartiesReportFilter = settlementFeePartiesReportFilter;
            _pssSettlementFeePartyManager = pssSettlementFeePartyManager;
            _pssSettlementFeePartyStagingManager = pssSettlementFeePartyStagingManager;
            _pssSettlementManager = pssSettlementManager;
        }


        /// <summary>
        /// Validates and Creates settlement fee part in <see cref="SettlementFeePartyConfiguration"/>
        /// </summary>
        /// <param name="errors"> Validation errors</param>
        /// <param name="model">User input model</param>
        public void AddFeeParty(ref List<ErrorModel> errors, AddSettlementFeePartyVM model)
        {

            try
            {
                ValidateUserInput(errors, model);

                PSSFeeParty feeParty = new PSSFeeParty
                {
                    AccountNumber = model.AccountNumber,
                    IsActive = true,
                    Name = model.Name,
                    LastUpdatedBy = new Orchard.Users.Models.UserPartRecord
                    {
                        Id = _orchardServices.WorkContext.CurrentUser.Id
                    }
                };
                if (!model.AllowAdditionalCommandSplit)
                {
                    BankViewModel bankVM = _bankManager.GetActiveBankByBankCode(model.SelectedBankCode);
                    feeParty.Bank = new Bank { Id = bankVM.Id };
                }

                if (!_feePartyManager.Save(feeParty))
                {
                    throw new CouldNotSaveRecord();
                };
            }
            catch (Exception exception)
            {

                _feePartyManager.RollBackAllTransactions();
                Logger.Error(exception, exception.Message);
                throw;
            }

        }

        /// <summary>
        /// Add/Edit settlement fee party
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="model"></param>
        public void EditSettlementFeeParty(ref List<ErrorModel> errors, PSSSettlementFeePartiesVM model)
        {
            if (model.SelectedSettlementFeeParties != null && model.SelectedSettlementFeeParties.Sum(x => x.DeductionValue) != 100)
            {
                errors.Add(new ErrorModel { ErrorMessage = $"Total percentage must be 100", FieldName = nameof(model.SelectedSettlementFeeParties) });
                throw new DirtyFormDataException();
            }

            try
            {
                DataTable dataTable = new DataTable("Parkway_CBS_Police_Core" + typeof(PSSSettlementFeePartyStaging).Name);

                dataTable.Columns.Add(new DataColumn(nameof(PSSSettlementFeePartyStaging.DeductionTypeId), typeof(int)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSSettlementFeePartyStaging.Settlement) + "_Id", typeof(long)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSSettlementFeePartyStaging.DeductionValue), typeof(decimal)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSSettlementFeePartyStaging.FeeParty) + "_Id", typeof(long)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSSettlementFeePartyStaging.IsActive), typeof(bool)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSSettlementFeePartyStaging.IsDeleted), typeof(bool)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSSettlementFeePartyStaging.HasAdditionalSplits), typeof(bool)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSSettlementFeePartyStaging.AdditionalSplitValue), typeof(string))); 
                dataTable.Columns.Add(new DataColumn(nameof(PSSSettlementFeePartyStaging.MaxPercentage), typeof(bool)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSSettlementFeePartyStaging.SN), typeof(int)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSSettlementFeePartyStaging.Position), typeof(int)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSSettlementFeePartyStaging.Reference), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSSettlementFeePartyStaging.CreatedAtUtc), typeof(DateTime)));
                dataTable.Columns.Add(new DataColumn(nameof(PSSSettlementFeePartyStaging.UpdatedAtUtc), typeof(DateTime)));

                decimal percentage = 0;
                bool isMaxPercentage = false;
                string reference = string.Format("SFP-{0}-SFPID-{1}", DateTime.Now.Ticks, Util.StrongRandom());
                
                int sn = 1;
                int position = 0;

                if (model.AddedSettlementFeeParties != null && model.AddedSettlementFeeParties.Any())
                {
                    foreach (var selectedFeeParty in model.AddedSettlementFeeParties.OrderByDescending(x => x.DeductionValue).Select((value, i) => new { i, value }))
                    {
                        if (!_feePartyManager.CheckIfFeePartyExistById(selectedFeeParty.value.FeePartyId))
                        {
                            throw new NoRecordFoundException($"No fee party with Id {selectedFeeParty.value.FeePartyId} found");
                        }
                        if (selectedFeeParty.value.DeductionValue > percentage)
                        {
                            percentage = selectedFeeParty.value.DeductionValue;
                            isMaxPercentage = true;
                        }

                        DataRow row = dataTable.NewRow();

                        if (selectedFeeParty.value.HasAdditionalSplits)
                        {
                            row[nameof(PSSSettlementFeePartyStaging.AdditionalSplitValue)] = _feePartyAdapterConfigurationManager.GetSettlementAdapterConfigurationsByAdapterId(selectedFeeParty.value.AdapterId).Name;
                        }
                        else
                        {
                            row[nameof(PSSSettlementFeePartyStaging.AdditionalSplitValue)] = DBNull.Value;
                        }

                        row[nameof(PSSSettlementFeePartyStaging.DeductionTypeId)] = (int)DeductionShareType.Percentage;
                        row[nameof(PSSSettlementFeePartyStaging.Settlement) + "_Id"] = model.SettlementId;
                        row[nameof(PSSSettlementFeePartyStaging.FeeParty) + "_Id"] = selectedFeeParty.value.FeePartyId;
                        row[nameof(PSSSettlementFeePartyStaging.IsActive)] = true;
                        row[nameof(PSSSettlementFeePartyStaging.DeductionValue)] = selectedFeeParty.value.DeductionValue;
                        row[nameof(PSSSettlementFeePartyStaging.IsDeleted)] = false;
                        row[nameof(PSSSettlementFeePartyStaging.SN)] = sn;
                        row[nameof(PSSSettlementFeePartyStaging.HasAdditionalSplits)] = selectedFeeParty.value.HasAdditionalSplits;
                        row[nameof(PSSSettlementFeePartyStaging.MaxPercentage)] = isMaxPercentage;
                        row[nameof(PSSSettlementFeePartyStaging.Position)] = selectedFeeParty.i;
                        row[nameof(PSSSettlementFeePartyStaging.Reference)] = reference;
                        row[nameof(PSSSettlementFeePartyStaging.CreatedAtUtc)] = DateTime.Now.ToLocalTime();
                        row[nameof(PSSSettlementFeePartyStaging.UpdatedAtUtc)] = DateTime.Now.ToLocalTime();
                        dataTable.Rows.Add(row);
                        isMaxPercentage = false;
                        sn++;
                        position = selectedFeeParty.i;
                    }
                }

                if (model.RemovedSettlementFeeParties != null && model.RemovedSettlementFeeParties.Any())
                {
                    foreach (var removedFeeParty in model.RemovedSettlementFeeParties.OrderByDescending(x => x.DeductionValue).Select((value, i) => new { i, value }))
                    {
                        if (!_feePartyManager.CheckIfFeePartyExistById(removedFeeParty.value.FeePartyId))
                        {
                            throw new NoRecordFoundException($"No fee party with Id {removedFeeParty.value.FeePartyId} found");
                        }

                        if (removedFeeParty.value.DeductionValue > percentage)
                        {
                            percentage = removedFeeParty.value.DeductionValue;
                            isMaxPercentage = true;
                        }

                        DataRow row = dataTable.NewRow();

                        if (removedFeeParty.value.HasAdditionalSplits)
                        {
                            row[nameof(PSSSettlementFeePartyStaging.AdditionalSplitValue)] = _feePartyAdapterConfigurationManager.GetSettlementAdapterConfigurationsByAdapterId(removedFeeParty.value.AdapterId).Name;
                        }
                        else
                        {
                            row[nameof(PSSSettlementFeePartyStaging.AdditionalSplitValue)] = DBNull.Value;
                        }
                       
                        row[nameof(PSSSettlementFeePartyStaging.DeductionTypeId)] = (int)DeductionShareType.Percentage;
                        row[nameof(PSSSettlementFeePartyStaging.Settlement) + "_Id"] = model.SettlementId;
                        row[nameof(PSSSettlementFeePartyStaging.FeeParty) + "_Id"] = removedFeeParty.value.FeePartyId;
                        row[nameof(PSSSettlementFeePartyStaging.IsActive)] = false;
                        row[nameof(PSSSettlementFeePartyStaging.DeductionValue)] = removedFeeParty.value.DeductionValue;
                        row[nameof(PSSSettlementFeePartyStaging.IsDeleted)] = true;
                        row[nameof(PSSSettlementFeePartyStaging.SN)] = sn;
                        row[nameof(PSSSettlementFeePartyStaging.HasAdditionalSplits)] = removedFeeParty.value.HasAdditionalSplits;
                        row[nameof(PSSSettlementFeePartyStaging.MaxPercentage)] = isMaxPercentage;
                        row[nameof(PSSSettlementFeePartyStaging.Position)] = position;
                        row[nameof(PSSSettlementFeePartyStaging.Reference)] = reference;
                        row[nameof(PSSSettlementFeePartyStaging.CreatedAtUtc)] = DateTime.Now.ToLocalTime();
                        row[nameof(PSSSettlementFeePartyStaging.UpdatedAtUtc)] = DateTime.Now.ToLocalTime();
                        dataTable.Rows.Add(row);
                        isMaxPercentage = false;
                        sn++;
                        position++;
                    }
                }

                if (dataTable?.Rows?.Count > 0)
                {
                    if (!_pssSettlementFeePartyStagingManager.Value.SaveBundle(dataTable, "Parkway_CBS_Police_Core_" + typeof(PSSSettlementFeePartyStaging).Name))
                    {
                        throw new CouldNotSaveRecord();
                    }
               
                    _pssSettlementFeePartyStagingManager.Value.UpdateSettlementFeePartyFromStaging(reference);

                    // Set all max percentage to false
                    _pssSettlementFeePartyManager.Value.SetMaxPercentageToFalse(model.SettlementId);

                    //Set the actual max percentage
                    _pssSettlementFeePartyManager.Value.SetMaxPercentage(model.SettlementId);

                    _pssSettlementManager.Value.UpdateSettlementBatchUpdatedAtDate(model.SettlementId);

                }
            }
            catch (Exception exception)
            {

                _feePartyManager.RollBackAllTransactions();
                Logger.Error(exception, exception.Message);
                throw;
            }

        }

        /// <summary>
        /// Validates user inputs
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="model"></param>
        /// <exception cref="DirtyFormDataException"></exception>
        private void ValidateUserInput(List<ErrorModel> errors, AddSettlementFeePartyVM model)
        {
            if (string.IsNullOrEmpty(model.Name?.Trim()))
            {
                errors.Add(new ErrorModel { ErrorMessage = $"Kindly enter a name", FieldName = nameof(model.Name) });
                throw new DirtyFormDataException();
            }

            if (!model.AllowAdditionalCommandSplit && string.IsNullOrEmpty(model.AccountNumber?.Trim()))
            {
                errors.Add(new ErrorModel { ErrorMessage = $"Kindly enter an account number", FieldName = nameof(model.AccountNumber) });
                throw new DirtyFormDataException();
            }
            if (!model.AllowAdditionalCommandSplit && string.IsNullOrEmpty(model.SelectedBankCode?.Trim()))
            {
                errors.Add(new ErrorModel { ErrorMessage = $"Kindly select a bank", FieldName = nameof(model.SelectedBankCode) });
                throw new DirtyFormDataException();
            }
        }

        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="permission"></param>
        public void CheckForPermission(Orchard.Security.Permissions.Permission permission)
        {
            _handlerComposition.IsAuthorized(permission);
        }

        /// <summary>
        /// Gets view model for settlement fee parties report
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        public PSSSettlementFeePartiesVM GetVMForReports(SettlementFeePartiesSearchParams searchParams)
        {
            dynamic recordsAndAggregate = _settlementFeePartiesReportFilter.Value.GetSettlementReportViewModel(searchParams);
            IEnumerable<PSSSettlementFeePartyVM> reports = (IEnumerable<PSSSettlementFeePartyVM>)recordsAndAggregate.ReportRecords;

            return new PSSSettlementFeePartiesVM
            {
                SettlementName = _pssSettlementManager.Value.GetSettlementById(searchParams.SettlementId).Name,
                SelectedSettlementFeeParties = reports.ToList(),
                TotalNumberOfActiveSettlementFeeParties = (int)((IEnumerable<ReportStatsVM>)recordsAndAggregate.TotalNumberOfActiveSettlementFeeParties).First().TotalRecordCount,
            };
        }

        public PSSSettlementFeePartiesVM GetVMForEditParties(SettlementFeePartiesSearchParams searchParams)
        {
            dynamic recordsAndAggregate = _settlementFeePartiesReportFilter.Value.GetSettlementReportViewModel(searchParams);
            IEnumerable<PSSSettlementFeePartyVM> reports = (IEnumerable<PSSSettlementFeePartyVM>)recordsAndAggregate.ReportRecords;
            List<PSSFeePartyVM> feePartiesVM = _feePartyManager.GetAllActiveFeeParties();
            List<FeePartyAdapterConfigurationVM> feePartyAdapters = _feePartyAdapterConfigurationManager.GetActiveSettlementAdapterConfigurations();
            return new PSSSettlementFeePartiesVM
            {
                SettlementName = _pssSettlementManager.Value.GetSettlementById(searchParams.SettlementId).Name,
                SettlementId = searchParams.SettlementId,
                FeeParties = feePartiesVM.Select(x => new SettlementFeePartyVM
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList(),
                FeePartyAdapters = feePartyAdapters.Select(x => new Core.DTO.PSSFeePartyAdapterConfigurationDTO
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList(),
                SelectedSettlementFeeParties = reports.ToList(),
                TotalNumberOfActiveSettlementFeeParties = (int)((IEnumerable<ReportStatsVM>)recordsAndAggregate.TotalNumberOfActiveSettlementFeeParties).First().TotalRecordCount,
            };
        }

        /// <summary>
        /// Populates <see cref="AddSettlementFeePartyVM"/> for view
        /// </summary>
        /// <returns></returns>
        public AddSettlementFeePartyVM GetAddSettlementFeePartyVM()
        {
            return new AddSettlementFeePartyVM
            {
                Banks = _bankManager.GetAllActiveBanks(),
                FeePartyAdapterConfigurations = _feePartyAdapterConfigurationManager.GetActiveSettlementAdapterConfigurations()
            };
        }

        /// <summary>
        /// Get Reports VM for Settlement fee parties report
        /// </summary>
        /// <param name="feePartyReportSearchParams"></param>
        /// <returns></returns>
        public SettlementFeePartiesVM GetSettlementFeePartiesReportVM(FeePartyReportSearchParams feePartyReportSearchParams)
        {
            dynamic recordsAndAggregate = _settlementFeePartyReportFilter.Value.GetFeePartyReportViewModel(feePartyReportSearchParams);
            IEnumerable<SettlementFeePartyVM> reports = (IEnumerable<SettlementFeePartyVM>)recordsAndAggregate.ReportRecords;

            return new SettlementFeePartiesVM
            {
                FeeParties = reports.ToList(),
                TotalNumberOfFeePartyConfiguration = (int)((IEnumerable<ReportStatsVM>)recordsAndAggregate.TotalNumberOfSettlementFeePartyConfiguration).First().TotalRecordCount,
            };
        }

    }
}