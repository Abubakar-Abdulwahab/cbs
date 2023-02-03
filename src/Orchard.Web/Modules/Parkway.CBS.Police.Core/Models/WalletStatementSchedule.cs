using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Models
{
    public class WalletStatementSchedule : CBSModel
    {
        public virtual string CronExpression { get; set; }

        public virtual DateTime PeriodStartDate { get; set; }

        public virtual DateTime PeriodEndDate { get; set; }

        public virtual DateTime NextScheduleDate { get; set; }
    }
}