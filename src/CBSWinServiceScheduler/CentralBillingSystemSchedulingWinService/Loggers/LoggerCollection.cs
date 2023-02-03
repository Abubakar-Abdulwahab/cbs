using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CentralBillingSystemSchedulingWinService.Loggers
{
    
    public class LoggerCollection : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            List<LoggerItem> collectionOfLoggers = new List<LoggerItem>();
            
            foreach (XmlNode childNode in section.ChildNodes)
            {
                collectionOfLoggers.Add(new LoggerItem
                {
                    Name = childNode.Attributes.GetNamedItem("Name").Value,
                    Enabled = (childNode.Attributes.GetNamedItem("Enabled").Value).Equals("true", System.StringComparison.InvariantCultureIgnoreCase) ? true : false,
                    LogPath = @childNode.Attributes.GetNamedItem("LogPath").Value,
                });
            }
            return collectionOfLoggers;
        }
    }
}
