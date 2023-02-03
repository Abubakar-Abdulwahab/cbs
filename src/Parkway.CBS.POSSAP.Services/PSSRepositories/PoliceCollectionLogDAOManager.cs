using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.POSSAP.Services.HelperModel;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories
{
    public class PoliceCollectionLogDAOManager : Repository<PoliceCollectionLog>, IPoliceCollectionLogDAOManager
    {
        public PoliceCollectionLogDAOManager(IUoW uow) : base(uow)
        { }

        /// <summary>
        /// Get collection log count
        /// </summary>
        /// <param name="pssServiceSettlement"></param>
        /// <param name="settlementRuleVM"></param>
        /// <returns>IEnumerable<PoliceCollectionLogReportStatsVM></returns>
        public IEnumerable<PoliceCollectionLogReportStatsVM> GetCollectionLogCount(PSSServiceSettlementConfigurationVM pssServiceSettlement, PSSSettlementRuleVM settlementRuleVM)
        {
            var query = GetCriteria(pssServiceSettlement, settlementRuleVM);

            return query
            .SetProjection(
                        Projections.ProjectionList()
                            .Add(Projections.Count<PoliceCollectionLog>(x => x.Id), "TotalRecordCount")
                    ).SetResultTransformer(Transformers.AliasToBean<PoliceCollectionLogReportStatsVM>()).Future<PoliceCollectionLogReportStatsVM>();
        }

        /// <summary>
        /// Get paginated collection logs
        /// </summary>
        /// <param name="pssServiceSettlement"></param>
        /// <param name="settlementRuleVM"></param>
        /// <returns>IEnumerable<PoliceCollectionLogVM></returns>
        public List<PoliceCollectionLogVM> GetPagedCollectionLogs(PSSServiceSettlementConfigurationVM pssServiceSettlement, PSSSettlementRuleVM settlementRuleVM, int take, int skip)
        {
            var query = GetCriteria(pssServiceSettlement, settlementRuleVM).SetFirstResult(skip).SetMaxResults(take);

            return query
                .SetProjection(Projections.ProjectionList()
                .Add(Projections.Property("tl.AmountPaid"), nameof(PoliceCollectionLogVM.AmountPaid))
                .Add(Projections.Property("rh.Name"), nameof(PoliceCollectionLogVM.RevenueHeadName))
                .Add(Projections.Property("pr.Id"), nameof(PoliceCollectionLogVM.RequestId))
                .Add(Projections.Property("tl.Invoice.Id"), nameof(PoliceCollectionLogVM.InvoiceId))
                ).SetResultTransformer(Transformers.AliasToBean<PoliceCollectionLogVM>()).Future<PoliceCollectionLogVM>().ToList();
        }


        public ICriteria GetCriteria(PSSServiceSettlementConfigurationVM pssServiceSettlement, PSSSettlementRuleVM settlementRuleVM)
        {
            var criteria = _uow.Session.CreateCriteria<PoliceCollectionLog>("pcl");
            criteria
                .CreateAlias("pcl.TransactionLog", "tl")
                .CreateAlias("pcl.Request", "pr")
                .CreateAlias("tl.RevenueHead", "rh")
                .CreateAlias("tl.MDA", "mda")
                .CreateAlias("pr.Service", "ps")
                .Add(Restrictions.EqProperty("pcl.TransactionLog.Id", "tl.Id"))
                .Add(Restrictions.EqProperty("pcl.Request.Id", "pr.Id"))
                .Add(Restrictions.EqProperty("pr.Service.Id", "ps.Id"))
                .Add(Restrictions.EqProperty("tl.RevenueHead.Id", "rh.Id"))
                .Add(Restrictions.Eq("mda.Id", pssServiceSettlement.MDAId))
                .Add(Restrictions.Eq("rh.Id", pssServiceSettlement.RevenueHeadId))
                .Add(Restrictions.Eq("tl.PaymentProvider", pssServiceSettlement.PaymentProviderId))
                .Add(Restrictions.Eq("tl.Channel", pssServiceSettlement.Channel))
                .Add(Restrictions.Eq("tl.Settled", false))
                .Add(Restrictions.Eq("ps.Id", pssServiceSettlement.ServiceId))
                .Add(Restrictions.Between("tl.CreatedAtUtc", settlementRuleVM.NextScheduleDate, settlementRuleVM.NextScheduleDate.AddDays(1).AddMilliseconds(-1)));

            var serviceRevenueHeadCriteria = DetachedCriteria.For<PSServiceRevenueHead>("prh")
                .CreateAlias("prh.FlowDefinitionLevel", "fdl")
                .Add(Restrictions.Eq("fdl.Id", pssServiceSettlement.DefinitionLevelId))
                .SetProjection(Projections.Constant(1));

            criteria.Add(Subqueries.Exists(serviceRevenueHeadCriteria));

            return criteria;
        }

        /// <summary>
        /// Split the cost of service from the original amount
        /// </summary>
        /// <param name="pssPresettlementDeduction"></param>
        /// <param name="collectionLogVMs"></param>
        /// <returns></returns>
        public ConcurrentQueue<PoliceCollectionLogVM> ComputeCostofService(PSSPresettlementDeductionConfigurationVM pssPresettlementDeduction, List<PoliceCollectionLogVM> collectionLogVMs)
        {
            try
            {
                ConcurrentQueue<PoliceCollectionLogVM> computeCollectionLogVMs = new ConcurrentQueue<PoliceCollectionLogVM>();

                if (collectionLogVMs != null && collectionLogVMs.Count > 0)
                {
                    Parallel.ForEach(collectionLogVMs, item =>
                    {
                        if(pssPresettlementDeduction.DeductionShareTypeId == (int)Police.Core.Models.Enums.DeductionShareType.Percentage)
                        {
                            var collectionLogCostofService = new PoliceCollectionLogVM()
                            {
                                AmountPaid = Math.Round((pssPresettlementDeduction.PercentageShare / 100 * item.AmountPaid), 2),
                                InvoiceId = item.InvoiceId,
                                RequestId = item.RequestId,
                                RevenueHeadName = "COST OF SERVICE FOR " + item.RevenueHeadName.ToUpper(),
                                IsDeduction = true
                            };
                            computeCollectionLogVMs.Enqueue(collectionLogCostofService);

                            //Balance after cost of service
                            var collectionLog = new PoliceCollectionLogVM()
                            {
                                AmountPaid = item.AmountPaid - Math.Round((pssPresettlementDeduction.PercentageShare / 100 * item.AmountPaid), 2),
                                InvoiceId = item.InvoiceId,
                                RequestId = item.RequestId,
                                RevenueHeadName = item.RevenueHeadName
                            };
                            computeCollectionLogVMs.Enqueue(collectionLog);

                        }else if (pssPresettlementDeduction.DeductionShareTypeId == (int)Police.Core.Models.Enums.DeductionShareType.Flat)
                        {
                            var collectionLogCostofService = new PoliceCollectionLogVM()
                            {
                                AmountPaid = pssPresettlementDeduction.FlatShare,
                                InvoiceId = item.InvoiceId,
                                RequestId = item.RequestId,
                                RevenueHeadName = item.RevenueHeadName,
                                IsDeduction = true
                            };
                            computeCollectionLogVMs.Enqueue(collectionLogCostofService);

                            //Balance after cost of service
                            var collectionLog = new PoliceCollectionLogVM()
                            {
                                AmountPaid = item.AmountPaid - pssPresettlementDeduction.FlatShare,
                                InvoiceId = item.InvoiceId,
                                RequestId = item.RequestId,
                                RevenueHeadName = item.RevenueHeadName
                            };
                            computeCollectionLogVMs.Enqueue(collectionLog);
                        }

                    });
                }

                return computeCollectionLogVMs;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
