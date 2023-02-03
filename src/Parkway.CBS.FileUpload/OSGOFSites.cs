using Newtonsoft.Json;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using System.Xml.Serialization;

namespace Parkway.CBS.FileUpload
{
    [XmlRoot(ElementName = "OSGOFSites")]
    public class OSGOFSites : IConfigurationSectionHandler
    {
        [XmlElement(ElementName = "OSGOFState")]
        public List<OSGOFState> ListOfStates { get; set; }

        public object Create(object parent, object configContext, XmlNode section)
        {
            return section.OuterXml;
        }
    }


    [XmlRoot(ElementName = "OSGOFState")]
    public class OSGOFState
    {
        [XmlElement(ElementName = "lga")]
        public List<OSGOFStateLGA> ListOfLGAs { get; set; }

        [XmlAttribute(AttributeName = "Id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

    }

    [XmlRoot(ElementName = "lga")]
    public class OSGOFStateLGA
    {
        [XmlElement(ElementName = "Site")]
        public List<OSGOFCellSites> ListOfSites { get; set; }

        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "Id")]
        public string Id { get; set; }
    }


    [XmlRoot(ElementName = "Site")]
    public class OSGOFCellSites
    {
        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "Id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "Address")]
        public string Address { get; set; }

        [XmlAttribute(AttributeName = "Coor")]
        public string Coors { get; set; }

        [XmlAttribute(AttributeName = "Amount")]
        public decimal Amount { get; set; }

    }
}
