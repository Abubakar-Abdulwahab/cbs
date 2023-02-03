using Parkway.CBS.ClientRepository.Repositories.ReferenceDataImpl.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ClientRepository.Repositories.ReferenceDataImpl.Contracts
{
    public interface ILGAMapping
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        List<LGACollection> GetLGACollections();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="LGAFileId"></param>
        /// <returns></returns>
        string GetLGADatabaseId(string tenant, string LGAFileId);

    }
}
