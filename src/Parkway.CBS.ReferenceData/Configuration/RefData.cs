using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Parkway.CBS.ReferenceData.Configuration
{
    [XmlRoot(ElementName = "RefData")]
    public class RefData
    {
        [XmlElement(ElementName = "Endpoint")]
        public List<Endpoint> Endpoint { get; set; }

        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "Type")]
        public string Type { get; set; }

        [XmlAttribute(AttributeName = "ClassName")]
        public string ClassName { get; set; }
    }
}
