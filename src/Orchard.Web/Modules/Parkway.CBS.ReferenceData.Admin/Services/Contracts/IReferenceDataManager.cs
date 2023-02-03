using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Payee.ReferenceDataImplementation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ReferenceData.Admin.Services.Contracts
{
    public interface IReferenceDataManager<ReferenceData> : IDependency, IBaseManager<ReferenceData>
    {

    }
}
