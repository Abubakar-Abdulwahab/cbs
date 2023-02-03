using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface IRoleManager<Role> : IDependency, IBaseManager<Role>
    {
    }
}
