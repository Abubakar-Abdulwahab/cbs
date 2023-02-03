using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ClientRepository.Repositories.ReferenceDataImpl.Contracts
{
    public interface IGeneralBatchReferenceDAOManager : IRepository<GeneralBatchReference>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="batchIdentifier"></param>
        /// <returns>GeneralBatchReference</returns>
        GeneralBatchReference GetBatchRecord(string batchIdentifier);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="batchIdentifier"></param>
        /// <returns>GeneralBatchReference</returns>
        GeneralBatchReference GetBatchRecord(long batchIdentifier);


    }
}
