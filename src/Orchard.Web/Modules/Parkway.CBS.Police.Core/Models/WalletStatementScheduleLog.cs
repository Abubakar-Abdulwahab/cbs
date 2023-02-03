using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Models
{
    public class WalletStatementScheduleLog : CBSModel
    {
        public virtual WalletStatementSchedule WalletStatementSchedule { get; set; }

        public virtual DateTime PeriodStartDate { get; set; }

        public virtual DateTime PeriodEndDate { get; set; }
    }
}