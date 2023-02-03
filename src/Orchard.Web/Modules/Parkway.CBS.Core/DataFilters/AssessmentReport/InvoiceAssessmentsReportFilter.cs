using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Parkway.CBS.Core.DataFilters.AssessmentReport.SearchFilters;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;

namespace Parkway.CBS.Core.DataFilters.AssessmentReport
{
    public class InvoiceAssessmentsReportFilter : IInvoiceAssessmentsReportFilter
    {
        private readonly ITransactionManager _transactionManager;
        protected readonly IEnumerable<IInvoiceAssessmentSearchFilter> _searchFilters;


        public InvoiceAssessmentsReportFilter(IOrchardServices orchardService, IEnumerable<IInvoiceAssessmentSearchFilter> searchFilters)
        {
            _transactionManager = orchardService.TransactionManager;
            _searchFilters = searchFilters;
        }


        public IEnumerable<CollectionReportStats> GetAggregate(InvoiceAssessmentSearchParams searchParams, bool applyAccessRestrictions)
        {
            var session = _transactionManager.GetSession();
            var query = GetCriteria(searchParams, applyAccessRestrictions);
            return query.SetProjection(
                Projections.ProjectionList()
                    .Add(Projections.Sum<InvoiceItems>(x => (x.TotalAmount)), "TotalAmountOfPayment")
                    .Add(Projections.CountDistinct<InvoiceItems>(x => x.Invoice.Id), "RecordCount")
            ).SetResultTransformer(Transformers.AliasToBean<CollectionReportStats>()).Future<CollectionReportStats>();
        }


        public IEnumerable<DetailReport> GetReport(InvoiceAssessmentSearchParams searchParams, bool applyAccessRestrictions)
        {
            var query = GetCriteria(searchParams, applyAccessRestrictions);

            if (!searchParams.DontPageData)
            {
                query
                .SetFirstResult(searchParams.Skip)
                .SetMaxResults(searchParams.Take);
            }


            query.SetProjection(
                Projections.ProjectionList()
                //.Add(Projections.Property("invoiceItems.CreatedAtUtc"), "InvoiceDate")
                .Add(Projections.Property<InvoiceItems>(invitm => invitm.CreatedAtUtc), "InvoiceDate")
                //.Add(Projections.Property("mda.Name"), "MDAName")
                .Add(Projections.Property<InvoiceItems>(invitm => invitm.Mda.Name), "MDAName")
                //.Add(Projections.Property("revenueHead.Name"), "RevenueHeadName")
                .Add(Projections.Property<InvoiceItems>(invitm => invitm.RevenueHead.Name), "RevenueHeadName")
                //.Add(Projections.Property("taxEntity.TaxPayerIdentificationNumber"), "PayersTIN")
                .Add(Projections.Property<InvoiceItems>(invitm => invitm.TaxEntity.TaxPayerIdentificationNumber), "PayersTIN")
                //.Add(Projections.Property("invoiceItems.InvoiceNumber"), "InvoiceNumber")
                .Add(Projections.Property<InvoiceItems>(invitm => invitm.InvoiceNumber), "InvoiceNumber")
                //.Add(Projections.Property("invoiceItems.UnitAmount"), "Amount")
                //.Add(Projections.Property<InvoiceItems>(invitm => invitm.UnitAmount), "Amount")
                .Add(Projections.Property<InvoiceItems>(invitm => invitm.TotalAmount), "TotalAmount")
                //.Add(Projections.Property("invoiceItems.Quantity"), "Quantity")
                .Add(Projections.Property<InvoiceItems>(invitm => invitm.Quantity), "Quantity")
                //.Add(Projections.Property("invoice.Status"), "PaymentStatus")
                .Add(Projections.Property<InvoiceItems>(invitm => invitm.Invoice.Status), "PaymentStatus")
                //.Add(Projections.Property("invoice.PaymentDate"), "PaymentDate")
                .Add(Projections.Property<InvoiceItems>(invitm => invitm.Invoice.PaymentDate), "PaymentDate")
                //.Add(Projections.Property("invoice.DueDate"), "DueDate")
                .Add(Projections.Property<InvoiceItems>(invitm => invitm.Invoice.DueDate), "DueDate")
                //.Add(Projections.Property("taxEntity.Recipient"), "TaxPayerName")
                .Add(Projections.Property<InvoiceItems>(invitm => invitm.TaxEntity.Recipient), "TaxPayerName")
                ).SetResultTransformer(Transformers.AliasToBean<DetailReport>());


            return query.AddOrder(Order.Desc("Id"))
                .Future<DetailReport>();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, Aggregate }</returns>
        public dynamic GetReportViewModel(InvoiceAssessmentSearchParams searchParams, bool applyAccessRestrictions)
        {
            dynamic returnOBJ = new ExpandoObject();
            returnOBJ.ReportRecords = GetReport(searchParams, applyAccessRestrictions);
            returnOBJ.Aggregate = GetAggregate(searchParams, applyAccessRestrictions);
            return returnOBJ;
        }


        public ICriteria GetCriteria(InvoiceAssessmentSearchParams searchParams, bool applyAccessRestrictions)
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
            var criteria = session.CreateCriteria<InvoiceItems>("InvoiceItems");
            criteria
                .CreateAlias("InvoiceItems.Mda", "Mda")
                .CreateAlias("InvoiceItems.RevenueHead", "RevenueHead")
                .CreateAlias("InvoiceItems.Invoice", "Invoice")
                .CreateAlias("InvoiceItems.TaxEntity", "TaxEntity");

            if (searchParams.DateFilterBy == FilterDate.DueDate)
            {
                criteria.Add(Restrictions.Between("Invoice.DueDate", searchParams.StartDate, searchParams.EndDate));
            }
            else
            {
                criteria.Add(Restrictions.Between("CreatedAtUtc", searchParams.StartDate, searchParams.EndDate));
            }


            foreach (var filter in _searchFilters)
            {
                filter.AddCriteriaRestriction(criteria, searchParams);
            }


            if (applyAccessRestrictions)
            {
                var armCriteria = DetachedCriteria.For<AccessRoleMDARevenueHead>("arm")
                .Add(Restrictions.Disjunction()
                //.Add(Restrictions.EqProperty("MDA.Id", "InvoiceItems.Mda.Id"))
                .Add(Restrictions
                    .And(Restrictions.EqProperty("MDA.Id", "InvoiceItems.Mda.Id"), Restrictions.IsNull("RevenueHead.Id")))
                .Add(Restrictions.EqProperty("RevenueHead.Id", "InvoiceItems.RevenueHead.Id")))
                .SetProjection(Projections.Constant(1));

                var aruCriteria = DetachedCriteria.For<AccessRoleUser>("aru")
                    .Add(Restrictions.Eq("User.Id", searchParams.AdminUserId))
                    .Add(Restrictions.EqProperty("AccessRole.Id", "arm.AccessRole.Id"))
                    .SetProjection(Projections.Constant(1));


                var arCriteria = DetachedCriteria.For<AccessRole>("ar")
                    .Add(Restrictions.Eq("AccessType", (int)AccessType.InvoiceAssessmentReport))
                    .Add(Restrictions.EqProperty("Id", "aru.AccessRole.Id"))
                    .SetProjection(Projections.Constant(1));


                aruCriteria.Add(Subqueries.Exists(arCriteria));
                armCriteria.Add(Subqueries.Exists(aruCriteria));
                criteria.Add(Subqueries.Exists(armCriteria));
            }

            return criteria;
        }


    }
}