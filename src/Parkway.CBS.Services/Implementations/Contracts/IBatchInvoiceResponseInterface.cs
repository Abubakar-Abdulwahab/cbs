using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Services.Implementations.Contracts
{
    public interface IBatchInvoiceResponseInterface
    {
        void ProcessInvoices(string tenantName, string batchIdentifier, string batchFileName);
    }
}
