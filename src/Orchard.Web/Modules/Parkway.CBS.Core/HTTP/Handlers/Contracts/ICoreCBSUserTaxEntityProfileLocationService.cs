using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.HTTP.Handlers.Contracts
{
    public interface ICoreCBSUserTaxEntityProfileLocationService : IDependency
    {
        /// <summary>
        /// Attaches a CBS User to a tax entity profile location with the specified id
        /// </summary>
        /// <param name="cbsUserId">CBS User Id</param>
        /// <param name="locationId">Tax entity profile location id</param>
        void AttachUserToLocation(long cbsUserId, int locationId);


        /// <summary>
        /// Validates if the sub user with specified user part record id belongs to a branch created by admin user with specified tax entity
        /// </summary>
        /// <param name="adminUserTaxEntity"></param>
        /// <param name="subUserUserId"></param>
        /// <returns></returns>
        bool CheckIfSubUserBelongsToAdminUser(long adminUserTaxEntity, int subUserUserId);
    }
}
