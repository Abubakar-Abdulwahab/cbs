using Orchard.Localization;
using Orchard.Logging;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Seeds.Contracts;
using Parkway.CBS.Core.Services.Contracts;
using System.Collections.Generic;

namespace Parkway.CBS.Core.Seeds
{
    public class SectorSeeds : ISectorSeeds
    {
        ITaxEntityCategoryManager<TaxEntityCategory> _repository;
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public SectorSeeds(ITaxEntityCategoryManager<TaxEntityCategory> repository)
        {
            _repository = repository;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public void SeedSectors()
        {
            List<TaxEntityCategory> sectors = new List<TaxEntityCategory>();
            sectors.Add(new TaxEntityCategory { Name = "Individual", Status = true, Identifier = 0 });
            sectors.Add(new TaxEntityCategory { Name = "Corporate", Status = true, Identifier = 1 });
            sectors.Add(new TaxEntityCategory { Name = "Dealer", Status = false, Identifier = 2 });

            if (_repository.SaveBundle(sectors)) { Logger.Error("Could not seed sector table"); }
        }
    }
}