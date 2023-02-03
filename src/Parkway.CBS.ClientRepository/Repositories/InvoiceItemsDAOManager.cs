using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ClientRepository.Repositories
{
    public class InvoiceItemsDAOManager : Repository<InvoiceItems>, IInvoiceItemsDAOManager
    {
        public InvoiceItemsDAOManager(IUoW uow) : base(uow)
        {

        }



        public void CreateNAGISInvoiceItems(long batchId)
        {
            try
            {
                var queryText = $" INSERT INTO Parkway_CBS_Core_InvoiceItems (InvoiceNumber, MDA_Id, RevenueHead_Id, Invoice_Id, UnitAmount, TaxEntity_Id, TaxEntityCategory_Id, Quantity, CreatedAtUtc, UpdatedAtUtc) SELECT" +
                                $" NagSum.InvoiceNumber, revHead.MDA_Id, NagOld.RevenueHead_Id, inv.Id, NagOld.AmountDue, NagSum.TaxEntity_Id, NagSum.TaxEntityCategory_Id, NagOld.Quantity, :dateSaved, :dateSaved" +
                                $" FROM Parkway_CBS_Core_NagisOldInvoices NagOld " +
                                $" INNER JOIN Parkway_CBS_Core_NagisOldInvoiceSummary NagSum ON NagSum.Id = NagOld.NagisOldInvoiceSummary_Id AND NagSum.TaxEntity_Id = NagOld.TaxEntity_Id" +
                                $" INNER JOIN Parkway_CBS_Core_Invoice inv ON NagSum.InvoiceNumber = inv.InvoiceNumber AND NagSum.TaxEntity_Id = inv.TaxPayer_Id" +
                                $" INNER JOIN Parkway_CBS_Core_RevenueHead revHead ON revHead.Id = NagOld.RevenueHead_Id" +
                                $" WHERE NagOld.NagisDataBatch_Id = :batch_Id";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("batch_Id", batchId);
                query.SetParameter("dateSaved", DateTime.Now.ToLocalTime());

                query.ExecuteUpdate();

            }
            catch (Exception exception)
            {
                throw;
            }
        }
    }
}
