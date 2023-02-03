using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Models;
using Parkway.CBS.ReferenceData.Admin.CoreServices.Contracts;
using Parkway.CBS.ReferenceData.Admin.Services.Contracts;
using System.Collections.Generic;

namespace Parkway.CBS.ReferenceData.Admin.CoreServices
{
    public class CoreLGA : ICoreLGA
    {
        private readonly IReferenceDataLGAManager<LGA> _lgaManager;
        private readonly ITransactionManager _transactionManager;
        private readonly IOrchardServices _orchardServices;
        public ILogger Logger { get; set; }

        public CoreLGA(IOrchardServices orchardService, IReferenceDataLGAManager<LGA> lgaManager)
        {
            _lgaManager = lgaManager;
            _transactionManager = orchardService.TransactionManager;
            Logger = NullLogger.Instance;
            _orchardServices = orchardService;
        }

        public IEnumerable<LGA> GetLGAs(int stateId)
        {
            var lgas = _lgaManager.GetLGAs(stateId);
            if (lgas == null) { throw new NoRecordFoundException("No record found "); }
            return lgas;
        }


        /// <summary>
        /// Get LGA by Id
        /// </summary>
        /// <param name="lGAId"></param>
        /// <returns>LGA</returns>
        /// <exception cref="NoRecordFoundException"></exception>
        public LGA GetLGA(int id)
        {
            var lga = _lgaManager.Get(id);
            if(lga == null) { throw new NoRecordFoundException("Could not find the given LGA Id " + id); }
            return lga;
        }
    }
}