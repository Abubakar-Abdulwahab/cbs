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
    public class ApprovalAccessListStagingManager : BaseManager<ApprovalAccessListStaging>, IApprovalAccessListStagingManager<ApprovalAccessListStaging>
    {
        private readonly IRepository<ApprovalAccessListStaging> _repository;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public ApprovalAccessListStagingManager(IRepository<ApprovalAccessListStaging> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
        }

        /// <summary>
        /// Sets IsDeleted to true for the removed commands in ApprovalAccessList
        /// </summary>
        /// <param name="reference"></param>
        public void UpdateApprovalAccessListFromStaging(string reference)
        {
            try
            {

                var queryText = $"MERGE Parkway_CBS_Police_Core_ApprovalAccessList AS Target USING Parkway_CBS_Police_Core_ApprovalAccessListStaging AS Source ON Source.ApprovalAccessRoleUser_Id = Target.ApprovalAccessRoleUser_Id AND Source.Command_Id = Target.Command_Id AND Source.Service_Id = Target.Service_Id WHEN MATCHED AND Source.Reference = '{reference}' THEN UPDATE SET Target.IsDeleted = Source.IsDeleted  WHEN NOT MATCHED BY Target AND Source.Reference = '{reference}' THEN INSERT(State_Id, LGA_Id, Command_Id, CommandCategory_Id,Service_Id, ApprovalAccessRoleUser_Id, CreatedAtUtc, UpdatedAtUtc, IsDeleted) VALUES(Source.State_Id, Source.LGA_Id, Source.Command_Id, Source.CommandCategory_Id, Service_Id, Source.ApprovalAccessRoleUser_Id, GETDATE(), GETDATE(), 0);";
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