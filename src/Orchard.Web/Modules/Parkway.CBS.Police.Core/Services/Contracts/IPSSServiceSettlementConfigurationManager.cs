using Orchard;
using Parkway.CBS.Core.Services.Contracts;


namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPSSServiceSettlementConfigurationManager<PSSServiceSettlementConfiguration> : IDependency, IBaseManager<PSSServiceSettlementConfiguration>
    {
        void DoSeed();
    }
}
