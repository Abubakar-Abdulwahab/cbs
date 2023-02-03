using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Models
{
    public class WalletStatementScheduleUpdate : CBSModel
    {
        public virtual WalletStatementSchedule WalletStatementSchedule { get; set; }

        public virtual DateTime CurrentSchedule { get; set; }

        /// <summary>
        /// this is the wallet statement request start date
        /// <para>Indicates when the wallet statement request will start</para>
        /// </summary>
        public virtual DateTime NextStartDate { get; set; }

        /// <summary>
        /// indicate when the wallet statement request will end
        /// </summary>
        public virtual DateTime NextEndDate { get; set; }

        public virtual DateTime NextScheduleDate { get; set; }
    }
}