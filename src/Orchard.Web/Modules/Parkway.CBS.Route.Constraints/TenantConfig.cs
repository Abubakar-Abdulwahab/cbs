using System.Linq;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Core.Models.Enums;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Parkway.CBS.Core.StateConfig;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.Route.Constraints
{
    public static class TenantConfig
    {
        private static Dictionary<string, string> tenantConfigs = LoadConfigs();

        public static string GetTenantConfig(string siteName)
        {
            return tenantConfigs[siteName];
        }

        private static Dictionary<string, string> LoadConfigs()
        {
            if (tenantConfigs == null || tenantConfigs.Count == 0)
            {
                tenantConfigs = new Dictionary<string, string>();
                var stateConfigs = Util.StateConfig().StateConfig;
                foreach (var stateCon in stateConfigs)
                {
                    Node setting = stateCon.Node.Where(k => k.Key == TenantConfigKeys.AdminBaseURL.ToString()).FirstOrDefault();
                    if (setting != null && !string.IsNullOrEmpty(setting.Value))
                    {
                        tenantConfigs.Add(stateCon.Value, setting.Value);
                    }
                };
            }

            return tenantConfigs;
        }
    }
}