using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Parkway.CBS.Core.DataFilters.TaxPayerReport.ProfileSearchFilters;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.DataFilters.TaxPayerReport
{
    public class TaxPayerReportingFilter : ITaxPayerReportFilter
    {
        private readonly ITransactionManager _transactionManager;
        private readonly IEnumerable<IProfileSearchFilters> _searchFilters;

        public TaxPayerReportingFilter(IOrchardServices orchardService, IEnumerable<IProfileSearchFilters> searchFilters)
        {
            _transactionManager = orchardService.TransactionManager;
            _searchFilters = searchFilters;
        }


        /// <summary>
        /// Get tax profile report based on skip, take and search parameters provided.
        /// </summary>
        /// <param name="searchModel">TaxProfilesSearchParams</param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public IEnumerable<TaxEntity> GetReportForTaxProfiles(TaxProfilesSearchParams searchModel, int skip, int take)
        {
            ICriteria query = GenerateCriteriaForTaxProfilesReport(searchModel);
            if(take != 0)
            {
                return query.SetFirstResult(skip).SetMaxResults(take).AddOrder(Order.Desc("Id")).Future<TaxEntity>();
            }

            return query.AddOrder(Order.Desc("Id")).Future<TaxEntity>();
        }


        public IEnumerable<TaxProfileReportStats> GetAggregateForTaxProfiles(TaxProfilesSearchParams searchModel)
        {
            var session = _transactionManager.GetSession();
            var query = GenerateCriteriaForTaxProfilesReport(searchModel);
            return query.SetProjection(
                Projections.ProjectionList()
                    //.Add(Projections.Sum("AmountPaid"), "TotalAmountOfPayment")
                    .Add(Projections.Count("Id"), "TotalNumberOfTaxProfiles")
            ).SetResultTransformer(Transformers.AliasToBean<TaxProfileReportStats>()).Future<TaxProfileReportStats>();
        }


        /// <summary>
        /// Build up the criteria expression tree for query the list tax profiles
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>ICriteria</returns>
        private ICriteria GenerateCriteriaForTaxProfilesReport(TaxProfilesSearchParams searchParams)
        {
            var session = _transactionManager.GetSession();
            var criteria = session.CreateCriteria<TaxEntity>();

            List<string> searchType = new List<string> { };
            searchType.Add(!string.IsNullOrEmpty(searchParams.Name) ? typeof(NameCR).Name : null);
            searchType.Add((!string.IsNullOrEmpty(searchParams.PhoneNumber)) ? typeof(PhoneNumberCR).Name : string.Empty);
            searchType.Add(!string.IsNullOrEmpty(searchParams.TIN) ? typeof(TINCR).Name : string.Empty);
            searchType.Add(!string.IsNullOrEmpty(searchParams.PayerId) ? typeof(PayerIDCR).Name : string.Empty);
            searchType.Add(searchParams.CategoryId != 0? typeof(CategoryCR).Name : string.Empty);

            foreach (var para in searchType)
            {
                if (string.IsNullOrEmpty(para)) { continue; }
                var filter = _searchFilters.Where(s => s.FilterName == para).SingleOrDefault();
                if (filter != null)
                { filter.AddCriteriaRestriction(criteria, searchParams); }
                else { throw new Exception("No search filter found"); }
            }
            return criteria;
        }


        private ICriteria BuildQuery(string searchParams)
        {
            var session = _transactionManager.GetSession();

            var taxEntityCriteria = session.CreateCriteria<TaxEntity>();


            if (searchParams != null)
            {
                taxEntityCriteria.Add(Restrictions.Disjunction()
                    .Add(Restrictions.InsensitiveLike("Recipient", String.IsNullOrEmpty(searchParams) ? "" : searchParams, MatchMode.Anywhere))
                    .Add(Restrictions.InsensitiveLike("TaxPayerIdentificationNumber", String.IsNullOrEmpty(searchParams) ? "" : searchParams, MatchMode.Anywhere))
                    .Add(Restrictions.InsensitiveLike("PhoneNumber", String.IsNullOrEmpty(searchParams) ? "" : searchParams, MatchMode.Anywhere))
                    .Add(Restrictions.InsensitiveLike("Email", String.IsNullOrEmpty(searchParams) ? "" : searchParams, MatchMode.Anywhere))
                    .Add(Restrictions.InsensitiveLike("Address", String.IsNullOrEmpty(searchParams) ? "" : searchParams, MatchMode.Anywhere)));
            }
            return taxEntityCriteria;
        }


        #region Statement of Account

        /// <summary>
        /// Get tax report for statement of account
        /// </summary>
        /// <param name="taxPayerIdNumber"></param>
        /// <param name="fromRange"></param>
        /// <param name="endRange"></param>
        /// <param name="paymentTypeId"></param>
        /// <returns>AccountStatementModel</returns>
        public AccountStatementModel GetReportForStatementOfAccount(Int64 taxPayerIdNumber, DateTime fromRange, DateTime endRange, int paymentTypeId)
        {
            List<AccountStatementLogModel> accountStatementLogs = new List<AccountStatementLogModel>();
            AccountStatementLogModel accountStatementLog;

            var session = _transactionManager.GetSession();

            var transactionLogCriteria = session.CreateCriteria<TransactionLog>("transactionLog");

            transactionLogCriteria.Add(Restrictions.Eq("transactionLog.TaxEntity.Id", taxPayerIdNumber));

            if (paymentTypeId != 0)
            {
                transactionLogCriteria.Add(Restrictions.Eq("TypeID", paymentTypeId));
            }

            transactionLogCriteria.Add(Restrictions.Between("PaymentDate", fromRange, endRange));

            var query = transactionLogCriteria.AddOrder(Order.Asc("InvoiceNumber")).Future<TransactionLog>();

            AccountStatementModel accountStatement = new AccountStatementModel();
            AccountStatementAggregate statementAggregate = GetAggregateForStatementOfAccount(fromRange, endRange, paymentTypeId, taxPayerIdNumber);

            if (query.Count() > 0)
            {
                foreach (var log in query)
                {
                    accountStatementLog = new AccountStatementLogModel();
                    accountStatementLog.InvoiceNumber = log.InvoiceNumber;
                    accountStatementLog.ReceiptNumber = log.ReceiptNumber;
                    accountStatementLog.Amount = log.AmountPaid;
                    accountStatementLog.BillAmount = log.Invoice.Amount;
                    accountStatementLog.TypeID = log.TypeID;
                    accountStatementLogs.Add(accountStatementLog);
                }
            }
            accountStatement.Report = accountStatementLogs;
            accountStatement.TotalBillAmount = statementAggregate.TotalBillAmount;
            accountStatement.TotalCreditAmount = statementAggregate.TotalCreditAmount;

            return accountStatement;
        }


        /// <summary>
        /// Get aggregate of TotalCreditAmount and TotalBillAmount
        /// </summary>
        /// <param name="fromRange"></param>
        /// <param name="endRange"></param>
        /// <param name="paymentTypeId"></param>
        /// <param name="taxPayerIdNumber"></param>
        /// <returns>AccountStatementAggregate</returns>
        public AccountStatementAggregate GetAggregateForStatementOfAccount(DateTime fromRange, DateTime endRange, int paymentTypeId, Int64 taxPayerIdNumber)
        {
            var session = _transactionManager.GetSession();
            var creditLogCriteria = session.CreateCriteria<TransactionLog>("transactionLog");
            creditLogCriteria.Add(Restrictions.Eq("transactionLog.TaxEntity.Id", taxPayerIdNumber));
            creditLogCriteria.Add(Restrictions.Between("PaymentDate", fromRange, endRange));
            creditLogCriteria.Add(Restrictions.Eq("TypeID", (int)Models.Enums.PaymentType.Credit));
            var totalCreditAmount = creditLogCriteria.SetProjection(
                Projections.ProjectionList()
                    .Add(Projections.Sum("AmountPaid"), "TotalCreditAmount")
            ).SetResultTransformer(Transformers.AliasToBean<AccountStatementAggregate>()).Future<AccountStatementAggregate>();

            var billLogCriteria = session.CreateCriteria<TransactionLog>("transactionLog");
            billLogCriteria.Add(Restrictions.Eq("transactionLog.TaxEntity.Id", taxPayerIdNumber));
            billLogCriteria.Add(Restrictions.Between("PaymentDate", fromRange, endRange));
            billLogCriteria.Add(Restrictions.Eq("TypeID", (int)Models.Enums.PaymentType.Bill));
            var totalBillAmount = billLogCriteria.SetProjection(
                Projections.ProjectionList()
                    .Add(Projections.Sum("AmountPaid"), "TotalBillAmount")
            ).SetResultTransformer(Transformers.AliasToBean<AccountStatementAggregate>()).Future<AccountStatementAggregate>();

            AccountStatementAggregate statementAggregate = null;
            switch (paymentTypeId)
            {
                case (int)Models.Enums.PaymentType.All:
                    statementAggregate = new AccountStatementAggregate();
                    statementAggregate.TotalBillAmount = totalBillAmount.First().TotalBillAmount;
                    statementAggregate.TotalCreditAmount = totalCreditAmount.First().TotalCreditAmount;
                    break;
                case (int)Models.Enums.PaymentType.Bill:
                    statementAggregate = new AccountStatementAggregate();
                    statementAggregate.TotalBillAmount = totalBillAmount.First().TotalBillAmount;
                    break;
                case (int)Models.Enums.PaymentType.Credit:
                    statementAggregate = new AccountStatementAggregate();
                    statementAggregate.TotalCreditAmount = totalCreditAmount.First().TotalCreditAmount;
                    break;
                default:
                    statementAggregate = new AccountStatementAggregate();
                    break;
            }
            return statementAggregate;
        }

        #endregion




        //private ICriteria BuildMultiQuery(string searchParams)
        //{
        //    var session = _transactionManager.GetSession();

        //    var multiResult = session.CreateMultiCriteria().Add(session.CreateCriteria(typeof(TaxEntity)).
        //                                                        Add(Restrictions.Disjunction()
        //                                                        .Add(Restrictions.InsensitiveLike("Recipient", String.IsNullOrEmpty(searchParams) ? "" : searchParams,                          MatchMode.Anywhere))
        //                                                        .Add(Restrictions.InsensitiveLike("TaxPayerIdentificationNumber", String.IsNullOrEmpty(searchParams) ?                          "" : searchParams, MatchMode.Anywhere))
        //                                                        .Add(Restrictions.InsensitiveLike("PhoneNumber", String.IsNullOrEmpty(searchParams) ? "" : searchParams,                        MatchMode.Anywhere))
        //                                                        .Add(Restrictions.InsensitiveLike("Email", String.IsNullOrEmpty(searchParams) ? "" : searchParams,                              MatchMode.Anywhere))
        //                                                        .Add(Restrictions.InsensitiveLike("Address", String.IsNullOrEmpty(searchParams) ? "" : searchParams,                           MatchMode.Anywhere))))
        //                                               .Add(session.CreateCriteria(typeof(TaxEntity)).SetProjection(Projections.RowCount())).

        //}
    }
}