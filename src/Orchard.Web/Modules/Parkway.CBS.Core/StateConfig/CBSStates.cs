using System.Xml;
using System.Configuration;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Parkway.CBS.Core.StateConfig
{

    [XmlRoot(ElementName = "CBSStates")]
    public class CBSStates : IConfigurationSectionHandler
    {
        [XmlElement(ElementName = "StateConfig")]
        public List<StateConfig> StateConfig { get; set; }

        public object Create(object parent, object configContext, XmlNode section)
        {
            return section.OuterXml;
        }
    }

    [XmlRoot(ElementName = "StateConfig")]
    public class StateConfig
    {
        [XmlElement(ElementName = "node")]
        public List<Node> Node { get; set; }
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }

        [XmlAttribute(AttributeName = "vendorCode")]
        public string CashflowVendorCode { get; set; }

        [XmlAttribute(AttributeName = "fileSiteName")]
        public string FileSiteName { get; set; }
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