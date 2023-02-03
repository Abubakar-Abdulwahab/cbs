using System;
using Parkway.CBS.Core.Models;
using Parkway.CBS.ClientRepository.Repositories.Contracts;
using NHibernate.Linq;
using System.Linq;

namespace Parkway.CBS.ClientRepository.Repositories
{
    public class IPPISBatchDAOManager : Repository<IPPISBatch>, IIPPISBatchDAOManager
    {
        public IPPISBatchDAOManager(IUoW uow) : base(uow)
        { }

        /// <summary>
        /// Get model
        /// </summary>
        /// <param name="lambda"></param>
        /// <returns><typeparamref name="M"/>Model</returns>
        public IPPISBatch GetRecordForMonthAndYear(int month, int year)
        {
            try
            {
                return _uow.Session.QueryOver<IPPISBatch>().Where(x => x.Month == month && x.Year == year).SingleOrDefault();
            }
            catch (Exception exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Get batch record for this Id
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns>IPPISBatch</returns>
        public IPPISBatch GetBatchRecord(long batchId)
        {
            try
            {
                return _uow.Session.QueryOver<IPPISBatch>().Where(x => x.Id == batchId).SingleOrDefault();
            }
            catch (Exception exception)
            {
                throw;
            }
        }
    }
}
