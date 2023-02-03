using Orchard.Logging;
using Parkway.CBS.Police.Admin.Seeds.Contracts;
using System;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Admin.Controllers
{
    [Authorize]
    public class PoliceSeedsController : Controller
    {
        public ILogger Logger { get; set; }
        private readonly IPoliceRankingSeeds _rankingSeeds;

        public PoliceSeedsController(IPoliceRankingSeeds rankingSeeds)
        {
            _rankingSeeds = rankingSeeds;
        }

        // GET: Seeds
        public string PoliceRanking()
        {
            try
            {
                _rankingSeeds.DoRanking();
                return "Ok";
            }
            catch (Exception)
            {
                return "Error occurred";
            }
        }
    }
}