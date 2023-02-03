using Parkway.CBS.Police.Admin.Seeds.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Admin.Seeds
{
    public class PSSReasonSeeds : IPSSReasonSeeds
    {
        private readonly IPSSReasonManager<PSSReason> _pssReasonRepo;

        public PSSReasonSeeds(IPSSReasonManager<PSSReason> pssReasonRepo)
        {
            _pssReasonRepo = pssReasonRepo;
        }

        public void AddPSSReason()
        {
            List<PSSReason> reasons = new List<PSSReason> {
                { new PSSReason{ Name = "Orderly" } },
                { new PSSReason{ Name = "Escort" } },
                { new PSSReason{ Name = "Residential Property" } },
                { new PSSReason { Name = "Commercial Property" } },
                { new PSSReason { Name = "Event" } }
            };
            if (!_pssReasonRepo.SaveBundle(reasons))
            {
                throw new Exception { };
            }
        }
    }
}