using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Parkway.Scheduler.Interface.Loggers
{

    /// <summary>
    /// Logger collection.Collection of avaliable loggers from the config file.
    /// </summary>
    public class SchedulerLoggerCollection : IConfigurationSectionHandler
    {

        /// <summary>
        /// Create the collection
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="configContext"></param>
        /// <param name="section"></param>
        /// <returns>List{LoggerItem}</returns>
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
