using Orchard.Users.Models;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class PSSFeeParty : CBSModel
    {
        public virtual string Name { get; set; }

        public virtual string AccountNumber { get; set; }

        public virtual Bank Bank { get; set; }

        public virtual UserPartRecord LastUpdatedBy { get; set; }

        public virtual bool IsActive { get; set; }

        /// <summary>
        /// <see cref="Enums.SettlementAccountType"/>
        /// </summary>
        public virtual int SettlementAccountType { get; set; }
    }
}