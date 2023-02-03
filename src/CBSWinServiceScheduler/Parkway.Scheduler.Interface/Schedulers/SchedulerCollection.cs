using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Parkway.Scheduler.Interface.Schedulers
{
    public class SchedulerCollection : IConfigurationSectionHandler
    {

        /// <summary>
        /// Collection of Quartz schedulers
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="configContext"></param>
        /// <param name="section"></param>
        /// <returns>List{SchedulerItem}</returns>
        public object Create(object parent, object configContext, XmlNode section)
        {
            List<SchedulerItem> collectionOfSchedulers = new List<SchedulerItem>();

            foreach (XmlNode childNode in section.ChildNodes)
            {
                var sd = childNode.ChildNodes;
                collectionOfSchedulers.Add(new SchedulerItem
                {
                    Name = childNode.Attributes.GetNamedItem("Name").Value,
                    Active = (childNode.Attributes.GetNamedItem("Active").Value).Equals("true", System.StringComparison.InvariantCultureIgnoreCase) ? true : false,
                    Endpoint = @childNode.Attributes.GetNamedItem("Endpoint").Value,
                    ClientToken = @childNode.Attributes.GetNamedItem("ClientToken").Value,
                    ClientSecret = @childNode.Attributes.GetNamedItem("ClientSecret").Value,
                    Properties = childNode.ChildNodes,
                });
            }
            return collectionOfSchedulers;
        }
    }
}