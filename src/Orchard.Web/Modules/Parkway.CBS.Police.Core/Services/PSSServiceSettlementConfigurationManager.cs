using Orchard;
using System.Linq;
using Orchard.Data;
using Orchard.Logging;
using NHibernate.Linq;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using System.Collections.Generic;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Core.Models;
using System;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Police.Core.DTO;
using System.Collections;

namespace Parkway.CBS.Police.Core.Services
{
    public class DistinctServiceRevenue : IEqualityComparer<PSServiceRevenueHeadVM>
    {

        public bool Equals(PSServiceRevenueHeadVM x, PSServiceRevenueHeadVM y)
        {
            return ((x.ServiceId == y.ServiceId) && (x.RevenueHeadId == y.RevenueHeadId));
        }

        public int GetHashCode(PSServiceRevenueHeadVM obj)
        {
            return "S".GetHashCode() ^ obj.ServiceId ^ "R".GetHashCode() ^ obj.RevenueHeadId;
        }
    }


    public class PSSServiceSettlementConfigurationManager : BaseManager<PSSServiceSettlementConfiguration>, IPSSServiceSettlementConfigurationManager<PSSServiceSettlementConfiguration>
    {
        private readonly IRepository<PSSServiceSettlementConfiguration> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public ILogger Logger { get; set; }


        public PSSServiceSettlementConfigurationManager(IRepository<PSSServiceSettlementConfiguration> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
            _user = user;
        }

        public void DoSeed()
        {
            List<PSSServiceSettlementConfiguration> f = new List<PSSServiceSettlementConfiguration> { };
            PSSServiceSettlementConfiguration fg = null;
            Dictionary<string, string> l = new Dictionary<string, string>{ };

            try
            {
                KeyValuePair<int, int> m = new KeyValuePair<int, int>(2, 3);

                List<KeyValuePair<int, int>> settlementIds = _transactionManager.GetSession().Query<PSSSettlement>().Where(s => s.IsActive).Select(s => new KeyValuePair<int, int>(s.Id,s.Service.Id)).ToList();

                List<PSServiceRevenueHeadVM> srIds = _transactionManager.GetSession().Query<PSServiceRevenueHead>().Where(s => s.IsActive)
                    .Select(s => new PSServiceRevenueHeadVM
                    {
                        MDAId = s.RevenueHead.Mda.Id,
                        RevenueHeadId = s.RevenueHead.Id,
                        ServiceId = s.Service.Id,
                    }).ToList();

                var dsd = srIds.GroupBy(x => new { x.ServiceId, x.RevenueHeadId }).SelectMany(x => x);
                srIds = srIds.Distinct(new DistinctServiceRevenue()).ToList();
                var dsdd = srIds.Distinct(new DistinctServiceRevenue()).ToDictionary(x => (x.ServiceId.ToString() + x.RevenueHeadId.ToString() )).ToList();

                //we have gotten the list of services
                //now lets get the list of payment providers
                List<int> paymentProvidersIds = _transactionManager.GetSession().Query<ExternalPaymentProvider>().Where(s => s.IsActive).Select(s => s.Id).ToList();
                List<int> channelsIds = Enum.GetValues(typeof(PaymentChannel)).Cast<PaymentChannel>().Select(x => (int)x).ToList();
               

                foreach (var settlement in settlementIds)
                {
                    foreach (var provider in paymentProvidersIds)
                    {
                        foreach (var channel in channelsIds)
                        {
                            //foreach (var item in dsd)
                            foreach (var item in dsdd)
                            {
                                if(settlement.Key != item.Value.ServiceId) { continue; }

                                if (l.ContainsKey($"S {item.Value.ServiceId} M {item.Value.MDAId} R {item.Value.RevenueHeadId} P {provider} C {channel}"))
                                {
                                    continue;
                                }
                                l.Add($"S {item.Value.ServiceId} M {item.Value.MDAId} R {item.Value.RevenueHeadId} P {provider} C {channel}", 
                                    $"S {item.Value.ServiceId} M {item.Value.MDAId} R {item.Value.RevenueHeadId} P {provider} C {channel}");
                                _transactionManager.GetSession().Save(new PSSServiceSettlementConfiguration
                                {
                                    Channel = channel,
                                    MDA = new MDA { Id = item.Value.MDAId },
                                    RevenueHead = new RevenueHead { Id = item.Value.RevenueHeadId },
                                    Service = new PSService { Id = item.Value.ServiceId },
                                    PaymentProvider = new ExternalPaymentProvider { Id = provider },
                                    Settlement = new PSSSettlement { Id = settlement.Key },
                                    IsActive = true,
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                _transactionManager.GetSession().Transaction.Rollback();
                throw;
            }
        }

    }
}