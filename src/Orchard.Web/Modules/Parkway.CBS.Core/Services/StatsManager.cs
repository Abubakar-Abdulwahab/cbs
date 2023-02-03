using NHibernate.Criterion;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models.Enums;
using NHibernate;
using NHibernate.Linq;
using Parkway.CBS.Core.DataFilters.AssessmentReport.SearchFilters;

namespace Parkway.CBS.Core.Services
{
    public class StatsManager : IStatsManager
    {
        private readonly IRepository<Stats> _repository;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;


        public StatsManager(IOrchardServices orchardServices, IRepository<Stats> repository)
        {
            _repository = repository;
            _transactionManager = orchardServices.TransactionManager;
        }


        public IList<StatsPerMonth> GetStatsForPieChart(DateTime startDate, DateTime endDate, string mdaSelected, MDA mda = null)
        {
            try
            {
                var session = _transactionManager.GetSession();
                if(mdaSelected == "All")
                {
                    //var tt = session.CreateCriteria<Stats>()
                    //                    .SetProjection(
                    //                        Projections.ProjectionList()
                    //                        .Add(Projections.Sum("AmountExpected"), "AmountExpected")
                    //                        .Add(Projections.Sum("AmountPaid"), "AmountPaid")
                    //                        .Add(Projections.Sum("NumberOfInvoicesSent"), "NumberOfInvoicesSent")
                    //                        .Add(Projections.Sum("NumberOfInvoicesPaid"), "NumberOfInvoicesPaid")
                    //                        .Add(Projections.GroupProperty("Mda"), "Mda")
                    //                    ).Add(Restrictions.Between("DueDate", startDate, endDate))
                    //                    .SetResultTransformer(Transformers.AliasToBean<StatsPerMonth>()).List<StatsPerMonth>();
                    return session.CreateCriteria<Stats>()
                                        .SetProjection(
                                            Projections.ProjectionList()
                                            .Add(Projections.Sum("AmountExpected"), "AmountExpected")
                                            .Add(Projections.Sum("AmountPaid"), "AmountPaid")
                                            .Add(Projections.Sum("NumberOfInvoicesSent"), "NumberOfInvoicesSent")
                                            .Add(Projections.Sum("NumberOfInvoicesPaid"), "NumberOfInvoicesPaid")
                                            .Add(Projections.GroupProperty("Mda"), "Mda")
                                        ).Add(Restrictions.Between("DueDate", startDate, endDate))
                                        .SetResultTransformer(Transformers.AliasToBean<StatsPerMonth>()).List<StatsPerMonth>();
                }
                else
                {
                    //var fg = session.CreateCriteria<Stats>()
                    //                    .SetProjection(
                    //                        Projections.ProjectionList()
                    //                        .Add(Projections.Sum("AmountExpected"), "AmountExpected")
                    //                        .Add(Projections.Sum("AmountPaid"), "AmountPaid")
                    //                        .Add(Projections.Sum("NumberOfInvoicesSent"), "NumberOfInvoicesSent")
                    //                        .Add(Projections.Sum("NumberOfInvoicesPaid"), "NumberOfInvoicesPaid")
                    //                        .Add(Projections.GroupProperty("RevenueHead"), "RevenueHead")
                    //                    ).Add(Restrictions.Between("DueDate", startDate, endDate))
                    //                    .Add(Restrictions.Eq("Mda", mda))
                    //                    .SetResultTransformer(Transformers.AliasToBean<StatsPerMonth>()).List<StatsPerMonth>();

                    return session.CreateCriteria<Stats>()
                                        .SetProjection(
                                            Projections.ProjectionList()
                                            .Add(Projections.Sum("AmountExpected"), "AmountExpected")
                                            .Add(Projections.Sum("AmountPaid"), "AmountPaid")
                                            .Add(Projections.Sum("NumberOfInvoicesSent"), "NumberOfInvoicesSent")
                                            .Add(Projections.Sum("NumberOfInvoicesPaid"), "NumberOfInvoicesPaid")
                                            .Add(Projections.GroupProperty("RevenueHead"), "RevenueHead")
                                        ).Add(Restrictions.Between("DueDate", startDate, endDate))
                                        .Add(Restrictions.Eq("Mda", mda))
                                        .SetResultTransformer(Transformers.AliasToBean<StatsPerMonth>()).List<StatsPerMonth>();
                }
                
                #region Equivalent SQL query
                //SELECT Date as month, SUM(AmountExpected) as sum, SUM(AmountPaid) as amountpaid, SUM(NumberOfInvoicesSent) numbersent, SUM(NumberOfInvoicesPaid) numberpaid
                //FROM[CentralBillingSystem].[dbo].[ParkWay_CBS_Module_Stats]
                //WHERE Date between '2016-06-01 00:00:00.000' AND '2017-12-01 00:00:00.000'
                //GROUP BY Date order by date; 
                #endregion
            }
            catch (Exception exception)
            {
                return new List<StatsPerMonth>();
            }
        }

