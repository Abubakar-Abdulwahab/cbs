using Parkway.CBS.Core.Models;

namespace Parkway.CBS.ClientRepository.Repositories.Contracts
{
    public interface ITaxEntityDAOManager : IRepository<TaxEntity>
    {
        /// <summary>
        /// Update the Tax Entity customer details records with the customer details response from the Cashflow .
        /// </summary>
        /// <param name="batchId"></param>
        void UpdateTaxEntityWithCashflowInvoiceResponse(long batchId);

        /// <summary>
        /// Update the Tax Entity customer details records with the customer details response from the Cashflow .
        /// </summary>
        /// <param name="batchId"></param>
        void UpdateTaxEntityWithCashflowCustomerResponse(long batchId);
    }
}
