using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Client.Web.RouteName
{
    public class PAYESchedule
    {
        /// <summary>
        /// C.PAYE.Schedules
        /// </summary>
        public static string PAYESchedules { get { return "C.PAYE.Schedules"; } }

        /// <summary>
        /// C.PAYE.Schedules.Utilized.Receipts
        /// </summary>
        public static string PAYEScheduleUtilizedReceipts { get { return "C.PAYE.Schedules.Utilized.Receipts"; } }

        /// <summary>
        /// Schedules.Move.Right
        /// </summary>
        public static string PAYESchedulesNavigation { get { return "Schedules.Move.Right"; } }
    }
}