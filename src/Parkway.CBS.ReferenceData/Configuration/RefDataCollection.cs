using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Parkway.CBS.ReferenceData.Configuration
{

    [XmlRoot(ElementName = "RefDataCollection")]
    public class RefDataCollection : IConfigurationSectionHandler
    {
        [XmlElement(ElementName = "RefDataItem")]
        public List<RefDataItem> RefDataItem { get; set; }

        public object Create(object parent, object configContext, XmlNode section)
        {
            return section.OuterXml;
        }
    }
}