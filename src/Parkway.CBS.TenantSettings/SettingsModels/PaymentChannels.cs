using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Parkway.CBS.TenantSettings.SettingsModels
{
    [XmlRoot(ElementName = "PaymentChannels")]
    public class PaymentChannels
    {
        [XmlElement(ElementName = "PaymentChannel")]
        public List<PaymentChannel> ListOfAvailablePaymentChannels { get; set; }
    }
}
