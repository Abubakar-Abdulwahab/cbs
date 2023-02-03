using Parkway.DataExporter.Implementations.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Parkway.DataExporter.Implementations.Models
{
    public class ExportTemplates
    {
        public List<TemplateGroup> Groups { get; set; }
    }

    public class TemplateGroup
    {
        [XmlAttribute("Type")]
        public TemplateType Type { get; set; }
        public List<TemplateCollection> Collections { get; set; }
    }

    public class TemplateCollection
    {
        [XmlAttribute("Tenants")]
        public string Tenants { get; set; }

        [XmlAttribute("Name")]
        public string Name { get; set; }

        public List<Template> Templates { get; set; }

    }

    public class Template
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlAttribute("File")]
        public string File { get; set; }

        [XmlAttribute("BasePath")]
        public string BasePath { get; set; }

        [XmlAttribute("SavingPath")]
        public string SavingPath { get; set; }

        public List<Field> Fields { get; set; }
    }

    public class Field
    {
        [XmlAttribute("Key")]
        public string Key { get; set; }
        [XmlAttribute("Value")]
        public string Value { get; set; }
    }

}
