using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Core.Services
{
    public class RevenueHeadPermissionConstraintsManager : BaseManager<RevenueHeadPermissionConstraints>, IRevenueHeadPermissionConstraintsManager<RevenueHeadPermissionConstraints>
    {
        private readonly IRepository<RevenueHeadPermissionConstraints> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public RevenueHeadPermissionConstraintsManager(IRepository<RevenueHeadPermissionConstraints> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _user = user;
            _transactionManager = orchardServices.TransactionManager;

        }


        /// <summary>
        /// This method fetches existing revenue head constraints for the expert system with the specified Id.
        /// </summary>
        /// <param name="expertSystemId"></param>
        /// <param name="permissionId">Permission Id</param>
        /// <returns></returns>
        public IEnumerable<RevenueHeadPermissionsConstraintsVM> GetExistingConstraints(int expertSystemId, int permissionId)
        {
            return _transactionManager.GetSession().Query<RevenueHeadPermissionConstraints>()
                .Where(rhConst => ((rhConst.ExpertSystem.Id == expertSystemId) && (rhConst.RevenueHeadPermission.Id == permissionId)))
                .Select(rhConst => new RevenueHeadPermissionsConstraintsVM { MDAId = rhConst.MDA.Id, RevenueHeadId = (rhConst.RevenueHead == null) ? 0 : rhConst.RevenueHead.Id, PermissionId = rhConst.RevenueHeadPermission.Id });
        }


        /// <summary>
        /// Delete records for expert system with specified Id and selected permission.
        /// </summary>
        /// <param name="expertSystemId"></param>
        public void DeleteExpertSystemRecords(int expertSystemId)
        {
           _transactionManager.GetSession().CreateQuery("Delete from " + typeof(RevenueHeadPermissionConstraints) + " where ExpertSystem_Id = " + expertSystemId).ExecuteUpdate();
        }



        public void ClearTable()
        {
            _transactionManager.GetSession().CreateSQLQuery("TRUNCATE TABLE Parkway_CBS_Core_" + nameof(RevenueHeadPermissionConstraints)+";").ExecuteUpdate();
        }


        public void UpdateMDAId()
        {
            try
            {
                var queryText = $"UPDATE rpc SET rpc.{nameof(RevenueHeadPermissionConstraints.MDA)}_Id = rh.{nameof(RevenueHead.Mda)}_Id FROM Parkway_CBS_Core_{nameof(RevenueHeadPermissionConstraints)} rpc INNER JOIN Parkway_CBS_Core_{nameof(RevenueHead)} as rh ON rh.{nameof(RevenueHead.Id)} = rpc.{nameof(RevenueHeadPermissionConstraints.RevenueHead)}_Id";
                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.ExecuteUpdate();
            }
            catch (Exception)
            { throw; }
        }

    }
}