using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface IHangfireJobReferenceManager<HangfireJobReference> : IDependency, IBaseManager<HangfireJobReference>
    {
    }
}