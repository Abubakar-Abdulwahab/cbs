using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HTTP.Handlers
{
    public class CoreCBSUserTaxEntityProfileLocationService : ICoreCBSUserTaxEntityProfileLocationService
    {
        private readonly ICBSUserTaxEntityProfileLocationManager<CBSUserTaxEntityProfileLocation> _repo;
        ILogger Logger { get; set; }

        public CoreCBSUserTaxEntityProfileLocationService(ICBSUserTaxEntityProfileLocationManager<CBSUserTaxEntityProfileLocation> repo)
        {
            _repo = repo;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Attaches a CBS User to a tax entity profile location with the specified id
        /// </summary>
        /// <param name="cbsUserId">CBS User Id</param>
        /// <param name="locationId">Tax entity profile location id</param>
        public void AttachUserToLocation(long cbsUserId, int locationId)
        {
            try
            {
                CBSUserTaxEntityProfileLocation userAttachedToLocation = new CBSUserTaxEntityProfileLocation
                {
                    CBSUser = new CBSUser { Id = cbsUserId },
                    TaxEntityProfileLocation = new TaxEntityProfileLocation { Id = locationId }
                };
                if (!_repo.Save(userAttachedToLocation)) { throw new CouldNotSaveRecord(); }
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _repo.RollBackAllTransactions();
                throw;
            }
        }

        /// <summary>
        /// Validates if the sub user with specified user part record id belongs to a branch created by admin user with specified tax entity
        /// </summary>
        /// <param name="adminUserTaxEntity"></param>
        /// <param name="subUserUserId"></param>
        /// <returns></returns>
        public bool CheckIfSubUserBelongsToAdminUser(long adminUserTaxEntity, int subUserUserId)
        {
            return _repo.Count(x => x.CBSUser.UserPartRecord.Id == subUserUserId && x.TaxEntityProfileLocation.TaxEntity.Id == adminUserTaxEntity) > 0;
        }
    }
}