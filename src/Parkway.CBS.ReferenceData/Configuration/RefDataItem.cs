using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Parkway.CBS.ReferenceData.Configuration
{

    [XmlRoot(ElementName = "RefDataItem")]
    public class RefDataItem
    {
        [XmlElement(ElementName = "RefData")]
        public List<RefData> RefData { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
    }
}
