using Parkway.CBS.ClientRepository.Repositories.NAGIS.Contracts;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ClientRepository.Repositories.NAGIS
{
    public class NAGISDataBatchDAOManager : Repository<NagisDataBatch>, INAGISDataBatchDAOManager
    {
        public NAGISDataBatchDAOManager(IUoW uow) : base(uow)
        {

        }

        public NagisDataBatch GetBatchDetails(long generalBatchReferenceId)
        {
            try
            {
                return _uow.Session.QueryOver<NagisDataBatch>().Where(x => x.GeneralBatchReference.Id == generalBatchReferenceId).SingleOrDefault();
            }
            catch (Exception exception)
            {
                throw;
            }
        }

        public NagisDataBatch GetBatchRecord(long batchId)
        {
            try
            {
                return _uow.Session.QueryOver<NagisDataBatch>().Where(x => x.Id == batchId).SingleOrDefault();
            }
            catch (Exception exception)
            {
                throw;
            }
        }
    }
}
