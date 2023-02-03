using Orchard;

namespace Parkway.CBS.Police.Admin.Seeds.Contracts
{
    public interface ISettlementsSeed : IDependency
    {
        void SeedSettlementConfig();
        void SeedSettlementConfig1(int settlementId, int mdaId, int serviceId, string revenueHeads);
    }
}
