using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Parkway.CBS.ClientRepository.Repositories.ReferenceDataImpl.Configuration
{
    [XmlRoot(ElementName = "LGAMappingCollection")]
    public class LGAMappingCollection : IConfigurationSectionHandler
    {
        [XmlElement(ElementName = "LGACollection")]
        public List<LGACollection> LGACollection { get; set; }
        public object Create(object parent, object configContext, XmlNode section)
        {
            return section.OuterXml;
        }

    }

    [XmlRoot(ElementName = "LGACollection")]
    public class LGACollection
    {
        [XmlElement(ElementName = "LGA")]
        public List<LGA> lga { get; set; }

        [XmlAttribute(AttributeName = "TenantName")]
        public string TenantName { get; set; }
    }

    [XmlRoot(ElementName = "LGA")]
    public class LGA
    {
        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "LGADatabaseId")]
        public string LGADatabaseId { get; set; }

        [XmlAttribute(AttributeName = "LGAFileId")]
        public string LGAFileId { get; set; }
    }
}
