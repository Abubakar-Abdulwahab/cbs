using Parkway.CBS.ClientRepository.Repositories.ReferenceDataImpl.Contracts;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ClientRepository.Repositories.ReferenceDataImpl
{
    public class GeneralBatchReferenceDAOManager : Repository<GeneralBatchReference>, IGeneralBatchReferenceDAOManager
    {
        public GeneralBatchReferenceDAOManager(IUoW uow) : base(uow)
        {

        }

        public GeneralBatchReference GetBatchRecord(string batchIdentifier)
        {
            try
            {
                return _uow.Session.QueryOver<GeneralBatchReference>().Where(x => x.BatchRef == batchIdentifier).SingleOrDefault();
            }
            catch (Exception exception)
            {
                throw;
            }
        }

        public GeneralBatchReference GetBatchRecord(long batchIdentifier)
        {
            try
            {
                return _uow.Session.QueryOver<GeneralBatchReference>().Where(x => x.Id == batchIdentifier).SingleOrDefault();
            }
            catch (Exception exception)
            {
                throw;
            }
        }
    }
}
