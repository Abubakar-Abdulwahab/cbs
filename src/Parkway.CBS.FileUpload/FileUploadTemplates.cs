using Newtonsoft.Json;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using System.Xml.Serialization;

namespace Parkway.CBS.FileUpload
{
    [XmlRoot(ElementName = "FileUploadTemplates")]
    public class FileUploadTemplates : IConfigurationSectionHandler
    {
        [XmlElement(ElementName = "Template")]
        public List<Template> ListOfTemplates { get; set; }

        public string SelectedTemplate { get; set; }

        public string SelectedImplementation { get; set; }

        public object Create(object parent, object configContext, XmlNode section)
        {
            return section.OuterXml;
        }
    }

    [XmlRoot(ElementName = "Template")]
    public class Template
    {
        [XmlElement(ElementName = "UploadImplInterface")]
        public List<UploadImplInterface> ListOfUploadImplementations { get; set; }

        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "View")]
        public string View { get; set; }
    }


    [XmlRoot(ElementName = "UploadImplInterface")]
    public class UploadImplInterface
    {
        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "Value")]
        public string Value { get; set; }

        [XmlAttribute(AttributeName = "ClassName")]
        public string ClassName { get; set; }

        [XmlAttribute(AttributeName = "BatchInvoiceResponseClassName")]
        public string BatchInvoiceResponseClassName { get; set; }
    }
}
