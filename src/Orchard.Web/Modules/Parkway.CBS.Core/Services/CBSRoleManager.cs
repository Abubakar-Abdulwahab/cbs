using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;

namespace Parkway.CBS.Core.Services
{
    public class CBSRoleManager : BaseManager<CBSRole>, ICBSRoleManager<CBSRole>
    {
        private readonly IRepository<CBSRole> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;


        public CBSRoleManager(IRepository<CBSRole> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _user = user;
            _transactionManager = orchardServices.TransactionManager;
        }

    }

    public class CBSPermissionManager : BaseManager<CBSPermission>, ICBSPermissionManager<CBSPermission>
    {
        private readonly IRepository<CBSPermission> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;


        public CBSPermissionManager(IRepository<CBSPermission> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _user = user;
            _transactionManager = orchardServices.TransactionManager;
        }

    }

    public class CBSUserRoleManager : BaseManager<CBSUserRole>, ICBSUserRoleManager<CBSUserRole>
    {
        private readonly IRepository<CBSUserRole> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;


        public CBSUserRoleManager(IRepository<CBSUserRole> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _user = user;
            _transactionManager = orchardServices.TransactionManager;
        }
    }

    public class CBSRolePermissionManager : BaseManager<CBSRolePermission>, ICBSRolePermissionManager<CBSRolePermission>
    {
        private readonly IRepository<CBSRolePermission> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;


        public CBSRolePermissionManager(IRepository<CBSRolePermission> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _user = user;
            _transactionManager = orchardServices.TransactionManager;
        }

    }
}