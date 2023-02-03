using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Parkway.CBS.WindowService.Configuration
{
    [XmlRoot(ElementName = "FileWatcherDirectoryCollection")]
    public class FileWatcherDirectoryCollection : IConfigurationSectionHandler
    {
        [XmlElement(ElementName = "FileProcessor")]
        public List<FileProcessor> FileProcessor { get; set; }

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

        [XmlAttribute(AttributeName = "StateId")]
        public string StateId { get; set; }

        /// <summary>
        /// used for invoice generation. this field contains the Id of the tax entity
        /// we map federal agencies to when an existing tax profile doesnot exists for
        /// agency
        /// </summary>
        [XmlAttribute(AttributeName = "UnknownTaxPayerCodeId")]
        public string UnknownTaxPayerCodeId { get; set; }
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

        [XmlAttribute(AttributeName = "summarypath")]
        public string Summarypath { get; set; }

        [XmlAttribute(AttributeName = "processedCSVpath")]
        public string ProcessCSVPath { get; set; }
    }

    [XmlRoot(ElementName = "FileProcessor")]
    public class FileProcessor
    {
        [XmlElement(ElementName = "Tenant")]
        public Tenant Tenant { get; set; }

        [XmlElement(ElementName = "Path")]
        public Path Path { get; set; }

        [XmlElement(ElementName = "ImplementationClass")]
        public ImplementationClass ImplementationClass { get; set; }

        [XmlAttribute(AttributeName = "IsActive")]
        public bool IsActive { get; set; }

        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "ProcessorType")]
        public string ProcessorType { get; set; }
    }

    [XmlRoot(ElementName = "ImplementationClass")]
    public class ImplementationClass
    {
        [XmlAttribute(AttributeName = "ClassName")]
        public string ClassName { get; set; }
    }
}
