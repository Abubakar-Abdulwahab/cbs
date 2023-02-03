using Orchard;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HTTP.Handlers
{
    public class CoreMDARevenueAccessRestrictionsStagingService : ICoreMDARevenueAccessRestrictionsStagingService
    {
        private readonly IMDARevenueAccessRestrictionsStagingManager<MDARevenueAccessRestrictionsStaging> _accessRestrictionsStagingRepo;
        private readonly IMDARevenueHeadEntryStagingManager<MDARevenueHeadEntryStaging> _entryStagingRepo;
        private readonly IRevenueHeadManager<RevenueHead> _revHeadRepo;
        private readonly IMDAManager<MDA> _mdaRepo;
        private readonly IOrchardServices _orchardServices;
        private readonly IExternalPaymentProviderManager<ExternalPaymentProvider> _paymentProviderRepo;

        public CoreMDARevenueAccessRestrictionsStagingService(IMDARevenueAccessRestrictionsStagingManager<MDARevenueAccessRestrictionsStaging> accessRestrictionsStagingRepo, IMDARevenueHeadEntryStagingManager<MDARevenueHeadEntryStaging> entryStagingRepo, IOrchardServices orchardServices, IRevenueHeadManager<RevenueHead> revHeadRepo, IExternalPaymentProviderManager<ExternalPaymentProvider> paymentProviderRepo, IMDAManager<MDA> mdaRepo)
        {
            _accessRestrictionsStagingRepo = accessRestrictionsStagingRepo;
            _entryStagingRepo = entryStagingRepo;
            _orchardServices = orchardServices;
            _revHeadRepo = revHeadRepo;
            _paymentProviderRepo = paymentProviderRepo;
            _mdaRepo = mdaRepo;
        }

        /// <summary>
        /// Validate and save MDARevenueAccessRestrictions MDAs & Revenue Heads
        /// </summary>
        /// <param name="additions"></param>
        /// <param name="removals"></param>
        /// <param name="providerId"></param>
        /// <param name="userId"></param>
        /// <param name="operationType"></param>
        /// <returns>MDARevenueHeadEntryStaging Reference</returns>
        public string ValidateAndSaveStagingData(Dictionary<int, IEnumerable<int>> additions, Dictionary<int, IEnumerable<int>> removals, int providerId, int userId, string implementingOperationType)
        {
            try
            {
                //Number of ticks + Admin Id + Payment Provider Id + Operation type integer value - Reference Format
                var reference = string.Format("ARS-{0}-A-{1}-PP-{2}-OT-{3}", DateTime.Now.Ticks, userId, providerId, implementingOperationType.GetHashCode());

                MDARevenueHeadEntryStaging referenceStaging = new MDARevenueHeadEntryStaging
                {
                    ReferenceNumber = reference,
                    OperationType = implementingOperationType.GetHashCode(),
                    OperationTypeIdentifierId = providerId
                };

                if (!_entryStagingRepo.Save(referenceStaging)) { throw new Exception("Unable to save to MDARevenueHeadEntryStaging"); }

                var dataTable = new DataTable("Parkway_CBS_Core_" + typeof(MDARevenueAccessRestrictionsStaging).Name);
                dataTable.Columns.Add(new DataColumn("MDA_Id", typeof(int)));
                dataTable.Columns.Add(new DataColumn("RevenueHead_Id", typeof(int)));
                dataTable.Columns.Add(new DataColumn("IsRemoval", typeof(bool)));
                dataTable.Columns.Add(new DataColumn("MDARevenueHeadEntryStaging_Id", typeof(Int64)));
                dataTable.Columns.Add(new DataColumn("LastUpdatedBy_Id", typeof(int)));
                dataTable.Columns.Add(new DataColumn("CreatedAtUtc", typeof(DateTime)));
                dataTable.Columns.Add(new DataColumn("UpdatedAtUtc", typeof(DateTime)));

                ValidateAndSaveRestrictions(additions, dataTable, referenceStaging.Id, userId, false);

                ValidateAndSaveRestrictions(removals, dataTable, referenceStaging.Id, userId, true);

                if (!_accessRestrictionsStagingRepo.SaveBundle(dataTable, "Parkway_CBS_Core_" + typeof(MDARevenueAccessRestrictionsStaging).Name))
                {
                    throw new Exception($"Unable to save MDA Revenue Head Access Restrictions to MDARevenueAccessRestrictionsStaging table");
                }

                return reference;
            }
            catch (Exception)
            {
                _accessRestrictionsStagingRepo.RollBackAllTransactions();
                throw;
            }
        }



        /// <summary>
        /// Validate and save restricitons
        /// </summary>
        /// <param name="restrictionsMap"></param>
        /// <param name="dataTable"></param>
        /// <param name="referenceStagingId"></param>
        /// <param name="userId"></param>
        /// <param name="isRemoval"></param>
        private void ValidateAndSaveRestrictions(Dictionary<int, IEnumerable<int>> restrictionsMap, DataTable dataTable, long referenceStagingId, int userId, bool isRemoval)
        {
            foreach (var mda in restrictionsMap)
            {
                foreach (var rh in mda.Value)
                {
                    var row = dataTable.NewRow();
                    row["MDA_Id"] = mda.Key;
                    row["IsRemoval"] = isRemoval;
                    row["MDARevenueHeadEntryStaging_Id"] = referenceStagingId;
                    row["LastUpdatedBy_Id"] = userId;
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
        }


    }
}