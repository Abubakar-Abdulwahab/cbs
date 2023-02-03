using Orchard;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.CacheProvider;
using Parkway.CBS.Police.Core.Models.Enums;
using System.Linq;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Police.Core.CoreServices
{
    public class CoreServiceOptions : ICoreServiceOptions
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IPSSCharacterCertificateOptionsManager<PSServiceOptions> _serviceOptionsManager;

        public CoreServiceOptions(IOrchardServices orchardServices, IPSSCharacterCertificateOptionsManager<PSServiceOptions> serviceOptionsManager)
        {
            _orchardServices = orchardServices;
            _serviceOptionsManager = serviceOptionsManager;
        }


        /// <summary>
        /// Get service options
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns>IEnumerable{PSSServiceOptionsVM}</returns>
        public IEnumerable<PSServiceOptionsVM> GetActiveOtpions(int serviceId)
        {
            string tenant = _orchardServices.WorkContext.CurrentSite.SiteName;
            IEnumerable<PSServiceOptionsVM> options = ObjectCacheProvider.GetCachedObject<IEnumerable<PSServiceOptionsVM>>(tenant, $"{nameof(POSSAPCachePrefix.ServiceOptions)}-{serviceId}");

            if (options == null || !options.Any())
            {
                options = _serviceOptionsManager.GetActiveCharacterCertificateOtpions(serviceId);

                if (options != null && options.Any())
                {
                    ObjectCacheProvider.TryCache(tenant, $"{nameof(POSSAPCachePrefix.ServiceOptions)}-{serviceId}", options);
                }
            }
            return options;
        }


        /// <summary>
        /// Check if this s
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="selectedOption"></param>
        /// <returns>PSSServiceOptionsVM</returns>
        public PSServiceOptionsVM GetActiveServiceOption(int serviceId, int optionId)
        {
            string tenant = _orchardServices.WorkContext.CurrentSite.SiteName;
            PSServiceOptionsVM option = ObjectCacheProvider.GetCachedObject<PSServiceOptionsVM>(tenant, $"{nameof(POSSAPCachePrefix.ServiceOptionValue)}-{serviceId}-{optionId}");

            if (option == null)
            {
                option = _serviceOptionsManager.GetServiceOption(serviceId, optionId);

                if (option != null)
                {
                    ObjectCacheProvider.TryCache(tenant, $"{nameof(POSSAPCachePrefix.ServiceOptionValue)}-{serviceId}-{optionId}", option);
                }
            }
            return option;
        }

    }
}