using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Parkway.ThirdParty.Payment.Processor.Models
{
    [XmlRoot(ElementName = "PaymentConfigurations")]
    public class PaymentConfigurations : IConfigurationSectionHandler
    {
        [XmlElement(ElementName = "Client")]
        public List<Client> ListOfClients { get; set; }


        public object Create(object parent, object configContext, XmlNode section)
        {
            return section.OuterXml;
        }
    }

    [XmlRoot(ElementName = "Client")]
    public class Client
    {
        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "PayDirectConfigurations")]
        public PayDirectConfigurations PayDirectConfigurations { get; set; }
    }
}
