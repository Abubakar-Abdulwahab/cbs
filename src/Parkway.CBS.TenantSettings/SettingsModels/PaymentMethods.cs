using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Parkway.CBS.TenantSettings.SettingsModels
{
    [XmlRoot(ElementName = "PaymentMethods")]
    public class PaymentMethods : ConfigurationSection //: IConfigurationSectionHandler
    {
        [ConfigurationProperty("PaymentMethod")]
        public PaymentMethod PaymentMethod
        {
            get
            {
                return (PaymentMethod)base["PaymentMethod"];
            }
        }
        //public string Value { get; set; }
        //[XmlElement(ElementName = "PaymentMethod")]
        //public List<PaymentMethod> ListOfPaymentMethods { get; set; }

        //public object Create(object parent, object configContext, XmlNode section)
        //{
        //    return section.OuterXml;
        //}
    }
}
