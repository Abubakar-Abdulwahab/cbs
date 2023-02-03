using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.OSGOF.Admin.Services.Contracts
{
    public interface IOperatorCategoryManager<OperatorCategory> : IDependency, IBaseManager<OperatorCategory>
    { }
}
