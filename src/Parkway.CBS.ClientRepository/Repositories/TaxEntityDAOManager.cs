using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.ClientRepository.Repositories
{
    public class TaxEntityDAOManager : Repository<TaxEntity>, ITaxEntityDAOManager
    {
        public TaxEntityDAOManager(IUoW uow) : base(uow)
        {

        }

        public void UpdateTaxEntityWithCashflowInvoiceResponse(long batchId)
        {
            try
            {
                var queryText = $"UPDATE t SET t.PrimaryContactId = NavInv.PrimaryContactId, t.CashflowCustomerId = NavInv.CashflowCustomerId FROM Parkway_CBS_Core_TaxEntity t" +
                    $" INNER JOIN Parkway_CBS_Core_NagisOldInvoiceSummary as NavInv ON t.Id = NavInv.TaxEntity_Id AND t.TaxEntityCategory_Id = NavInv.TaxEntityCategory_Id" +
                    $" WHERE NavInv.NagisDataBatch_Id = :batch_Id AND AmountDue > 0";
                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("batch_Id", batchId);

                query.ExecuteUpdate();
            }
            catch (Exception)
            { throw; }
        }

        public void UpdateTaxEntityWithCashflowCustomerResponse(long batchId)
        {
            try
            {
                var queryText = $"UPDATE t SET t.PrimaryContactId = NavCus.PrimaryContactId, t.CashflowCustomerId = NavCus.CashflowCustomerId FROM Parkway_CBS_Core_TaxEntity t" +
                    $" INNER JOIN Parkway_CBS_Core_NagisOldInvoiceCustomerResponse as NavCus ON t.Id = NavCus.TaxEntity_Id" +
                    $" WHERE NavCus.BatchIdentifier = :batch_Id";
                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("batch_Id", batchId);

                query.ExecuteUpdate();
            }
            catch (Exception)
            { throw; }
        }

    }
}
