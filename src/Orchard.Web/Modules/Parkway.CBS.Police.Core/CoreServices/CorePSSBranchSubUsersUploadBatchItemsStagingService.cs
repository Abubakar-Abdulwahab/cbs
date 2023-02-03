using Orchard.Logging;
using Orchard.Security;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Core.CoreServices
{
    public class CorePSSBranchSubUsersUploadBatchItemsStagingService : ICorePSSBranchSubUsersUploadBatchItemsStagingService
    {
        private readonly IPSSBranchSubUsersUploadBatchItemsStagingManager<PSSBranchSubUsersUploadBatchItemsStaging> _iPSSBranchSubUsersUploadBatchItemsStagingManager;
        private readonly IMembershipService _membershipService;
        ILogger Logger { get; set; }
        public CorePSSBranchSubUsersUploadBatchItemsStagingService(IPSSBranchSubUsersUploadBatchItemsStagingManager<PSSBranchSubUsersUploadBatchItemsStaging> iPSSBranchSubUsersUploadBatchItemsStagingManager, IMembershipService membershipService)
        {
            _iPSSBranchSubUsersUploadBatchItemsStagingManager = iPSSBranchSubUsersUploadBatchItemsStagingManager;
            _membershipService = membershipService;
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// Creates branch sub users
        /// </summary>
        /// <param name="batchId"></param>
        public void CreateBranchSubUsers(long batchId)
        {
            try
            {
                _iPSSBranchSubUsersUploadBatchItemsStagingManager.StartUOW();
                //create branches
                _iPSSBranchSubUsersUploadBatchItemsStagingManager.CreateBranches(batchId);

                //resolve tax entity profile location ids
                _iPSSBranchSubUsersUploadBatchItemsStagingManager.ResolveTaxEntityProfileLocationIdsForCreatedBranches(batchId);

                //create sub users
                int totalCount = _iPSSBranchSubUsersUploadBatchItemsStagingManager.Count(x => x.PSSBranchSubUsersUploadBatchStaging.Id == batchId && !x.HasError);
                int baseTake = 10;
                int pageCount = Util.Pages(baseTake, totalCount);
                for (int page = 0; page < pageCount; ++page)
                {
                    List<PSSBranchSubUsersUploadBatchItemsStagingDTO> items = _iPSSBranchSubUsersUploadBatchItemsStagingManager.GetItems(batchId, page * baseTake, baseTake).ToList();

                    //create user part records for new sub users
                    foreach(PSSBranchSubUsersUploadBatchItemsStagingDTO item in items)
                    {
                        _membershipService.CreateUser(new CreateUserParams(item.SubUserEmail.Trim(), Util.GenerateRandomPassword(), item.SubUserEmail, null, null, true));
                    }
                }

                _iPSSBranchSubUsersUploadBatchItemsStagingManager.EndUOW();

                //update user ids here
                _iPSSBranchSubUsersUploadBatchItemsStagingManager.UpdateUserIdsForSubUsersInBatchWithId(batchId);

                //create cbs user for sub users
                _iPSSBranchSubUsersUploadBatchItemsStagingManager.CreateSubUsersAsCBSUsers(batchId);

                //attach sub users to their respective branches
                _iPSSBranchSubUsersUploadBatchItemsStagingManager.AttachSubUsersToBranchLocations(batchId);
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _iPSSBranchSubUsersUploadBatchItemsStagingManager.RollBackAllTransactions();
                throw;
            }
        }
    }
}