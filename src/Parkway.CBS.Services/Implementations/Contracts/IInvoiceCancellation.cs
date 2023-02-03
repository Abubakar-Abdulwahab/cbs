using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Services.Implementations.Contracts
{
    public interface IInvoiceCancellation
    {
        /// <summary>
        /// This checks through the invoice list and cancel those that have passed the due date
        /// </summary>
        /// <returns></returns>
        string ProcessInvoiceCancellation(string tenantName);
    }
}
