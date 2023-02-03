using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Parkway.CBS.TenantSettings.SettingsModels
{
    public class TenantSettings : IConfigurationSectionHandler
    {
        public List<Tenant> Tenants { get; set; }

        public object Create(object parent, object configContext, XmlNode section)
        {
            return section.OuterXml;
        }
    }
}
