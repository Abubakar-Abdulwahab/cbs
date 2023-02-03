using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Linq;

namespace Parkway.CBS.Core.Services
{
    public class TaxClearanceCertificateRequestApproverManager : BaseManager<TaxClearanceCertificateRequestApprover>, ITaxClearanceCertificateRequestApproverManager<TaxClearanceCertificateRequestApprover>
    {
        private readonly IRepository<TaxClearanceCertificateRequestApprover> _stateModelRepository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public TaxClearanceCertificateRequestApproverManager(IRepository<TaxClearanceCertificateRequestApprover> stateModelRepository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(stateModelRepository, user, orchardServices)
        {
            _stateModelRepository = stateModelRepository;
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// Get user approver details using the userAdminId
        /// </summary>
        /// <param name="adminUserId"></param>
        /// <returns>WorkflowApproverDetailVM</returns>
        public WorkflowApproverDetailVM GetApproverDetails(int adminUserId)
        {
            try
            {
                return _transactionManager.GetSession().Query<TaxClearanceCertificateRequestApprover>().Where(x => x.AssignedApprover == new UserPartRecord { Id = adminUserId }).Select(x => new WorkflowApproverDetailVM
                {
                    PhoneNumber = x.ContactPhoneNumber,
                    ApprovalLevelId = x.ApprovalLevelId,
                }).Single();
            }
            catch (Exception exception)
            {
                Logger.Error($"Could not get request approver for admin user Id {adminUserId}. Exception message {exception.Message}");
                throw new NoRecordFoundException();
            }
        }

    }
}