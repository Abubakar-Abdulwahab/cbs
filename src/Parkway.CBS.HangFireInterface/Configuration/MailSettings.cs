using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Parkway.CBS.HangFireInterface.Configuration
{
    [XmlRoot(ElementName = "MailSettings")]
    public class MailSettings : IConfigurationSectionHandler
    {
        [XmlElement(ElementName = "Config")]
        public List<Config> Config { get; set; }

        public object Create(object parent, object configContext, XmlNode section)
        {
            return section.OuterXml;
        }
    }

    [XmlRoot(ElementName = "Config")]
    public class Config
    {
        [XmlElement(ElementName = "node")]
        public List<Node> Node { get; set; }

        [XmlAttribute(AttributeName = "provider")]
        public string Provider { get; set; }
    }

    [XmlRoot(ElementName = "node")]
    public class Node
    {
        [XmlAttribute(AttributeName = "key")]
        public string Key { get; set; }

        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }
}
