using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using System.Collections.Generic;

namespace Parkway.CBS.ReferenceData.Admin.Services.Contracts
{
    public interface IReferenceDataLGAManager<LGA> : IDependency, IBaseManager<LGA>
    {
        IEnumerable<LGA> GetLGAs(int stateId);
    }
}
