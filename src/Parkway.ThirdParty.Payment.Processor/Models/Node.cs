using System.Xml.Serialization;

namespace Parkway.ThirdParty.Payment.Processor.Models
{
    [XmlRoot(ElementName = "Config")]
    public class Config
    {
        [XmlAttribute(AttributeName = "key")]
        public string Key { get; set; }

        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }
}