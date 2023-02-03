using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;

namespace Parkway.CBS.Police.Core.Services
{
    public class AccountPaymentRequestSettlementDetailManager : BaseManager<AccountPaymentRequestSettlementDetail>, IAccountPaymentRequestSettlementDetailManager<AccountPaymentRequestSettlementDetail>
    {
        public AccountPaymentRequestSettlementDetailManager(IRepository<AccountPaymentRequestSettlementDetail> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
        }
    }
}