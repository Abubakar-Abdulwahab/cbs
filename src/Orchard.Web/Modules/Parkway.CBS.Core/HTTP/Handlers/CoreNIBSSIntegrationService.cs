using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Parkway.CBS.CacheProvider;
using Parkway.EbillsPay;
using Orchard.Services;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models.Enums;
using Orchard;

namespace Parkway.CBS.Core.HTTP.Handlers
{
    public class CoreNIBSSIntegrationService: ICoreNIBSSIntegrationService
    {
        private readonly INibssIntegrationCredentialsManager<NibssIntegrationCredentials> _nibssIntegrationCredRepo;
        private readonly IOrchardServices _orchardServices;

        public CoreNIBSSIntegrationService(INibssIntegrationCredentialsManager<NibssIntegrationCredentials> nibssIntegrationCredRepo, IOrchardServices orchardServices)
        {
            _nibssIntegrationCredRepo = nibssIntegrationCredRepo;
            _orchardServices = orchardServices;
        }

        /// <summary>
        /// Gets the NIBSS IV and SecretKey
        /// </summary>
        /// <returns></returns>
        public void GetNibssIntegrationCredential(ref string IV, ref string SecretKey)
        {
            string tenant = _orchardServices.WorkContext.CurrentSite.SiteName;
            NIBSSIntegrationCredentialVM credentials = ObjectCacheProvider.GetCachedObject<NIBSSIntegrationCredentialVM>(tenant, $"{nameof(CachePrefix.NibssCredential)}");
            if (credentials == null)
            {
                credentials = _nibssIntegrationCredRepo.GetActiveNibssIntegrationCredential();
                if (credentials != null)
                {
                    ObjectCacheProvider.TryCache(tenant, $"{nameof(CachePrefix.NibssCredential)}", credentials);
                }
            }

            SecretKey = credentials?.SecretKey;
            IV = credentials?.IV;
        }
    }
}