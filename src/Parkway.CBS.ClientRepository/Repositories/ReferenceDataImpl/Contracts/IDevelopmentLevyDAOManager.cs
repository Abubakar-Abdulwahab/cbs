using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Entities.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ClientRepository.Repositories.ReferenceDataImpl.Contracts
{
    public interface IDevelopmentLevyDAOManager : IRepository<TaxEntity>
    {
        List<ReferenceDataGenerateInvoiceModel> GetTaxEntitiesForDevelopmentLevy();

    }
}
