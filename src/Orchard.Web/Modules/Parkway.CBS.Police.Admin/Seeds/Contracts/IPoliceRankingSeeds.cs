using Orchard;

namespace Parkway.CBS.Police.Admin.Seeds.Contracts
{
    public interface IPoliceRankingSeeds : IDependency
    {
        void DoRanking();
    }
}
