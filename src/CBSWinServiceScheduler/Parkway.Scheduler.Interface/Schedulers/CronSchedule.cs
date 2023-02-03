using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.Scheduler.Interface.Schedulers
{
    public class CronSchedule
    {
        public TriggerDurationHelperModel Duration { get; set; }
        public string Identifier { get; set; }
        public string TriggerIdentifier { get; set; }
        public string GroupIdentifier { get; set; }
        public string CronExpression { get; set; }
        public DateTime StartTime { get; set; }
        public string Description { get; set; }
        public Dictionary<string, dynamic> JobData { get; set; }

        /// <summary>
        /// if this schedule is a fresh schedule
        /// </summary>
        public bool IsAReSchedule { get; set; }

    }
}
