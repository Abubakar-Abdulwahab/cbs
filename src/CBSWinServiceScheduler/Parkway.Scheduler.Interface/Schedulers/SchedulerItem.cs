using System.Collections.Specialized;
using System.Xml;

namespace Parkway.Scheduler.Interface.Schedulers
{
    public class SchedulerItem
    {
        /// <summary>
        /// Name of the scheduler
        /// </summary>
        public string Name { get; set; }
        public bool Active { get; set; }
        public string Endpoint { get; set; }
        public string ClientToken { get; set; }
        public string ClientSecret { get; set; }
        public XmlNodeList Properties { get; set; }
    }
}