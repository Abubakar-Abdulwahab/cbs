using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Parkway.CBS.Core.DataFilters.CollectionReport.SearchFilters;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using System.Collections.Generic;
using System.Dynamic;

namespace Parkway.CBS.Core.DataFilters.CollectionReport
{
    public class CollectionReportingFilter : ICollectionReportFilter
    {
        private readonly ITransactionManager _transactionManager;
        private readonly IEnumerable<ICollectionReportFilters> _searchFilters;

        public CollectionReportingFilter(IOrchardServices orchardService, IEnumerable<ICollectionReportFilters> searchFilters)
        {
            _transactionManager = orchardService.TransactionManager;
            _searchFilters = searchFilters;
        }       


        public ICriteria GetCriteria(CollectionSearchParams searchParams, bool applyAccessRestrictions)
        {
            #region SQL
            //criteria
            //    //.CreateAlias("invoiceItems.Mda", "mda")
            //    //.CreateAlias("invoiceItems.RevenueHead", "revenueHead")
            //    //.CreateAlias("invoiceItems.Invoice", "invoice")
            //    .Add(Restrictions.Between("CreatedAtUtc", searchParams.StartDate, searchParams.EndDate));
            /*
             * SELECT * 
                FROM 
                [CentralBillingSystem].[dbo].[Parkway_CBS_Core_InvoiceItems] it
                INNER JOIN [CentralBillingSystem].[dbo].[Parkway_CBS_Core_AccessRoleMDARevenueHead] p ON (it.MDA_Id = p.MDA_Id AND p.RevenueHead_Id IS NULL) OR it.RevenueHead_Id = p.RevenueHead_Id
                INNER JOIN [CentralBillingSystem].[dbo].[Parkway_CBS_Core_AccessRoleUser] au ON au.User_Id = 2 AND p.AccessRole_Id = au.AccessRole_Id
                INNER JOIN [CentralBillingSystem].[dbo].[Parkway_CBS_Core_AccessRole] ar ON ar.AccessType = 1 AND ar.Id = au.AccessRole_Id
                WHERE it.Id > 0
             */
            #endregion

            var session = _transactionManager.GetSession();
            var criteria = session.CreateCriteria<TransactionLog>("TransactionLog");
            criteria
                .CreateAlias("TransactionLog.MDA", "Mda")
                .CreateAlias("TransactionLog.RevenueHead", "RevenueHead")
                .CreateAlias("TransactionLog.Invoice", "Invoice")
                .CreateAlias("TransactionLog.TaxEntity", "TaxEntity")
                .Add(!Restrictions.Eq("TypeID", (int)PaymentType.Bill))
                .Add(Restrictions.Between(searchParams.PaymentDirection.ToString(), searchParams.FromRange.Value, searchParams.EndRange.Value));


            if (searchParams.TaxEntityId != 0)
            {
                criteria.Add(Restrictions.Eq("TransactionLog.TaxEntity.Id", searchParams.TaxEntityId));
            }

            foreach (var filter in _searchFilters)
            {
                filter.AddCriteriaRestriction(criteria, searchParams);
            }


            if (applyAccessRestrictions)
            {
                var armCriteria = DetachedCriteria.For<AccessRoleMDARevenueHead>("arm")
                .Add(Restrictions.Disjunction()
                .Add(Restrictions
                    .And(Restrictions.EqProperty("MDA.Id", "TransactionLog.MDA.Id"), Restrictions.IsNull("RevenueHead.Id")))
                .Add(Restrictions.EqProperty("RevenueHead.Id", "TransactionLog.RevenueHead.Id")))
                .SetProjection(Projections.Constant(1));

                var aruCriteria = DetachedCriteria.For<AccessRoleUser>("aru")
                    .Add(Restrictions.Eq("User.Id", searchParams.AdminUserId))
                    .Add(Restrictions.EqProperty("AccessRole.Id", "arm.AccessRole.Id"))
                    .SetProjection(Projections.Constant(1));


                var arCriteria = DetachedCriteria.For<AccessRole>("ar")
                    .Add(Restrictions.Eq("AccessType", (int)AccessType.CollectionReport))
                    .Add(Restrictions.EqProperty("Id", "aru.AccessRole.Id"))
                    .SetProjection(Projections.Constant(1));


                aruCriteria.Add(Subqueries.Exists(arCriteria));
                armCriteria.Add(Subqueries.Exists(aruCriteria));
                criteria.Add(Subqueries.Exists(armCriteria));
            }

            return criteria;
        }


        public IEnumerable<TransactionLog> GetReport(CollectionSearchParams searchParams, bool applyAccessRestrictions)
        {
            var query = GetCriteria(searchParams, applyAccessRestrictions);

            if (!searchParams.DontPageData)
            {
                query
                .SetFirstResult(searchParams.Skip)
                .SetMaxResults(searchParams.Take);
            }

            return query.AddOrder(Order.Desc(searchParams.PaymentDirection.ToString())).Future<TransactionLog>();
        }



        /// <summary>
        /// Get view model for collection report
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>dynamic</returns>
        public dynamic GetReportViewModel(CollectionSearchParams searchParams, bool applyAccessRestrictions)
        {
            dynamic returnOBJ = new ExpandoObject();
            returnOBJ.ReportRecords = GetReport(searchParams, applyAccessRestrictions);
            returnOBJ.Aggregate = GetAggregate(searchParams, applyAccessRestrictions);
            return returnOBJ;
        }



        public IEnumerable<CollectionReportStats> GetAggregate(CollectionSearchParams searchParams, bool applyAccessRestrictions)
        {
            var session = _transactionManager.GetSession();
            var query = GetCriteria(searchParams, applyAccessRestrictions);
            return query.SetProjection(
                Projections.ProjectionList()
                    .Add(Projections.Sum<TransactionLog>(x => (x.AmountPaid)), "TotalAmountOfPayment")
                    .Add(Projections.CountDistinct<TransactionLog>(x => x.Invoice.Id), "RecordCount")
            ).SetResultTransformer(Transformers.AliasToBean<CollectionReportStats>()).Future<CollectionReportStats>();
        }
        
    }

}