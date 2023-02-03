using System.Collections.Generic;
using System.Xml.Serialization;

namespace Parkway.ThirdParty.Payment.Processor.Models
{
    public abstract class BaseProcessorConfigurations
    {
        [XmlElement(ElementName = "Config")]
        public List<Config> ConfigNodes { get; set; }
    }
}