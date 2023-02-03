using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Utilities
{
    public class CronGenerator : ICronGenerator
    {
        private CRONBaker.CronBaker baker;

        public CronGenerator()
        {
            baker = new CRONBaker.CronBaker();
        }
        public string Expression()
        {
            return baker.Expression();
        }

        public ICronGenerator SetDailyInterval(int startDay, int interval)
        {
            baker.SetDailyInterval(startDay, interval);
            return this;
        }

        public ICronGenerator SetHour(int hour)
        {
            baker.SetHour(hour);
            return this;
        }

        public ICronGenerator SetMinute(int minute)
        {
            baker.SetMinute(minute);
            return this;
        }

        public ICronGenerator SetMonths(List<string> months, int dayOfMonthOrSelectedDay, int day)
        {
            baker.SetMonths(months, dayOfMonthOrSelectedDay, day);
            return this;
        }

        public ICronGenerator SetWeekly(List<string> days, int startMonth)
        {
            baker.SetWeekly(days, startMonth);
            return this;
        }

        public ICronGenerator SetYear(int interval, int startYear)
        {
            baker.SetYear(interval, startYear);
            return this;
        }
    }
}