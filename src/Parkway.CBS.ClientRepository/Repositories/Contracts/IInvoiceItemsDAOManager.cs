using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ClientRepository.Repositories.Contracts
{
    public interface IInvoiceItemsDAOManager : IRepository<InvoiceItems>
    {
        /// <summary>
        /// create the invoice Items for NAGIS records
        /// </summary>
        /// <param name="batchId"></param>
        void CreateNAGISInvoiceItems(long batchId);

    }
}
