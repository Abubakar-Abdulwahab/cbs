using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Parkway.CBS.PayeeProcessor.PayeeFileProcessorCollections
{

    [XmlRoot(ElementName = "PayeeFileProcessorCollection")]
    public class PayeeFileProcessorCollection : IConfigurationSectionHandler
    {
        [XmlElement(ElementName = "PayeeFileProcessor")]
        public List<PayeeFileProcessor> PayeeFileProcessor { get; set; }

        public object Create(object parent, object configContext, XmlNode section)
        {
            return section.OuterXml;
        }
    }


    [XmlRoot(ElementName = "Tenant")]
    public class Tenant
    {
        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "Path")]
    public class Path
    {
        [XmlAttribute(AttributeName = "directorytowatch")]
        public string Directorytowatch { get; set; }
        [XmlAttribute(AttributeName = "processedpath")]
        public string Processedpath { get; set; }
        [XmlAttribute(AttributeName = "processingpath")]
        public string Processingpath { get; set; }
    }

    [XmlRoot(ElementName = "SessionFactory")]
    public class SessionFactory
    {
        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "PayeeFileProcessor")]
    public class PayeeFileProcessor
    {
        [XmlElement(ElementName = "Tenant")]
        public Tenant Tenant { get; set; }
        [XmlElement(ElementName = "Path")]
        public Path Path { get; set; }
        [XmlElement(ElementName = "SessionFactory")]
        public SessionFactory SessionFactory { get; set; }
        [XmlAttribute(AttributeName = "useDefault")]
        public bool UseDefault { get; set; }
        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }
    }
     
}
