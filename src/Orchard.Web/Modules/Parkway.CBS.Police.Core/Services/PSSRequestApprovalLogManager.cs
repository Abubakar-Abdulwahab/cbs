using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.Services
{
    public class PSSRequestApprovalLogManager : BaseManager<PSSRequestApprovalLog>, IPSSRequestApprovalLogManager<PSSRequestApprovalLog>
    {
        private readonly IRepository<PSSRequestApprovalLog> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public PSSRequestApprovalLogManager(IRepository<PSSRequestApprovalLog> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
            _user = user;
        }


        /// <summary>
        /// Get approval logs for request with specified Id
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public List<ApprovalLogVM> GetApprovalLogForRequestById(long requestId)
        {
           return  _transactionManager.GetSession().Query<PSSRequestApprovalLog>().OrderByDescending(x => x.Id).Where(log => log.Request == new PSSRequest { Id = requestId }).Select(log => new ApprovalLogVM
            {
                Id = log.Id,
                Status = log.Status,
                ApprovingAdminUserName = log.AddedByAdminUser.UserName,
                ApprovalTime = log.CreatedAtUtc.ToString(),
                Comment = log.Comment
            }).ToList();
        }


    }
}