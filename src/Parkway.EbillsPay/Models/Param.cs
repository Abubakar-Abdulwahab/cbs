using System.Xml.Serialization;

namespace Parkway.EbillsPay.Models
{
    [XmlRoot(ElementName = "Param")]
    public class Param
    {
        [XmlElement(ElementName = "Key")]
        public string Key { get; set; }

        [XmlElement(ElementName = "Value")]
        public string Value { get; set; }
    }
}