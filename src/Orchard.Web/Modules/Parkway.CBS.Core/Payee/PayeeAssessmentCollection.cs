using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using System.Xml.Serialization;

//http://xmltocsharp.azurewebsites.net/
namespace Parkway.CBS.Core.Payee
{
    [XmlRoot(ElementName = "PayeeAssessmentCollection")]
    public class PayeeAssessmentCollection : IConfigurationSectionHandler
    {
        [XmlElement(ElementName = "AssessmentInterfaceItem")]
        public List<AssessmentInterfaceItem> AssessmentInterfaceItem { get; set; }

        public object Create(object parent, object configContext, XmlNode section)
        {
            return section.OuterXml;
        }
    }


    [XmlRoot(ElementName = "AssessmentInterfaceItem")]
    public class AssessmentInterfaceItem
    {
        [XmlElement(ElementName = "AssessmentInterface")]
        public List<AssessmentInterface> AssessmentInterface { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
    }


    [XmlRoot(ElementName = "AssessmentInterface")]
    public class AssessmentInterface
    {
        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "Value")]
        public string Value { get; set; }

        [XmlAttribute(AttributeName = "IsActive", DataType = "boolean")]
        public Boolean IsActive { get; set; }

        [XmlAttribute(AttributeName = "ClassName")]
        public string ClassName { get; set; }

        public string StateName { get; set; }
    }    
}
