using NHibernate.Linq;
using Parkway.CBS.ClientRepository.Repositories.NAGIS.Contracts;
using Parkway.CBS.ClientRepository.Repositories.NAGIS.Models;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.ClientRepository.Repositories.NAGIS
{
    public class NagisOldInvoiceSummaryDAOManager : Repository<NagisOldInvoiceSummary>, INagisOldInvoiceSummaryDAOManager
    {
        public NagisOldInvoiceSummaryDAOManager(IUoW uow) : base(uow)
        {

        }

        public List<NAGISDataGenerateInvoiceModel> GetBatch(long batchId)
        {
            var obj = new NagisDataBatch { Id = batchId };
            return _uow.Session.Query<NagisOldInvoiceSummary>()
                .Where(x => x.NagisDataBatch == obj)
                .Select(x =>
                new NAGISDataGenerateInvoiceModel
                {
                    NAGISOldInvoiceSummaryId = x.Id,
                    Address = x.TaxEntity.Address,
                    Recipient = x.TaxEntity.Recipient,
                    TaxProfileId = x.TaxEntity.Id,
                    TaxProfileCategoryId = x.TaxEntity.TaxEntityCategory.Id,
                    CashflowCustomerId = x.TaxEntity.CashflowCustomerId,
                    Amount = x.TotalAmount,
                    AmountDue = x.AmountDue,
                    NagisInvoiceNumber = x.NagisInvoiceNumber,
                    GroupId = x.GroupId,
                    //InvoiceItems = x.InvoiceItems,
                    Type = x.TaxEntity.TaxEntityCategory.Id == 1 ? Cashflow.Ng.Models.Enums.CashFlowCustomerType.Individual : Cashflow.Ng.Models.Enums.CashFlowCustomerType.Business,
                }).ToList();
        }

        public List<NAGISDataGenerateInvoiceModel> GetChunkedBatch(long batchId, int chunkSize, int skip)
        {
            var obj = new NagisDataBatch { Id = batchId };
            return _uow.Session.Query<NagisOldInvoiceSummary>()
                .Where(x => x.NagisDataBatch == obj)
                .Skip(skip).Take(chunkSize)
                .Select(x =>
                new NAGISDataGenerateInvoiceModel
                {
                    NAGISOldInvoiceSummaryId = x.Id,
                    Address = x.TaxEntity.Address,
                    Recipient = x.TaxEntity.Recipient,
                    TaxProfileId = x.TaxEntity.Id,
                    TaxProfileCategoryId = x.TaxEntity.TaxEntityCategory.Id,
                    CashflowCustomerId = x.TaxEntity.CashflowCustomerId,
                    Amount = x.TotalAmount,
                    AmountDue = x.AmountDue,
                    NagisInvoiceNumber = x.NagisInvoiceNumber,
                    GroupId = x.GroupId,
                    //InvoiceItems = x.InvoiceItems,
                    Type = x.TaxEntity.TaxEntityCategory.Id == 1 ? Cashflow.Ng.Models.Enums.CashFlowCustomerType.Individual : Cashflow.Ng.Models.Enums.CashFlowCustomerType.Business,
                }).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="batchId"></param>
        public void UpdateInvoiceStagingWithCashFlowResponse(long batchId)
        {
            try
            {
                var queryText = $"UPDATE invSummary SET invSummary.InvoiceNumber = BI.InvoiceNumber, invSummary.CashflowInvoiceIdentifier = BI.CashflowInvoiceIdentifier, invSummary.InvoiceDescription = BI.InvoiceDescription, " +
                    $" invSummary.PrimaryContactId = BI.PrimaryContactId, invSummary.StatusId = BI.Status, invSummary.MDAId = BI.MDAId, invSummary.ExpertSystemId = BI.ExpertSystemId,invSummary.CashflowCustomerId= BI.CashflowCustomerId," +
                    $" invSummary.DueDate = BI.DueDate, invSummary.InvoiceUniqueKey = BI.InvoiceUniqueKey, invSummary.InvoiceURL = BI.IntegrationPreviewUrl, invSummary.RevenueHead_Id = BI.RevenueHeadId FROM Parkway_CBS_Core_NagisOldInvoiceSummary invSummary" +
                    $" INNER JOIN Parkway_CBS_Core_NagisOldInvoiceResponseStaging as BI ON BI.CashflowInvoiceIdentifier = invSummary.TaxEntity_Id AND BI.InvoiceUniqueKey = invSummary.NagisInvoiceNumber WHERE invSummary.NagisDataBatch_Id = :batch_Id";
                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("batch_Id", batchId);

                query.ExecuteUpdate();
            }
            catch (Exception)
            { throw; }
        }
    }
}
