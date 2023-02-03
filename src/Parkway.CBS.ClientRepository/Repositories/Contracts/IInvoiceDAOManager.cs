using Parkway.CBS.Core.Models;
using Parkway.CBS.Entities.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ClientRepository.Repositories.Contracts
{
    public interface IInvoiceDAOManager : IRepository<Invoice>
    {
        /// <summary>
        /// create the invoice for IPPIS record
        /// </summary>
        /// <param name="batchId"></param>
        void CreateIPPISInvoices(long batchId, RevenueHeadDetailsForInvoiceGenerationLite revenueHeadDetails);


        /// <summary>
        /// Create Invoices with response from Cashflow batch invoice
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="revenueHead"></param>
        void CreateBatchInvoices(long batchId, RevenueHead revenueHead);

        /// <summary>
        /// create the invoice for NAGIS record
        /// </summary>
        /// <param name="batchId"></param>
        void CreateNAGISInvoices(long batchId);

        /// <summary>
        /// create the Transaction Log for IPPIS invoices
        /// </summary>
        /// <param name="batchId"></param>
        void CreateIPPISInvoiceTransactionLog(long batchId);

        /// <summary>
        /// This checks through the invoice list and cancel those that have not been paid but have passed the due date
        /// </summary>
        /// <returns>string</returns>
        string ProcessInvoiceCancellation();

    }
}
