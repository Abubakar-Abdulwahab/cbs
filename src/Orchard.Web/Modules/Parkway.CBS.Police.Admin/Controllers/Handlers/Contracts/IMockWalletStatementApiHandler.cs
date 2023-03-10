using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts
{
    public interface IMockWalletStatementApiHandler : IDependency
    {
        List<dynamic> GetStatements(int skip);
    }
}
