using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.Core.DataFilters.CollectionReport.Contracts;
using Parkway.CBS.Police.Core.DataFilters.CollectionReport.SearchFilters;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Parkway.CBS.Police.Core.DataFilters.CollectionReport
{
    public class PoliceCollectionFilter : IPoliceCollectionFilter
    {
        private readonly ITransactionManager _transactionManager;
        private readonly IEnumerable<Lazy<IPSSCollectionReportFilters>> _searchFilters;

        public PoliceCollectionFilter(IOrchardServices orchardService, IEnumerable<Lazy<IPSSCollectionReportFilters>> searchFilters)
        {
            _transactionManager = orchardService.TransactionManager;
            _searchFilters = searchFilters;
        }


        /// <summary>
        /// Get veiw model for request reports
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, Aggregate }</returns>
        public dynamic GetRequestReportViewModel(PSSCollectionSearchParams searchParams, bool applyAccessRestrictions)
        {
            dynamic returnOBJ = new ExpandoObject();

            try
            {
                returnOBJ.ReportRecords = GetReport(searchParams, applyAccessRestrictions);
                returnOBJ.Aggregate = GetAggregate(searchParams, applyAccessRestrictions);
            }
            catch (NoRecordFoundException)
            {
                returnOBJ.ReportRecords = new List<PSSRequestVM> { };
            }
            return returnOBJ;
        }


        /// <summary>
        /// Get the aggregate of the requests
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="applyAccessRestrictions"></param>
        /// <returns></returns>
        private IEnumerable<ReportStatsVM> GetAggregate(PSSCollectionSearchParams searchParams, bool applyAccessRestrictions)
        {
            var query = GetCriteria(searchParams, applyAccessRestrictions);

            return query.SetProjection(
               Projections.ProjectionList()
               .Add(Projections.Sum<PoliceCollectionLog>(x => (x.TransactionLog.AmountPaid)), "TotalAmount")
                    .Add(Projections.Count<PoliceCollectionLog>(x => x.TransactionLog.Invoice.Id), "TotalRecordCount")
               ).SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).Future<ReportStatsVM>();
        }


        /// <summary>
        /// Get the list of requests
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="applyAccessRestrictions"></param>
        /// <returns>IEnumerable{PSSRequestVM}</returns>
        private IEnumerable<PSSTransactionLogVM> GetReport(PSSCollectionSearchParams searchParams, bool applyAccessRestrictions)
        {
            var query = GetCriteria(searchParams, applyAccessRestrictions);

            if (!searchParams.DontPageData)
            {
                query
                .SetFirstResult(searchParams.Skip)
                .SetMaxResults(searchParams.Take);
            }

            List<BankVM> banks = Util.GetListOfObjectsFromJSONFile<BankVM>(SettingsFileNames.Banks.ToString());
            IEnumerable<PoliceCollectionLog> pssTransactionLogVM = query.AddOrder(Order.Desc("Id")).Future<PoliceCollectionLog>();

            return pssTransactionLogVM.Select(x =>
                    new PSSTransactionLogVM()
                    {
                        InvoiceNumber = x.TransactionLog.InvoiceNumber,
                        AmountPaid = Math.Round(x.TransactionLog.AmountPaid, 2) + 0.00M,
                        PaymentDate = x.TransactionLog.PaymentDate,
                        PaymentReference = x.TransactionLog.PaymentReference,
                        RevenueHeadName = x.TransactionLog.RevenueHead.Name,
                        PayerName = x.Request.CBSUser.Name,
                        ReceiptNumber = x.TransactionLog.Receipt.ReceiptNumber,
                        PaymentProviderName = Util.GetPaymentProviderDescription(x.TransactionLog.PaymentProvider),
                        Channel = Util.GetPaymentChannelDescription(x.TransactionLog.Channel),
                        Bank = Util.GetBankName(banks, x.TransactionLog.BankCode),
                        FileRefNumber = x.Request.FileRefNumber,
                        CommandName = x.Request.Command.Name
                    });
        }


        public ICriteria GetCriteria(PSSCollectionSearchParams searchParams, bool applyAccessRestrictions)
        {
            var session = _transactionManager.GetSession();
            var criteria = session.CreateCriteria<PoliceCollectionLog>("PCL");
            criteria
                .CreateAlias("PCL.TransactionLog", "TransactionLog")
                .CreateAlias("PCL.TransactionLog.RevenueHead", "RevenueHead")
                .CreateAlias("PCL.Request.TaxEntity", "TaxEntity")
                .CreateAlias("PCL.TransactionLog.Invoice", "Invoice")
                .CreateAlias("PCL.Request", "Request")
                .CreateAlias("PCL.Request.Service", "Service")
                .CreateAlias("PCL.Request.Command", "Command")
                .Add(Restrictions.Between($"TransactionLog.{searchParams.PaymentDirection.ToString()}", searchParams.StartDate, searchParams.EndDate));

            foreach (var filter in _searchFilters)
            {
                filter.Value.AddCriteriaRestriction(criteria, searchParams);
            }

            if (applyAccessRestrictions)
            {
                var pRequestCriteria = DetachedCriteria.For<PSSRequest>("pr")
                    .Add(Restrictions.EqProperty("Id", "PCL.Request.Id"))
                    .SetProjection(Projections.Constant(1));

                var commandCriteria = DetachedCriteria.For<Command>("cm")
                    .Add(Restrictions.EqProperty("Id", "pr.Command.Id"))
                    .SetProjection(Projections.Constant(1));

                var accessListCriteria = DetachedCriteria.For<ApprovalAccessList>("aal")
                .Add(Restrictions.Disjunction()
                .Add(Restrictions.And(Restrictions.And(Restrictions.EqProperty("CommandCategory.Id", "cm.CommandCategory.Id"), Restrictions.IsNull("State.Id")), Restrictions.IsNull("Command.Id")))
                .Add(Restrictions.And(Restrictions.And(Restrictions.EqProperty("CommandCategory.Id", "cm.CommandCategory.Id"), Restrictions.EqProperty("State.Id", "cm.State.Id")), Restrictions.IsNull("Command.Id")))
                .Add(Restrictions.And(Restrictions.And(Restrictions.EqProperty("CommandCategory.Id", "cm.CommandCategory.Id"), Restrictions.EqProperty("State.Id", "cm.State.Id")), Restrictions.EqProperty("Command.Id", "cm.Id"))))
                .SetProjection(Projections.Constant(1));

                var accessRoleUserCriteria = DetachedCriteria.For<ApprovalAccessRoleUser>("aru")
                    .Add(Restrictions.Eq("User.Id", searchParams.AdminUserId))
                    .Add(Restrictions.EqProperty("Id", "aal.ApprovalAccessRoleUser.Id"))
                    .SetProjection(Projections.Constant(1));


                accessListCriteria.Add(Subqueries.Exists(accessRoleUserCriteria));
                commandCriteria.Add(Subqueries.Exists(accessListCriteria));
                pRequestCriteria.Add(Subqueries.Exists(commandCriteria));
                criteria.Add(Subqueries.Exists(pRequestCriteria));
            }

            return criteria;
        }
    }
}