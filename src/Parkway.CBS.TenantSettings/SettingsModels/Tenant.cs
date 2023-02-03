using System.Collections.Generic;

namespace Parkway.CBS.TenantSettings.SettingsModels
{
    public class Tenant
    {
        public string Name { get; set; }

        public string SiteNameOnFile { get; set; }

        public List<RegistrationRules> RegistrationRules { get; set; }
    }
}