using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Roles.Models;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class EscortRolePartialManager : BaseManager<EscortRolePartial>, IEscortRolePartialManager<EscortRolePartial>
    {
        private readonly IRepository<EscortRolePartial> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public ILogger Logger { get; set; }


        public EscortRolePartialManager(IRepository<EscortRolePartial> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
            _user = user;
        }

        /// <summary>
        /// Get the VM for partials
        /// </summary>
        /// <returns>List{EscortPartialVM}</returns>
        public IEnumerable<EscortPartialVM> GetPartials(int adminId)
        {
            try
            {
                var queryString = string.Format($"select {nameof(EscortRolePartial.PartialName)},{nameof(EscortRolePartial.ImplementationClass)}  from Parkway_CBS_Police_Core_{nameof(EscortRolePartial)} p inner join Orchard_Roles_UserRolesPartRecord as o on o.Role_id = p.Role_Id AND o.UserId = {adminId}");

                return _transactionManager.GetSession().CreateSQLQuery(queryString).SetResultTransformer(Transformers.AliasToBean<EscortPartialVM>()).List<EscortPartialVM>();
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, "Exception getting the partial for user Id " + adminId);
                throw;
            }
        }


        /// <summary>
        /// Get partials for role with specified id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public IEnumerable<EscortPartialVM> GetPartialsForRole(int roleId)
        {
            try
            {
                return _transactionManager.GetSession().Query<EscortRolePartial>().Where(x => x.Role.Id == roleId).Select(x => new EscortPartialVM
                {
                    PartialName = x.PartialName,
                    ImplementationClass = x.ImplementationClass
                });
            }catch(System.Exception exception)
            {
                Logger.Error(exception, $"Exception getting the partial for role id {roleId}");
                throw;
            }
        }

    }    
}