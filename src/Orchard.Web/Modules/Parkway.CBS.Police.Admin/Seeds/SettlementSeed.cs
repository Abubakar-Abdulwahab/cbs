using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Admin.Seeds.Contracts;
using Parkway.CBS.Police.Core.Services.Contracts;
using System.Collections.Generic;
using System.Linq;
using System;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Admin.Seeds
{
    public class SettlementSeed : ISettlementsSeed
    {
        private readonly IPSSServiceSettlementConfigurationManager<PSSServiceSettlementConfiguration> _repo;

        public SettlementSeed(IPSSServiceSettlementConfigurationManager<PSSServiceSettlementConfiguration> repo)
        {
            _repo = repo;
        }


        public void SeedSettlementConfig()
        {
            _repo.DoSeed();
        }


        public void SeedSettlementConfig1(int settlementId, int mdaId, int serviceId, string revenueHeads)
        {
            string[] revsSplit = revenueHeads.Split(',');
            IEnumerable<int> revs = revsSplit.AsEnumerable().Select(x => Convert.ToInt32(x));

            List<Tuple<PaymentProvider, PaymentChannel>> providers = new List<Tuple<PaymentProvider, PaymentChannel>>
            {
                new Tuple<PaymentProvider, PaymentChannel>( PaymentProvider.Bank3D, PaymentChannel.Web),
                new Tuple<PaymentProvider, PaymentChannel>( PaymentProvider.Readycash, PaymentChannel.MOB),
                new Tuple<PaymentProvider, PaymentChannel>( PaymentProvider.Readycash, PaymentChannel.AgencyBanking),
                new Tuple<PaymentProvider, PaymentChannel>( PaymentProvider.RemitaSingleProduct, PaymentChannel.BankBranch),
                new Tuple<PaymentProvider, PaymentChannel>( PaymentProvider.RemitaSingleProduct, PaymentChannel.Web),
                new Tuple<PaymentProvider, PaymentChannel>( PaymentProvider.RemitaSingleProduct, PaymentChannel.OtherChannels),
            };

            List<PSSServiceSettlementConfiguration> configs = new List<PSSServiceSettlementConfiguration> { };

            foreach (var rev in revs)
            {
                foreach (var provider in providers)
                {
                    configs.Add(new PSSServiceSettlementConfiguration
                    {
                        Channel = (int)provider.Item2,
                        MDA = new MDA { Id = mdaId },
                        RevenueHead = new RevenueHead { Id = rev },
                        Service = new PSService { Id = serviceId },
                        PaymentProvider = new ExternalPaymentProvider { Id = (int)provider.Item1 },
                        Settlement = new PSSSettlement { Id = settlementId },
                        IsActive = true,
                    });
                }
            }

            if (!_repo.SaveBundle(configs)) { throw new Exception("Error seeding records"); }
        }


    }
}