using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.Utilities
{
    public interface ICronGenerator : IDependency
    {

        /// <summary>
        /// Get cron expression
        /// </summary>
        /// <returns>string</returns>
        string Expression();


        /// <summary>
        /// Set year. Every year interval starting from startYear
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="startYear"></param>
        /// <returns>ICronGenerator</returns>
        ICronGenerator SetYear(int interval, int startYear);


        /// <summary>
        /// Set the months, direction of day and specific day
        /// <para>
        /// months: Jan = 1, Feb = 2, Mar = 3, Apr = 4, May = 5, Jun = 6, 
        /// Jul = 7, Aug = 8, Sep = 9, Oct = 10, Nov = 11, Dec = 12,        /// 
        /// </para>
        /// <para>
        /// dayOfMonthOrSelectedDay : None = 0, FirstDayOfTheMonth = 1, FirstWeekDayOfTheMonth = 2, LastDayOfTheMonth = 3,
        /// LastDayOfWeekDayOfTheMonth = 4, FirstDay = 5, SecondDay = 6, ThirdDay = 7, FourthDay = 8, LastDay = 9
        /// </para>
        /// <para>
        /// day: SUN = 1, MON = 2, TUE = 3, WED = 4, THU = 5, FRI = 6, SAT = 7
        /// </para>
        /// </summary>
        /// <param name="months"></param>
        /// <param name="dayOfMonthOrSelectedDay"></param>
        /// <param name="day"></param>
        ICronGenerator SetMonths(List<string> months, int dayOfMonthOrSelectedDay, int day);


        /// <summary>
        /// Set weekly schedule
        /// <para>
        /// days: SUN = 1, MON = 2, TUE = 3, WED = 4, THU = 5, FRI = 6, SAT = 7
        /// </para>
        /// <para>
        /// startMonth: Jan = 1, Feb = 2, Mar = 3, Apr = 4, May = 5, Jun = 6, 
        /// Jul = 7, Aug = 8, Sep = 9, Oct = 10, Nov = 11, Dec = 12,        /// 
        /// </para>
        /// </summary>
        /// <param name="days"></param>
        /// <param name="startMonth"></param>
        ICronGenerator SetWeekly(List<string> days, int startMonth);


        /// <summary>
        /// Set days
        /// <para>
        /// startDay: is the day the interval starts counting from e.g a startDay of 15 means the interval starts on thhe 15 of the given month, then intervals after
        /// interval: daily interval
        /// </para>
        /// </summary>
        /// <param name="startDay"></param>
        /// <param name="interval"></param>
        /// <returns>ICronGenerator</returns>
        ICronGenerator SetDailyInterval(int startDay, int interval);


        /// <summary>
        /// Set hour
        /// <para>
        /// hours: 0, 1,2,3,4,5,6,7,8,9,10, 11...23
        /// </para>
        /// </summary>
        /// <param name="hour"></param>
        /// <returns>ICronGenerator</returns>
        ICronGenerator SetHour(int hour);


        /// <summary>
        /// set minutes
        /// <para>
        /// minute: 0, 1, 2, 3, 4....59
        /// </para>
        /// </summary>
        /// <param name="minute"></param>
        /// <returns>ICronGenerator</returns>
        ICronGenerator SetMinute(int minute);
    }
}
