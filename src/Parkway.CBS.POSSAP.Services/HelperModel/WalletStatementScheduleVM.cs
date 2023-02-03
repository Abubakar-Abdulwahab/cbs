using System;

namespace Parkway.CBS.POSSAP.Services.HelperModel
{
    public class WalletStatementScheduleVM
    {
        public int Id { get; set; }

        public string CronExpression { get; set; }

        public DateTime PeriodStartDate { get; set; }

        public DateTime PeriodEndDate { get; set; }

        public DateTime NextScheduleDate { get; set; }
    }
}
