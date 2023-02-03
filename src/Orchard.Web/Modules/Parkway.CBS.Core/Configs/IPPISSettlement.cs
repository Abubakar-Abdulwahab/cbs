using System.Xml;
using System.Configuration;
using System.Xml.Serialization;
using System.Collections.Generic;
using System;

namespace Parkway.CBS.Core.Configs
{
    [XmlRoot(ElementName = "IPPISSettlement")]
    public class IPPISSettlement : IConfigurationSectionHandler
    {
        [XmlElement(ElementName = "Tenant")]
        public List<IPPISSettlementConfigTenant> Tenants { get; set; }

        public object Create(object parent, object configContext, XmlNode section)
        {
            return section.OuterXml;
        }
    }


    [XmlRoot(ElementName = "Tenant")]
    public class IPPISSettlementConfigTenant
    {
        [XmlElement(ElementName = "Party")]
        public List<SettlementParty> SettlementParties { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }

        [XmlAttribute(AttributeName = "isActive")]
        public string SIsActive
        {
            get { return this.IsActive.ToString(); }
            set { this.IsActive = Convert.ToBoolean(value); }
        }

        [XmlIgnore]
        public bool IsActive { get; set; }


        [XmlAttribute(AttributeName = "spacing")]
        public string SSpacing
        {
            get { return this.Spacing.ToString(); }
            set { this.Spacing = Convert.ToInt32(value); }
        }


        [XmlIgnore]
        public int Spacing { get; set; }
    }


    [XmlRoot(ElementName = "Party")]
    public class SettlementParty
    {
        [XmlElement(ElementName = "Node")]
        public List<Node> Node { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "percentage")]
        public string SPercentage
        {
            get { return this.Percentage.ToString(); }
            set { this.Percentage = Convert.ToDecimal(value); }
        }

        [XmlAttribute(AttributeName = "cap")]
        public string SCap
        {
            get { return this.Cap.ToString(); }
            set { this.Cap = string.IsNullOrEmpty(value) ? 0 : Convert.ToDecimal(value); }
        }

        [XmlIgnore]
        public decimal Percentage { get; set; }

        [XmlIgnore]
        public decimal Cap { get; set; }
    }


    [XmlRoot(ElementName = "Node")]
    public class Node
    {
        [XmlAttribute(AttributeName = "index")]
        public string SIndex
        {
            get { return this.Index.ToString(); }
            set { this.Index = Convert.ToInt32(value); }
        }

        [XmlIgnore]
        public int Index { get; set; }

        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }
}