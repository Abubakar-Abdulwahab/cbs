using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Module.API.Controllers.Handlers.Contracts
{
    public interface IScrapFile : IDependency
    {
        void ProcessFile();
    }
}
