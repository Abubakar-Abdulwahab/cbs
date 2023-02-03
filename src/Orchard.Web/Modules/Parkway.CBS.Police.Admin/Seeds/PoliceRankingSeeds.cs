using Parkway.CBS.Police.Admin.Seeds.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Admin.Seeds
{
    public class PoliceRankingSeeds : IPoliceRankingSeeds
    {
        private readonly IPoliceRankingManager<PoliceRanking> _repo;

        public PoliceRankingSeeds(IPoliceRankingManager<PoliceRanking> repo)
        {
            _repo = repo;
        }


        public void DoRanking()
        {
            List<PoliceRanking> ranks = new List<PoliceRanking>
            {
                { new PoliceRanking { RankLevel = 1, RankName = "Inspector-General of Police", IsActive = true } },
                { new PoliceRanking { RankLevel = 2, RankName = "Deputy Inspector-General of Police", IsActive = true } },
                { new PoliceRanking { RankLevel = 3, RankName = "Assistant Inspector-General of Police", IsActive = true} },
                { new PoliceRanking { RankLevel = 4, RankName = "Commissioner of Police", IsActive = true } },
                { new PoliceRanking { RankLevel = 5, RankName = "Deputy Commissioner of Police", IsActive = true } },
                { new PoliceRanking { RankLevel = 6, RankName = "Assistant Commissioner of Police", IsActive = true } },
                { new PoliceRanking { RankLevel = 7, RankName = "Chief Superintendent of Police", IsActive = true } },
                { new PoliceRanking { RankLevel = 8, RankName = "Superintendent of Police", IsActive = true } },
                { new PoliceRanking { RankLevel = 9, RankName = "Deputy Superintendent of Police", IsActive = true } },
                { new PoliceRanking { RankLevel = 10, RankName = "Assistant Superintendent of Police", IsActive = true } },
                { new PoliceRanking { RankLevel = 11, RankName = "Inspector of Police", IsActive = true } },
                { new PoliceRanking { RankLevel = 12, RankName = "Sergeant Major", IsActive = true } },
                { new PoliceRanking { RankLevel = 13, RankName = "Sergeant", IsActive = true } },
                { new PoliceRanking { RankLevel = 14, RankName = "Corporal", IsActive = true } },
                { new PoliceRanking { RankLevel = 15, RankName = "Lance Corporal", IsActive = true } },
            };

            if (!_repo.SaveBundle(ranks)) { throw new Exception { }; }
        }
    }
}