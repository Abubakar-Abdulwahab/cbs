using Parkway.DataExporter.Implementations.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Parkway.DataExporter.Implementations.Enums
{
    public static class EnumExtensions
    {
        public static IEnumerable<KeyValuePair<string, int>> ToKeyValuePairs(this Enum enumValue)
        {
            return from Enum e in Enum.GetValues(enumValue.GetType())
                   select new KeyValuePair<string, int>(e.ToFriendlyName(), (int)Enum.Parse(e.GetType(), e.ToString()));

        }

        public static string ToFriendlyName(this Enum value)
        {
            var attributes = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(EnumFriendlyName), false);
            return attributes.Length > 0 ? ((EnumFriendlyName)attributes[0]).FriendlyName : value.ToString();
        }

    }

    public enum TemplateType
    {
        [XmlEnum(Name = "Text")]
        Text = 0,
        [XmlEnum(Name = "HTML")]
        HTML = 1,
        [XmlEnum(Name = "Razor")]
        Razor = 2,
        [XmlEnum(Name = "KeyValue")]
        KeyValue = 3
    }

}
