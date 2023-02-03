using NHibernate.Linq;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.POSSAP.Services.HelperModel;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;
using System.Linq;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories
{
    public class PSSPresettlementDeductionConfigurationDAOManager : Repository<PSSPresettlementDeductionConfiguration>, IPSSPresettlementDeductionConfigurationDAOManager
    {
        public PSSPresettlementDeductionConfigurationDAOManager(IUoW uow) : base(uow)
        { }

        /// <summary>
        /// Get settlement presettled cost of service deductions
        /// </summary>
        /// <param name="settlementRuleId"></param>
        /// <param name="revenueHeadId"></param>
        /// <param name="serviceId"></param>
        /// <param name="paymentProviderId"></param>
        /// <param name="definitionLevelId"></param>
        /// <returns>PSSPresettlementDeductionConfigurationVM</returns>
        public PSSPresettlementDeductionConfigurationVM GetPresettlementDeductionConfiguration(int settlementRuleId, int revenueHeadId, int serviceId, int paymentProviderId, int definitionLevelId)
        {
            return _uow.Session.Query<PSSPresettlementDeductionConfiguration>()
                .Where(x => x.SettlementRule == new SettlementRule { Id = settlementRuleId } && x.RevenueHead == new RevenueHead { Id = revenueHeadId } && x.Service == new PSService { Id = serviceId } && x.PaymentProvider == new ExternalPaymentProvider { Id = paymentProviderId} && x.DefinitionLevel == new PSServiceRequestFlowDefinitionLevel { Id = definitionLevelId})
                .Select(x => new PSSPresettlementDeductionConfigurationVM
                {
                    SettlemntRuleId = x.SettlementRule.Id,
                    DefinitionLevelId = x.DefinitionLevel.Id,
                    PaymentProviderId = x.PaymentProvider.Id,
                    PaymentProviderName = x.PaymentProvider.Name,
                    ServiceId = x.Service.Id,
                    RevenueHeadId = x.RevenueHead.Id,
                    MDAId = x.MDA.Id,
                    Channel = x.Channel,
                    Name = x.Name,
                    ImplementClass = x.ImplementClass,
                    DeductionShareTypeId = x.DeductionShareTypeId,
                    PercentageShare = x.PercentageShare,
                    FlatShare = x.FlatShare
                }).SingleOrDefault();
        }
    }
}