        public IList<StatsPerMonth> GetStats(DateTime? startDate, DateTime? endDate)
        {
            var session = _transactionManager.GetSession();
            if (startDate == null || endDate == null)
            {
                startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-11);
                endDate = new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(1).Month, 1);
            }

            try
            {
                return session.CreateCriteria<Stats>()
                                        .SetProjection(
                                            Projections.ProjectionList()
                                            .Add(Projections.Sum("AmountExpected"), "AmountExpected")
                                            .Add(Projections.Sum("AmountPaid"), "AmountPaid")
                                            .Add(Projections.Sum("NumberOfInvoicesSent"), "NumberOfInvoicesSent")
                                            .Add(Projections.Sum("NumberOfInvoicesPaid"), "NumberOfInvoicesPaid")
                                            .Add(Projections.GroupProperty("DueDate"), "DueDate")
                                        ).Add(Restrictions.Ge("DueDate", startDate) && (Restrictions.Le("DueDate", endDate)))
                                        .SetResultTransformer(Transformers.AliasToBean<StatsPerMonth>()).AddOrder(Order.Asc("DueDate")).List<StatsPerMonth>();
                #region Equivalent SQL query
                //SELECT Date as month, SUM(AmountExpected) as sum, SUM(AmountPaid) as amountpaid, SUM(NumberOfInvoicesSent) numbersent, SUM(NumberOfInvoicesPaid) numberpaid
                //FROM[CentralBillingSystem].[dbo].[ParkWay_CBS_Module_Stats]
                //WHERE Date between '2016-06-01 00:00:00.000' AND '2017-12-01 00:00:00.000'
                //GROUP BY Date order by date; 
                #endregion
            }
            catch (Exception exception)
            {
                return new List<StatsPerMonth>();
            }
        }


        public IList<StatsPerMonth> GetStatsForRevenueHead(RevenueHead revenueHead, DateTime? startDate, DateTime? endDate)
        {
            var session = _transactionManager.GetSession();
            if (startDate == null || endDate == null)
            {
                startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-11);
                endDate = new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(1).Month, 1);
            }

            try
            {
                return session.CreateCriteria<Stats>()
                                        .SetProjection(
                                            Projections.ProjectionList()
                                            .Add(Projections.Sum("AmountExpected"), "AmountExpected")
                                            .Add(Projections.Sum("AmountPaid"), "AmountPaid")
                                            .Add(Projections.Sum("NumberOfInvoicesSent"), "NumberOfInvoicesSent")
                                            .Add(Projections.Sum("NumberOfInvoicesPaid"), "NumberOfInvoicesPaid")
                                             .Add(Projections.Property("CreatedAtUtc"), "CreatedAtUtc")
                                            .Add(Projections.Property("DueDate"), "DueDate")
                                           .Add(Projections.GroupProperty("DueDate"), "DueDate")
                                            .Add(Projections.GroupProperty("CreatedAtUtc"), "CreatedAtUtc"))
                                            .Add(Restrictions.Eq("RevenueHead", revenueHead))
                                       .Add(Restrictions.Or(Restrictions.Between("CreatedAtUtc", startDate, endDate), Restrictions.Between("DueDate", startDate, endDate)))
                                        .SetResultTransformer(Transformers.AliasToBean<StatsPerMonth>()).AddOrder(Order.Asc("DueDate")).List<StatsPerMonth>();
                #region Equivalent EQL query
                //SELECT Date as month, SUM(AmountExpected) as sum, SUM(AmountPaid) as amountpaid, SUM(NumberOfInvoicesSent) numbersent, SUM(NumberOfInvoicesPaid) numberpaid
                //FROM[CentralBillingSystem].[dbo].[ParkWay_CBS_Module_Stats]
                //WHERE RevenueHead_Id = 1004 AND Date between '2016-06-01 00:00:00.000' AND '2017-12-01 00:00:00.000'
                //GROUP BY Date order by date; 
                #endregion
            }
            catch (Exception exception)
            {
                return new List<StatsPerMonth>();
            }
        }

        public IList<StatsPerMonth> GetStatsPerQuarter(DateTime? startDate, DateTime? endDate)
        {
            var session = _transactionManager.GetSession();
            if (startDate == null || endDate == null)
            {
                startDate = new DateTime(DateTime.Now.Year, 1, 1);
                endDate = new DateTime(DateTime.Now.Year, 12, 31);
            }

            try
            {
                var th = session.CreateCriteria<Stats>()
                                        .SetProjection(
                                            Projections.ProjectionList()
                                            .Add(Projections.Sum("AmountExpected"), "AmountExpected")
                                            .Add(Projections.Sum("AmountPaid"), "AmountPaid")
                                            .Add(Projections.Sum("NumberOfInvoicesSent"), "NumberOfInvoicesSent")
                                            .Add(Projections.Sum("NumberOfInvoicesPaid"), "NumberOfInvoicesPaid")
                                             .Add(Projections.Property("CreatedAtUtc"), "CreatedAtUtc")
                                            .Add(Projections.Property("DueDate"), "DueDate")
                                           .Add(Projections.GroupProperty("DueDate"), "DueDate")
                                            .Add(Projections.GroupProperty("CreatedAtUtc"), "CreatedAtUtc")
                                        ).Add(Restrictions.Or(Restrictions.Between("CreatedAtUtc", startDate, endDate), Restrictions.Between("DueDate", startDate, endDate)))
                                        .SetResultTransformer(Transformers.AliasToBean<StatsPerMonth>()).AddOrder(Order.Asc("DueDate")).List<StatsPerMonth>();

                return session.CreateCriteria<Stats>()
                                        .SetProjection(
                                            Projections.ProjectionList()
                                            .Add(Projections.Sum("AmountExpected"), "AmountExpected")
                                            .Add(Projections.Sum("AmountPaid"), "AmountPaid")
                                            .Add(Projections.Sum("NumberOfInvoicesSent"), "NumberOfInvoicesSent")
                                            .Add(Projections.Sum("NumberOfInvoicesPaid"), "NumberOfInvoicesPaid")
                                             .Add(Projections.Property("CreatedAtUtc"), "CreatedAtUtc")
                                            .Add(Projections.Property("DueDate"), "DueDate")                                        
                                            .Add(Projections.GroupProperty("DueDate"), "DueDate")
                                            .Add(Projections.GroupProperty("CreatedAtUtc"), "CreatedAtUtc")
                                        )
                                        .Add(Restrictions.Or(Restrictions.Between("CreatedAtUtc", startDate, endDate), Restrictions.Between("DueDate", startDate, endDate)))
                                        .SetResultTransformer(Transformers.AliasToBean<StatsPerMonth>()).AddOrder(Order.Asc("DueDate")).List<StatsPerMonth>();
                #region Equivalent EQL query
                //SELECT Date as month, SUM(AmountExpected) as sum, SUM(AmountPaid) as amountpaid, SUM(NumberOfInvoicesSent) numbersent, SUM(NumberOfInvoicesPaid) numberpaid
                //FROM[CentralBillingSystem].[dbo].[ParkWay_CBS_Module_Stats]
                //WHERE RevenueHead_Id = 1004 AND Date between '2016-06-01 00:00:00.000' AND '2017-12-01 00:00:00.000'
                //GROUP BY Date order by date; 
                #endregion
            }
            catch (Exception exception)
            {
                return new List<StatsPerMonth>();
            }
        }


        public ICriteria GetCriteriaForInvoiceItems(DashboardStatsSearchParams searchParams, bool applyAccessRestrictions)
        {

            var session = _transactionManager.GetSession();
            var criteria = session.CreateCriteria<InvoiceItems>("InvoiceItems")
            .CreateAlias("InvoiceItems.Invoice", "Invoice");

            if (applyAccessRestrictions)
            {
                criteria
                .CreateAlias("InvoiceItems.Mda", "Mda")
                .CreateAlias("InvoiceItems.RevenueHead", "RevenueHead");

                var armCriteria = DetachedCriteria.For<AccessRoleMDARevenueHead>("arm")
                .Add(Restrictions.Disjunction()
                .Add(Restrictions
                    .And(Restrictions.EqProperty("MDA.Id", "InvoiceItems.Mda.Id"), Restrictions.IsNull("RevenueHead.Id")))
                .Add(Restrictions.EqProperty("RevenueHead.Id", "InvoiceItems.RevenueHead.Id")))
                .SetProjection(Projections.Constant(1));

                var aruCriteria = DetachedCriteria.For<AccessRoleUser>("aru")
                    .Add(Restrictions.Eq("User.Id", searchParams.AdminUserId))
                    .Add(Restrictions.EqProperty("AccessRole.Id", "arm.AccessRole.Id"))
                    .SetProjection(Projections.Constant(1));


                var arCriteria = DetachedCriteria.For<AccessRole>("ar")
                    .Add(Restrictions.Eq("AccessType", (int)AccessType.Dashboard))
                    .Add(Restrictions.EqProperty("Id", "aru.AccessRole.Id"))
                    .SetProjection(Projections.Constant(1));


                aruCriteria.Add(Subqueries.Exists(arCriteria));
                armCriteria.Add(Subqueries.Exists(aruCriteria));
                criteria.Add(Subqueries.Exists(armCriteria));
            }

            return criteria;
        }


        public ICriteria GetCriteria(DashboardStatsSearchParams searchParams, bool applyAccessRestrictions)
        {

            var session = _transactionManager.GetSession();
            var criteria = session.CreateCriteria<TransactionLog>("TransactionLog");
            criteria
                .CreateAlias("TransactionLog.MDA", "Mda")
                .CreateAlias("TransactionLog.RevenueHead", "RevenueHead");


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
                    .Add(Restrictions.Eq("AccessType", (int)AccessType.Dashboard))
                    .Add(Restrictions.EqProperty("Id", "aru.AccessRole.Id"))
                    .SetProjection(Projections.Constant(1));


                aruCriteria.Add(Subqueries.Exists(arCriteria));
                armCriteria.Add(Subqueries.Exists(aruCriteria));
                criteria.Add(Subqueries.Exists(armCriteria));
            }

            return criteria;
        }


        /// <summary>
        /// Get expected income on invoices that have not to been paid up until between date range
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="applyAccessRestrictions"></param>
        /// <returns>IEnumerable{DashboardStats}</returns>
        public IEnumerable<DashboardStats> GetExpectedIncomeOnInvoices(DashboardStatsSearchParams searchParams, bool applyAccessRestrictions, DateTime startDate, DateTime endDate)
        {
            return GetCriteriaForInvoiceItems(searchParams, applyAccessRestrictions).SetProjection(Projections.ProjectionList()
                    .Add(Projections.Sum<InvoiceItems>(x => (x.TotalAmount)), "TotalExpectedIncome"))
                    .Add(Restrictions.Between("Invoice.DueDate", startDate, endDate))
                    .SetResultTransformer(Transformers.AliasToBean<DashboardStats>()).Future<DashboardStats>();
        }


        /// <summary>
        /// Get the number of invoices generated in range
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="applyAccessRestrictions"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns>IEnumerable{DashboardStats}</returns>
        public IEnumerable<DashboardStats> GetNumberOfInvoicesGenerated(DashboardStatsSearchParams searchParams, bool applyAccessRestrictions, DateTime startDate, DateTime endDate)
        {
            return GetCriteriaForInvoiceItems(searchParams, applyAccessRestrictions)
                .SetProjection(Projections.ProjectionList()
                    .Add(Projections.CountDistinct<InvoiceItems>(x => (x.Invoice.Id)), "NumberOfInvoicesSent"))
                    .Add(Restrictions.Between("CreatedAtUtc", startDate, endDate))
                    .SetResultTransformer(Transformers.AliasToBean<DashboardStats>()).Future<DashboardStats>();
        }



        /// <summary>
        /// Get the summation of all invoice amounts generated for the given time frame
        /// and also the count of these invoices that were summed
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="applyAccessRestrictions"></param>
        /// <returns>IEnumerable{DashboardStats}</returns>
        public IEnumerable<DashboardStats> GetSumAndCountOfInvoicesGenerated(DashboardStatsSearchParams searchParams, bool applyAccessRestrictions, DateTime startDate, DateTime endDate)
        {
            return GetCriteria(searchParams, applyAccessRestrictions)
                .Add(Restrictions.Eq("TypeID", (int)PaymentType.Bill))
                .Add(Restrictions.Between("CreatedAtUtc", startDate, endDate))
                .SetProjection(Projections.ProjectionList()
                    .Add(Projections.Sum<TransactionLog>(x => (x.AmountPaid)), "TotalAmountOnInvoicesGenerated")
                    .Add(Projections.RowCount(), "NumberOfInvoicesSent"))
                    .SetResultTransformer(Transformers.AliasToBean<DashboardStats>()).Future<DashboardStats>();
        }


        /// <summary>
        /// Get the income received in this month range
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="applyAccessRestrictions"></param>
        /// <returns>IEnumerable{DashboardStats}</returns>
        public IEnumerable<DashboardStats> GetIncomeReceivedAndCount(DashboardStatsSearchParams searchParams, bool applyAccessRestrictions, DateTime startDate, DateTime endDate)
        {
            return GetCriteria(searchParams, applyAccessRestrictions).SetProjection(
                 Projections.ProjectionList()
                     .Add(Projections.Sum<TransactionLog>(x => (x.AmountPaid)), "ReceivedIncome")
                     .Add(Projections.CountDistinct<TransactionLog>(t => t.InvoiceNumber), "NumberOfInvoicesPaid"))
                     .Add(Restrictions.Eq("TypeID", (int)PaymentType.Credit))
                    .Add(Restrictions.Between("CreatedAtUtc", startDate, endDate))
                    .SetResultTransformer(Transformers.AliasToBean<DashboardStats>()).Future<DashboardStats>();
        }

    }

    public class StatsPerMonth
    {
        public virtual DateTime DueDate { get; set; }
        public virtual MDA Mda { get; set; }
        public virtual RevenueHead RevenueHead { get; set; }
        public virtual decimal AmountExpected { get; set; }
        public virtual decimal AmountPaid { get; set; }
        public virtual Int64 NumberOfInvoicesSent { get; set; }
        public virtual Int64 NumberOfInvoicesPaid { get; set; }

      
        public virtual DateTime CreatedAtUtc
        {
            get; set;
        }
     
    }


}
