using Orchard;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;
using Parkway.CBS.Core.Services.Contracts;


namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPSSReasonManager<PSSReason> : IDependency, IBaseManager<PSSReason>
    {
        ICollection<PSSReasonVM> GetReasonsVM();

        PSSReasonVM GetReasonVM(int id);
    }
}
