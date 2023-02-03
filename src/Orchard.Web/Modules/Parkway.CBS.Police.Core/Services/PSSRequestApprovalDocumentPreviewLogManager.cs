using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.Services
{
    public class PSSRequestApprovalDocumentPreviewLogManager : BaseManager<PSSRequestApprovalDocumentPreviewLog>, IPSSRequestApprovalDocumentPreviewLogManager<PSSRequestApprovalDocumentPreviewLog>
    {
        private readonly IRepository<PSSRequestApprovalDocumentPreviewLog> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public PSSRequestApprovalDocumentPreviewLogManager(IRepository<PSSRequestApprovalDocumentPreviewLog> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
        }


        
    }
}