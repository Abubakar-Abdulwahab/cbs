using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Orchard.Environment;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.OSGOF.Admin.Models;
using Parkway.CBS.OSGOF.Admin.Services.Contracts;
using Parkway.CBS.OSGOF.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.OSGOF.Admin.Services
{
    public class CellSitesManager : BaseManager<CellSites>, ICellSiteManager<CellSites>
    {
        private readonly IRepository<CellSites> _repository;
        private readonly IOrchardServices _orchardServices;
        private readonly IRepository<UserPartRecord> _user;
        private readonly ITransactionManager _transactionManager;

        public CellSitesManager(IRepository<CellSites> repository, IRepository<UserPartRecord> user, IOrchardServices orchardService) : base(repository, user, orchardService)
        {
            _transactionManager = orchardService.TransactionManager;
            _repository = repository;
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public IEnumerable<CellSites> GetCellSites(long operatorId, int skip, int take)
        {
            ICriteria query = GenerateCriteriaForCellSites(operatorId);
            return query.SetFirstResult(skip).SetMaxResults(take).AddOrder(Order.Desc("Id")).Future<CellSites>();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private ICriteria GenerateCriteriaForCellSites(long operatorID)
        {
            var session = _transactionManager.GetSession();
            var criteria = session.CreateCriteria<CellSites>();
            if (operatorID != 0)
            {
                criteria.Add(Restrictions.Eq("TaxProfile", new TaxEntity { Id = operatorID }));
            }

            return criteria;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CellSitesVM> GetAggregateForCellSites(long operatorID)
        {
            var session = _transactionManager.GetSession();
            var query = GenerateCriteriaForCellSites(operatorID);
            return query.SetProjection(
                Projections.ProjectionList()
                    .Add(Projections.Count("Id"), "TotalNumberOfCellSites")
            ).SetResultTransformer(Transformers.AliasToBean<CellSitesVM>()).Future<CellSitesVM>();
        }

        /// <summary>
        /// Get Cell Sites for an Operator based on LGA
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public List<CellSitesDropdownBindingVM> GetOperatorCellSites(int taxEntityId, int LGAId)
        {
            return _transactionManager.GetSession().Query<CellSites>()
            .Where(txp => txp.TaxProfile.Id == taxEntityId && txp.LGA.Id == LGAId)
            .Select(txp => new CellSitesDropdownBindingVM { OperatorSiteId = txp.OperatorSiteId, LGAId=txp.LGA.Id, Id = txp.Id }).OrderBy(k => k.Id).ToList();
        }

        /// <summary>
        /// Get Cell Site details
        /// </summary>
        /// <param name="cellSiteId"></param>
        /// <returns></returns>
        public CellSitesDetailsVM GetCellSite(int cellSiteId)
        {
            return _transactionManager.GetSession().Query<CellSites>()
            .Where(txp => txp.Id == cellSiteId)
            .Select(txp => new CellSitesDetailsVM { OperatorSiteId = txp.OperatorSiteId,OSGOFID = txp.OSGOFId, Amount = txp.OperatorCategory.Amount, Lat = txp.Lat, Long = txp.Long, SiteAddress=txp.SiteAddress, Id = txp.Id }).SingleOrDefault();
        }

    }
}