using Orchard.Logging;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using System;

namespace Parkway.CBS.Police.Core.CoreServices
{
    public class CoreExtractDetails : ICoreExtractDetails
    {
        private readonly IExtractDetailsManager<ExtractDetails> _extractDetailManager;
        public ILogger Logger { get; set; }

        public CoreExtractDetails(IExtractDetailsManager<ExtractDetails> extractDetailManager)
        {
            _extractDetailManager = extractDetailManager;
            Logger = NullLogger.Instance;

        }

        /// <summary>
        /// Check if <paramref name="affivdavitNumber"/> does not exist with the user 
        /// with the <paramref name="taxEntityId"/>
        /// </summary>
        /// <param name="affivdavitNumber"></param>
        /// <param name="taxEntityId"></param>
        /// <returns></returns>
        public bool CheckIfExistingAffidavitNumber(string affivdavitNumber, long taxEntityId)
        {
            try
            {
                return _extractDetailManager.CheckIfExistingAffidavitNumber(affivdavitNumber, taxEntityId);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Error checking if affidavit exist" + affivdavitNumber));
                throw;
            }
        }
    }
}