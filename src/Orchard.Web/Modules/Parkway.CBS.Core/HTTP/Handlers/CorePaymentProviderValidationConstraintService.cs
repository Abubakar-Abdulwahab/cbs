using System;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Orchard.Users.Models;
using System.Collections.Generic;
using System.Data;
using Newtonsoft.Json;
using Orchard.Logging;
using System.Linq;

namespace Parkway.CBS.Core.HTTP.Handlers
{
    public class CorePaymentProviderValidationConstraintService : ICorePaymentProviderValidationConstraintService
    {
        private readonly IExternalPaymentProviderManager<ExternalPaymentProvider> _repo;
        private readonly ICoreSettingsService _settingsService;
        private readonly IRevenueHeadManager<RevenueHead> _revHeadRepo;
        private readonly IPaymentProviderValidationConstraintManager<PaymentProviderValidationConstraint> _providerValidationConsRepo;
        private readonly IMDARevenueHeadEntryStagingManager<MDARevenueHeadEntryStaging> _mdaRevenueHeadEntryStagingRepo;
        private readonly IMDAManager<MDA> _mdaRepo;
        public ILogger Logger { get; set; }


        public CorePaymentProviderValidationConstraintService(IExternalPaymentProviderManager<ExternalPaymentProvider> repo, ICoreSettingsService settingsService, IRevenueHeadManager<RevenueHead> revHeadRepo, IPaymentProviderValidationConstraintManager<PaymentProviderValidationConstraint> providerValidationConsRepo, IMDARevenueHeadEntryStagingManager<MDARevenueHeadEntryStaging> mdaRevenueHeadEntryStagingRepo, IMDAManager<MDA> mdaRepo)
        {
            _repo = repo;
            _settingsService = settingsService;
            _revHeadRepo = revHeadRepo;
            _providerValidationConsRepo = providerValidationConsRepo;
            _mdaRevenueHeadEntryStagingRepo = mdaRevenueHeadEntryStagingRepo;
            _mdaRepo = mdaRepo;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Assign payment provider to selected revenue heads
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="admin"></param>
        public void TryAssignPaymentProviderToRevenueHeads(AssignExternalPaymentProviderVM userInput, UserPartRecord admin)
        {
            try
            {
                var rhAndMdaCollection = JsonConvert.DeserializeObject<Dictionary<int, IEnumerable<int>>>(userInput.SelectedRhAndMdas);

                var dataTable = new DataTable("Parkway_CBS_Core_" + typeof(PaymentProviderValidationConstraint).Name);
                dataTable.Columns.Add(new DataColumn("MDA_Id", typeof(int)));
                dataTable.Columns.Add(new DataColumn("RevenueHead_Id", typeof(int)));
                dataTable.Columns.Add(new DataColumn("IsDeleted", typeof(bool)));
                dataTable.Columns.Add(new DataColumn("PaymentProvider_Id", typeof(int)));
                dataTable.Columns.Add(new DataColumn("LastUpdatedBy_Id", typeof(int)));
                dataTable.Columns.Add(new DataColumn("CreatedAtUtc", typeof(DateTime)));
                dataTable.Columns.Add(new DataColumn("UpdatedAtUtc", typeof(DateTime)));

                foreach (var mda in rhAndMdaCollection)
                {
                    foreach (var rh in mda.Value)
                    {
                        var row = dataTable.NewRow();
                        row["MDA_Id"] = mda.Key;
                        row["IsDeleted"] = false;
                        row["PaymentProvider_Id"] = userInput.SelectedPaymentProviderParsed;
                        row["LastUpdatedBy_Id"] = admin.Id;
                        row["CreatedAtUtc"] = DateTime.Now.ToLocalTime();
                        row["UpdatedAtUtc"] = DateTime.Now.ToLocalTime();
                        if (rh != 0)
                        {
                            if (!_revHeadRepo.CheckIfRevenueHeadAndExistsWithMDA(rh, mda.Key)) { throw new NoRecordFoundException(string.Format("No match for rev {0} and MDA {1} ", rh, mda.Key)); }
                            row["RevenueHead_Id"] = rh;
                        }
                        else
                        {
                            if (!_mdaRepo.CheckIfMDAExists(mda.Key)) { throw new NoRecordFoundException(string.Format("No match for MDA {0} ", mda.Key)); }
                            row["RevenueHead_Id"] = DBNull.Value;
                        }

                        dataTable.Rows.Add(row);
                    }
                }

                if (_repo.SaveBundle(dataTable, "Parkway_CBS_Core_" + typeof(PaymentProviderValidationConstraint).Name))
                {
                    //Update MDA table
                    _mdaRepo.UpdateMDAPaymentProviderValidationConstraintsStatus();
                }
                else { throw new Exception($"Unable to assign validation constraints to payment provider with Id {userInput.SelectedPaymentProviderParsed}"); }
            }
            catch (Exception)
            {
                _mdaRepo.RollBackAllTransactions();
                throw;
            }
        }


        /// <summary>
        /// This method fetches existing access restrictions using the specified 
        /// for the payment provider Id.
        /// </summary>
        /// <param name="providerId"></param>
        /// <returns>IEnumerable{PaymentProviderValidationConstraintsVM}</returns>
        /// <exception cref="Exception"></exception>
        public IEnumerable<PaymentProviderValidationConstraintsVM> GetExistingRestrictions(int providerId)
        {
            return _providerValidationConsRepo.GetExistingConstraints(providerId);
        }


        /// <summary>
        /// Synchronize constraints records in payment provider validation constraints table with MDA revenue access restrictions staging table.
        /// </summary>
        /// <param name="providerId"></param>
        /// <param name="mdaRevenueHeadEntryStagingReference"></param>
        public void UpdatePaymentProviderConstraints(int providerId, string mdaRevenueHeadEntryStagingReference)
        {
            try
            {
                Int64 MDARevenueHeadEntryStagingId = _mdaRevenueHeadEntryStagingRepo.GetReferenceId(mdaRevenueHeadEntryStagingReference, nameof(PaymentProviderValidationConstraint).GetHashCode(), providerId);
                if(MDARevenueHeadEntryStagingId == 0) { throw new NoRecordFoundException("No reference found for MDARevenueHeadEntryStaging " + mdaRevenueHeadEntryStagingReference); }
                _providerValidationConsRepo.UpdateProviderRecords(providerId, MDARevenueHeadEntryStagingId);
                _mdaRepo.UpdateMDAPaymentProviderValidationConstraintsStatus();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _providerValidationConsRepo.RollBackAllTransactions();
                throw;
            }
        }


        /// <summary>
        /// Get the billable revenue heads
        /// </summary>
        /// <returns>List{MDAVM}</returns>
        public List<MDAVM> GetMDAsForBillableRevenueHeads()
        {
            return _revHeadRepo.GetMDAsForBillableRevenueHeads().ToList();
        }


        /// <summary>
        /// Get payment provider 
        /// <para>ToFuture</para>
        /// </summary>
        /// <param name="providerId"></param>
        /// <returns>IEnumerable{PaymentProviderVM}</returns>
        public IEnumerable<PaymentProviderVM> GetProvider(int providerId)
        {
            return _repo.GetProvider(providerId);
        }


        public IEnumerable<RevenueHeadLite> GetRevenueHeadsPerMdaOnAccessList(int mdaId, int userId, bool applyRestrictions)
        {
            return _revHeadRepo.GetRevenueHeadsPerMdaOnAccessList(mdaId, userId, applyRestrictions);
        }


        /// <summary>
        /// Check if payment provider exists
        /// </summary>
        /// <param name="providerIdPased"></param>
        /// <returns>bool</returns>
        public bool CheckIfPaymentProviderExists(int providerIdPased)
        {
            return _repo.Count(x => x.Id == providerIdPased) == 1;
        }

    }
}