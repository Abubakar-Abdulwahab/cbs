using NHibernate.Linq;
using Parkway.CBS.ClientRepository.Repositories.NAGIS.Contracts;
using Parkway.CBS.ClientRepository.Repositories.NAGIS.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ClientRepository.Repositories.NAGIS
{
    public class NagisOldCustomersDAOManager : Repository<NagisOldCustomers>, INagisOldCustomersDAOManager
    {
        public NagisOldCustomersDAOManager(IUoW uow) : base(uow)
        {

        }

        public void CreateNAGISCustomers(long batchId)
        {
            try
            {
                var queryText = $"INSERT INTO Parkway_CBS_Core_NagisOldCustomers (NagisDataBatch_Id, TIN, CustomerName, PhoneNumber, CustomerId, TaxEntityCategory_Id, Address, CreatedAtUtc, UpdatedAtUtc)" +
                    $" SELECT nagis.NagisDataBatch_Id, nagis.TIN, nagis.CustomerName, nagis.PhoneNumber, nagis.CustomerId, nagis.TaxEntityCategory_Id, nagis.Address, :dateSaved, :dateSaved FROM Parkway_CBS_Core_NagisOldInvoices nagis INNER JOIN (SELECT CustomerId, MIN(Id) as Id FROM Parkway_CBS_Core_NagisOldInvoices  WHERE NagisDataBatch_Id = :batch_Id AND OperationType_Id = :OperationType_Id AND AmountDue > 0 GROUP BY CustomerId) AS nagisg ON nagis.Id = nagisg.Id";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("dateSaved", DateTime.Now.ToLocalTime());
                query.SetParameter("batch_Id", batchId);
                query.SetParameter("OperationType_Id", (int)ReferenceDataOperationType.Create);

                query.ExecuteUpdate();
            }
            catch (Exception)
            { throw; }
        }

        public List<NAGISDataGenerateInvoiceModel> GetChunkedBatchCustomer(long batchId, int chunkSize, int skip)
        {
            var obj = new NagisDataBatch { Id = batchId };
            return _uow.Session.Query<NagisOldCustomers>()
                .Where(x => x.NagisDataBatch == obj)
                .Skip(skip).Take(chunkSize)
                .Select(x =>
                new NAGISDataGenerateInvoiceModel
                {
                    Address = x.TaxEntity.Address,
                    Recipient = x.TaxEntity.Recipient,
                    TaxProfileId = x.TaxEntity.Id,
                    TaxProfileCategoryId = x.TaxEntity.TaxEntityCategory.Id,
                    CashflowCustomerId = x.TaxEntity.CashflowCustomerId,
                    Type = x.TaxEntity.TaxEntityCategory.Id == 1 ? Cashflow.Ng.Models.Enums.CashFlowCustomerType.Individual : Cashflow.Ng.Models.Enums.CashFlowCustomerType.Business,
                }).ToList();
        }

        public void UpdateNAGISCustomerRecordsTaxEntityId(long batchId)
        {
            try
            {
                var queryText = $"UPDATE nagis SET nagis.TaxEntity_Id = t.TaxEntity_Id FROM Parkway_CBS_Core_NagisOldCustomers nagis" +
                    $" INNER JOIN Parkway_CBS_Core_NagisOldInvoiceStagingHelper as t ON t.SourceId = nagis.Id AND t.CustomerId = nagis.CustomerId WHERE nagis.NagisDataBatch_Id = :batch_Id";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("batch_Id", batchId);

                query.ExecuteUpdate();
            }
            catch (Exception)
            { throw; }
        }
    }
}
