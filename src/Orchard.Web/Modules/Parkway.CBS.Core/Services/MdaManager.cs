using System;
using System.Collections.Generic;
using Orchard.Data;
using Orchard.Users.Models;
using System.Linq.Expressions;
using Orchard;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.HelperModels;
using NHibernate;
using NHibernate.Criterion;
using Parkway.CBS.Core.Models.Enums;
using NHibernate.Transform;
using NHibernate.Linq;
using System.Linq;

namespace Parkway.CBS.Core.Services
{
    public class MDAManager : BaseManager<MDA>, IMDAManager<MDA>
    {
        private readonly IRepository<MDA> _mdaRepository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;

        public MDAManager(IRepository<MDA> mdaRepository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(mdaRepository, user, orchardServices)
        {
            _mdaRepository = mdaRepository;
            _orchardServices = orchardServices;
        }


        /// <summary>
        /// Get the list of MDAs that this user has access to
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>IEnumerable{MDAVM}</returns>
        public IEnumerable<MDAVM> GetAccessList(int userId, AccessType accessType, bool applyAccessRestrictions = false)
        {

            var session = _orchardServices.TransactionManager.GetSession();
            var criteria = session.CreateCriteria<MDA>("Mda");


            if (applyAccessRestrictions)
            {
                var armCriteria = DetachedCriteria.For<AccessRoleMDARevenueHead>("arm")
                .Add(Restrictions.EqProperty("MDA.Id", "Mda.Id"))
                .SetProjection(Projections.Constant(1));

                var aruCriteria = DetachedCriteria.For<AccessRoleUser>("aru")
                    .Add(Restrictions.Eq("User.Id", userId))
                    .Add(Restrictions.EqProperty("AccessRole.Id", "arm.AccessRole.Id"))
                    .SetProjection(Projections.Constant(1));


                var arCriteria = DetachedCriteria.For<AccessRole>("ar")
                    .Add(Restrictions.Eq("AccessType", (int)accessType))
                    .Add(Restrictions.EqProperty("Id", "aru.AccessRole.Id"))
                    .SetProjection(Projections.Constant(1));

                aruCriteria.Add(Subqueries.Exists(arCriteria));
                armCriteria.Add(Subqueries.Exists(aruCriteria));
                criteria.Add(Subqueries.Exists(armCriteria));
            }

            criteria.SetProjection(
                 Projections.ProjectionList()
                .Add(Projections.Property<MDA>(m => m.Name), "Name")
                .Add(Projections.Property<MDA>(m => m.Code), "Code")
                .Add(Projections.Property<MDA>(m => m.Id), "Id")
                ).SetResultTransformer(Transformers.AliasToBean<MDAVM>());

            return criteria.AddOrder(Order.Asc("Name"))
                .Future<MDAVM>();
        }


        public List<MDAVM> GetActiveMDAVMs()
        {
            return _orchardServices.TransactionManager.GetSession().Query<MDA>().Where(mda => mda.IsActive == true).Select(mda => new MDAVM { Id = mda.Id, Name = mda.Name }).ToList();
        }


        public void UpdateMDAPaymentProviderValidationConstraintsStatus()
        {
            string PVCtableName = "Parkway_CBS_Core_" + nameof(PaymentProviderValidationConstraint);

            var queryText = $"UPDATE mda SET mda.HasPaymentProviderValidationConstraint = :constraintStatus, mda.UpdatedAtUtc = :dateUpdated FROM Parkway_CBS_Core_MDA mda" +
                     $" INNER JOIN {PVCtableName} pvc ON pvc.MDA_Id = mda.Id";
            var query = _orchardServices.TransactionManager.GetSession().CreateSQLQuery(queryText);
            query.SetParameter("dateUpdated", DateTime.Now.ToLocalTime());
            query.SetParameter("constraintStatus", true);
            query.ExecuteUpdate();
        }
       



        /// <summary>
        /// Get list of all MDAs no filter
        /// </summary>
        /// <returns>IEnumerable{MDAVM}</returns>
        public IEnumerable<MDAVM> GetListOfMDAs()
        {
            return _orchardServices.TransactionManager.GetSession().Query<MDA>().Select(m => new MDAVM { Code = m.Code, Id = m.Id, Name = m.Name });
        }


        /// <summary>
        /// Check if MDA exists
        /// </summary>
        /// <param name="key"></param>
        /// <returns>bool</returns>
        public bool CheckIfMDAExists(int mdaId)
        {
            return _orchardServices.TransactionManager.GetSession().Query<MDA>().Count(m => m.Id == mdaId) == 1;
        }

    }
}