using NHibernate.Criterion;
using NHibernate.Linq;
using Parkway.CBS.ClientRepository.Repositories.ReferenceDataImpl.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Entities.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ClientRepository.Repositories.ReferenceDataImpl
{
    public class ReferenceDataWithHoldingTaxOnRentDAOManager : Repository<ReferenceDataWithHoldingTaxOnRent>, IReferenceDataWithHoldingTaxOnRentDAOManager
    {
        public ReferenceDataWithHoldingTaxOnRentDAOManager(IUoW uow) : base(uow)
        {

        }

        public List<ReferenceDataGenerateInvoiceModel> GetBatch(long batchId)
        {
            var obj = new ReferenceDataBatch { Id = batchId };
            return _uow.Session.Query<ReferenceDataWithHoldingTaxOnRent>()
                .Where(s => s.ReferenceDataBatch == obj)
                .Select(s =>
                new ReferenceDataGenerateInvoiceModel
                {
                    Address = s.TaxEntity.Address,
                    Recipient = s.TaxEntity.Recipient,
                    Email = s.TaxEntity.Email,
                    TaxProfileId = s.TaxEntity.Id,
                    TaxProfileCategoryId = s.TaxEntity.TaxEntityCategory.Id,
                    CashflowCustomerId = s.TaxEntity.CashflowCustomerId,
                    Amount = s.PropertyRentAmount,
                    WithholdingTaxonRentId = s.Id,
                    OperationType = s.ReferenceDataTaxEntityStaging.OperationType
                }).ToList();
        }

        public List<ReferenceDataGenerateInvoiceModel> GetChunkedBatch(long batchId, int chunkSize, int skip)
        {
            var obj = new ReferenceDataBatch { Id = batchId };
            return _uow.Session.Query<ReferenceDataWithHoldingTaxOnRent>()
                .Where(s => s.ReferenceDataBatch == obj)
                .Skip(skip).Take(chunkSize)
                .Select(s =>
                new ReferenceDataGenerateInvoiceModel
                {
                    Address = s.TaxEntity.Address,
                    Recipient = s.TaxEntity.Recipient,
                    Email = s.TaxEntity.Email,
                    TaxProfileId = s.TaxEntity.Id,
                    TaxProfileCategoryId = s.TaxEntity.TaxEntityCategory.Id,
                    CashflowCustomerId = s.TaxEntity.CashflowCustomerId,
                    Amount = s.PropertyRentAmount,
                    WithholdingTaxonRentId = s.Id,
                    OperationType = s.ReferenceDataTaxEntityStaging.OperationType
                }).ToList();
        }
    }
}
