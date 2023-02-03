using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.Core.HTTP.Handlers
{
    public class CoreEthnicityService : ICoreEthnicityService
    {
        private readonly IEthnicityManager<Ethnicity> _repo;
        public ILogger Logger { get; set; }
        public CoreEthnicityService(IEthnicityManager<Ethnicity> repo)
        {
            _repo = repo;
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// Gets all active ethnicities
        /// </summary>
        /// <returns></returns>
        public IEnumerable<EthnicityVM> GetEthnicities()
        {
            try
            {
                return _repo.GetEthnicities();
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Checks if ehtnicity with specified id exists
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ValidateEthnicity(int id)
        {
            return _repo.Count(x => x.Id == id) > 0;
        }
    }
}