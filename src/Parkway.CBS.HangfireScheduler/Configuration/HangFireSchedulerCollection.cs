using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using System.Xml.Serialization;

namespace Parkway.CBS.HangfireScheduler.Configuration
{
    public class HangFireSchedulerCollection : IConfigurationSectionHandler
    {
        [XmlElement(ElementName = "HangFireSchedulerItem")]
        public List<HangFireSchedulerItem> HangFireSchedulerItem { get; set; }

        public object Create(object parent, object configContext, XmlNode section)
        {
            return section.OuterXml;
        }

    }


    [XmlRoot(ElementName = "HangFireSchedulerItem")]
    public class HangFireSchedulerItem
    {
        [XmlAttribute(AttributeName = "IsActive")]
        public bool IsActive { get; set; }

        [XmlAttribute(AttributeName = "TenantName")]
        public string TenantName { get; set; }

        [XmlAttribute(AttributeName = "ConnectionString")]
        public string ConnectionString { get; set; }

        [XmlAttribute(AttributeName = "DashboardUrl")]
        public string DashboardUrl { get; set; }
    }
}
