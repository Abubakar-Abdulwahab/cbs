using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Core.Services
{
    public class MDARevenueAccessRestrictionsManager : BaseManager<MDARevenueAccessRestrictions>, IMDARevenueAccessRestrictionsManager<MDARevenueAccessRestrictions>
    {
        private readonly IRepository<MDARevenueAccessRestrictions> _accessRestrictionsRepository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public MDARevenueAccessRestrictionsManager(IRepository<MDARevenueAccessRestrictions> accessRestrictionsRepository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(accessRestrictionsRepository, user, orchardServices)
        {
            _accessRestrictionsRepository = accessRestrictionsRepository;
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
        }


        /// <summary>
        /// Synchronize MDARevenueAccessRestrictions table with changes in the MDARevenueAccessRestrictionsStaging table
        /// </summary>
        /// <param name="hashCodeOfImplementingClass">hash code of the operation class</param>
        /// <param name="operationTypeIdentifierId"></param>
        /// <param name="MDARevenueHeadEntryStagingId"></param>
        public void UpdateRestrictionsRecords(int hashCodeOfImplementingClass, long operationTypeIdentifierId, Int64 MDARevenueHeadEntryStagingId)
        {
            string targetTable = "Parkway_CBS_Core_" + typeof(MDARevenueAccessRestrictions).Name;
            string sourceTable = "Parkway_CBS_Core_" + typeof(MDARevenueAccessRestrictionsStaging).Name;

            var queryText = $"MERGE {targetTable} AS TARGET USING {sourceTable} AS SOURCE ON((TARGET.RevenueHead_Id = SOURCE.RevenueHead_Id OR(TARGET.RevenueHead_Id IS NULL AND SOURCE.RevenueHead_Id IS NULL)) AND TARGET.OperationTypeIdentifierId = :operationTypeIdentifierId AND TARGET.OperationType = :operationType AND TARGET.MDA_Id = SOURCE.MDA_Id) WHEN MATCHED AND SOURCE.MDARevenueHeadEntryStaging_Id = :MDARevenueHeadEntryStagingId THEN UPDATE SET TARGET.IsDeleted = SOURCE.IsRemoval, TARGET.UpdatedAtUtc = :updateDate WHEN NOT MATCHED BY TARGET AND SOURCE.MDARevenueHeadEntryStaging_Id = :MDARevenueHeadEntryStagingId THEN INSERT(MDA_Id, RevenueHead_Id, LastUpdatedBy_Id, CreatedAtUtc, UpdatedAtUtc, OperationType, OperationTypeIdentifierId, IsDeleted) VALUES(SOURCE.MDA_Id, SOURCE.RevenueHead_Id, SOURCE.LastUpdatedBy_Id, :updateDate, :updateDate, :operationType, :operationTypeIdentifierId, SOURCE.IsRemoval);";

            var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
            query.SetParameter("operationTypeIdentifierId", operationTypeIdentifierId);
            query.SetParameter("operationType", hashCodeOfImplementingClass);
            query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
            query.SetParameter("MDARevenueHeadEntryStagingId", MDARevenueHeadEntryStagingId);

            query.ExecuteUpdate();
        }


    }
}