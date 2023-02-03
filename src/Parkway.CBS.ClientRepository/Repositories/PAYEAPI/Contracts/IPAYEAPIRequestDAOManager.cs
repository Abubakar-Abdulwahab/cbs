using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ClientRepository.Repositories.PAYEAPI.Contracts
{
    public interface IPAYEAPIRequestDAOManager : IRepository<PAYEAPIRequest>
    {
        /// <summary>
        /// Get PAYE API request details using the batchidentifier and expertsystemid
        /// </summary>
        /// <param name="batchIdentifier"></param>
        /// <param name="expertSystemId"></param>
        /// <returns>PAYEAPIRequestVM</returns>
        PAYEAPIRequest GetAPIRequestDetails(string batchIdentifier, int expertSystemId);

    }
}
