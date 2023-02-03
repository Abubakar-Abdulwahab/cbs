using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class ScheduleHelperModel
    {
        /// <summary>
        /// billing id
        /// </summary>
        public int BillingIdentifier { get; set; }

        /// <summary>
        /// start date and time for when the cron starts running
        /// </summary>
        public DateTime StartDateAndTime { get; set; }

        /// <summary>
        /// cron expression
        /// </summary>
        public string CronExpression { get; set; }

        /// <summary>
        /// Duration for the schedule
        /// </summary>
        public DurationModel Duration { get; set; }
    }
}