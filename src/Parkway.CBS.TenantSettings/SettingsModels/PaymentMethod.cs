using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml.Serialization;

namespace Parkway.CBS.TenantSettings.SettingsModels
{
    //[XmlRoot(ElementName = "PaymentMethod")]
    public class PaymentMethod : ConfigurationElement
    {
        //[XmlAttribute(AttributeName = "Name")]
        [ConfigurationProperty("Name")]
        public string Name
        {
            get {
                return (String)base["Name"];
            }
        }

        //[XmlAttribute(AttributeName = "Id", DataType = "Integer")]
        [ConfigurationProperty("Id")]
        public int Id
        {
            get
            {
                return (int)base["Name"];
            }
        }


        //[XmlAttribute(AttributeName = "Description")]
        [ConfigurationProperty("Description")]
        public string Description
        {
            get { return (string)base["Description"]; }
        }

        //protected override ConfigurationElement CreateNewElement()
        //{
        //    return (ConfigurationElement)new PaymentMethod();
        //}

        //protected override object GetElementKey(ConfigurationElement element)
        //{
        //    return (object) (element as PaymentMethod).Name;
        //}

        //public List<PaymentChannel> ListOfAvailablePaymentChannels { get; set; }
    }
}