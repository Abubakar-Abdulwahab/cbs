using System.Collections.Generic;

namespace Parkway.CBS.TenantSettings.SettingsModels
{
    public class RegistrationRules
    {
        public List<Rule> Rules { get; set; }
    }

    public class Rule
    {
        public string Name { get; set; }

        public string AllowNull { get; set; }

        public string IsUnique { get; set; }
    }
}