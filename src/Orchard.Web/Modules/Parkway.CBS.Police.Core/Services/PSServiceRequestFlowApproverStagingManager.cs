using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using System;

namespace Parkway.CBS.Police.Core.Services
{
    public class PSServiceRequestFlowApproverStagingManager : BaseManager<PSServiceRequestFlowApproverStaging>, IPSServiceRequestFlowApproverStagingManager<PSServiceRequestFlowApproverStaging>
    {
        private readonly IRepository<PSServiceRequestFlowApproverStaging> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public PSServiceRequestFlowApproverStagingManager(IRepository<PSServiceRequestFlowApproverStaging> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
            _user = user;
        }

        /// <summary>
        /// Sets IsDeleted to true for the removed approver in _PSServiceRequestFlowApprover
        /// </summary>
        /// <param name="reference"></param>
        public void UpdateServiceRequestFlowApproverFromStaging(string reference)
        {
            try
            {
                string targetTableName = "Parkway_CBS_Police_Core_" + typeof(PSServiceRequestFlowApprover).Name;
                string sourceTableName = "Parkway_CBS_Police_Core_" + typeof(PSServiceRequestFlowApproverStaging).Name;


                var queryText = $"MERGE {targetTableName} AS Target USING {sourceTableName} AS Source ON Source.PSSAdminUser_Id = Target.PSSAdminUser_Id AND Source.FlowDefinitionLevel_Id = Target.FlowDefinitionLevel_Id AND Source.AssignedApprover_Id = Target.AssignedApprover_Id WHEN MATCHED AND Source.Reference = '{reference}' THEN UPDATE SET Target.{nameof(PSServiceRequestFlowApprover.IsDeleted)} = Source.{nameof(PSServiceRequestFlowApproverStaging.IsDeleted)} WHEN NOT MATCHED BY Target AND Source.Reference = '{reference}' THEN INSERT(PSSAdminUser_Id, FlowDefinitionLevel_Id, AssignedApprover_Id, CreatedAtUtc, UpdatedAtUtc, IsDeleted) VALUES(Source.PSSAdminUser_Id, Source.FlowDefinitionLevel_Id, Source.AssignedApprover_Id, GETDATE(), GETDATE(), 0);";
                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);

                query.ExecuteUpdate();
            }
            catch (Exception exception)
            {
                RollBackAllTransactions();
                Logger.Error(exception, string.Format("Exception updating details for Approval Access List with user id {0}, Exception message {1}", reference, exception.Message));
                throw;
            }
        }

    }
}