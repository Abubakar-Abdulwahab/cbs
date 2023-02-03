using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class PSSSettlementAdapterCommand : CBSModel
    {
        /// <summary>
        /// The command that serviced the request
        /// </summary>
        public virtual Command ServiceCommand { get; set; }

        /// <summary>
        /// The command that will receive the adapter settlement
        /// </summary>
        public virtual Command SettlementCommand { get; set; }

        public virtual PSSFeePartyAdapterConfiguration FeePartyAdapter { get; set; }

        public virtual bool IsActive { get; set; }

        /// <summary>
        /// <see cref="Enums.SettlementAccountType"/>
        /// </summary>
        public virtual int SettlementAccountType { get; set; }

    }
}