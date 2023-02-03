using NHibernate.Linq;
using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Entities.DTO;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.ClientRepository.Repositories
{
    public class IPPISTaxPayerSummaryDAOManager : Repository<IPPISTaxPayerSummary>, IIPPISTaxPayerSummaryDAOManager
    {
        public IPPISTaxPayerSummaryDAOManager(IUoW uow) : base(uow)
        { }


        /// <summary>
        /// Get the records from IPPISTaxPayerSummary that need invoices to be generated for them 
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <returns>List{IPPISGenerateInvoiceModel}</returns>
        public List<IPPISGenerateInvoiceModel> GetChunkedBatch(long batchId, int take, int skip)
        {
            var obj = new IPPISBatch { Id = batchId };
            return _uow.Session.Query<IPPISTaxPayerSummary>()
                .Where(s => s.IPPISBatch == obj)
                .Skip(skip).Take(take)
                .Select(s =>
                new IPPISGenerateInvoiceModel
                {
                    Address = s.TaxEntity.Address,
                    Recipient = s.TaxEntity.Recipient,
                    Email = s.TaxEntity.Email,
                    TaxProfileId = s.TaxEntity.Id,
                    TaxProfileCategoryId = s.TaxEntityCategory.Id,
                    CashflowCustomerId = s.TaxEntity.CashflowCustomerId,
                    Amount = s.TotalTaxAmount,
                    Month = s.IPPISBatch.Month,
                    Year = s.IPPISBatch.Year,
                    AgencyCode = s.TaxPayerCode,
                    IPPISTaxPayerSummaryId = s.Id,
                }).ToList();
        }
    }
}
