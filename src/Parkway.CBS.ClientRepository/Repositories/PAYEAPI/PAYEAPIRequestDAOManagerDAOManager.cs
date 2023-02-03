using NHibernate.Linq;
using Parkway.CBS.ClientRepository.Repositories.PAYEAPI.Contracts;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ClientRepository.Repositories.PAYEAPI
{
    public class PAYEAPIRequestDAOManagerDAOManager : Repository<PAYEAPIRequest>, IPAYEAPIRequestDAOManager
    {
        public PAYEAPIRequestDAOManagerDAOManager(IUoW uow) : base(uow)
        { }

        /// <summary>
        /// Get PAYE API request details using the batchidentifier and expertsystemid
        /// </summary>
        /// <param name="batchIdentifier"></param>
        /// <param name="expertSystemId"></param>
        /// <returns>PAYEAPIRequestVM</returns>
        public PAYEAPIRequest GetAPIRequestDetails(string batchIdentifier, int expertSystemId)
        {
            return _uow.Session.Query<PAYEAPIRequest>()
                .Where(x => x.BatchIdentifier == batchIdentifier && x.RequestedByExpertSystem.Id == expertSystemId).Single();
        }

    }
}
