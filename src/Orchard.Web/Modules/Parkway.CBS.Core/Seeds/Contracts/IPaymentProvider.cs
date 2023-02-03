using Orchard;

namespace Parkway.CBS.Core.Seeds.Contracts
{
    public interface IPaymentProviderSeeds : IDependency
    {
        void PopPaymentProviders();
    }
}
