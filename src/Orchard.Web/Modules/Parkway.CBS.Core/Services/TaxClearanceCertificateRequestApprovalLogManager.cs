using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Services
{
    public class TaxClearanceCertificateRequestApprovalLogManager : BaseManager<TaxClearanceCertificateRequestApprovalLog>, ITaxClearanceCertificateRequestApprovalLogManager<TaxClearanceCertificateRequestApprovalLog>  
    {
        private readonly IRepository<TaxClearanceCertificateRequestApprovalLog> _stateModelRepository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public TaxClearanceCertificateRequestApprovalLogManager(IRepository<TaxClearanceCertificateRequestApprovalLog> stateModelRepository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(stateModelRepository, user, orchardServices)
        {
            _stateModelRepository = stateModelRepository;
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// Get approval logs for request with specified Id
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>List<TCCApprovalLogVM></returns>
        public List<TCCApprovalLogVM> GetApprovalLogForRequestById(long requestId)
        {
            return _transactionManager.GetSession().Query<TaxClearanceCertificateRequestApprovalLog>().OrderByDescending(x => x.Id).Where(log => log.Request == new TaxClearanceCertificateRequest { Id = requestId }).Select(log => new TCCApprovalLogVM
            {
                Status = log.Status,
                ApprovingAdminUserName = log.AddedByAdminUser.UserName,
                ApprovalTime = log.CreatedAtUtc.ToString(),
                Comment = log.Comment
            }).ToList();
        }
    }
}