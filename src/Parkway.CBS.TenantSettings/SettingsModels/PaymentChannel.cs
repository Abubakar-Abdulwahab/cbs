using System.Xml.Serialization;

namespace Parkway.CBS.TenantSettings.SettingsModels
{
    [XmlRoot(ElementName = "PaymentChannel")]
    public class PaymentChannel
    {
        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "Id", DataType = "Integer")]
        public int Id { get; set; }

        [XmlAttribute(AttributeName = "Description")]
        public string Description { get; set; }
    }
}