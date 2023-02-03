using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HTTP.Handlers
{
    public class CoreTaxEntityProfileLocationService : ICoreTaxEntityProfileLocationService
    {
        private readonly IOrchardServices _orchardServices;
        private readonly ITaxEntityProfileLocationManager<TaxEntityProfileLocation> _repo;
        public ILogger Logger { get; set; }
        public CoreTaxEntityProfileLocationService(ITaxEntityProfileLocationManager<TaxEntityProfileLocation>repo)
        {
            _repo = repo;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Creates new branch information for tax entity with specified Id
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="isDefault"></param>
        /// <returns>location id</returns>
        public int CreateBranch(TaxEntityProfileLocationVM userInput, bool isDefault = false)
        {
            try
            {
                TaxEntityProfileLocation location = new TaxEntityProfileLocation
                {
                    Name = userInput.Name.Trim(),
                    State = new StateModel { Id = userInput.State },
                    LGA = new LGA { Id = userInput.LGA },
                    Address = userInput.Address.Trim(),
                    TaxEntity = new TaxEntity { Id = userInput.TaxEntityId },
                    IsDefault = isDefault
                };

                if (!_repo.Save(location)) { throw new CouldNotSaveRecord(); }
                return location.Id;
            }catch(Exception exception)
            {
                _repo.RollBackAllTransactions();
                Logger.Error(exception, $"Error when trying to create Tax Entity Profile Location(branch) for tax entity with Id {userInput.TaxEntityId}");
                throw;
            }
        }


        /// <summary>
        /// Checks if branch location for tax entity with specified id already exists
        /// </summary>
        /// <param name="name"></param>
        /// <param name="taxEntityId"></param>
        public bool CheckIfBranchWithAddressExists(string address, Int64 taxEntityId)
        {
            return _repo.Count(x => x.Address == address && x.TaxEntity.Id == taxEntityId) > 0;
        }


        /// <summary>
        /// Checks if branch name for tax entity with specified id already exists
        /// </summary>
        /// <param name="name"></param>
        /// <param name="taxEntityId"></param>
        /// <returns></returns>
        public bool CheckIfBranchWithNameExists(string name, Int64 taxEntityId)
        {
            return _repo.Count(x => x.Name == name && x.TaxEntity.Id == taxEntityId) > 0;
        }


        /// <summary>
        /// Gets locations of tax entity with the specified id
        /// </summary>
        /// <param name="taxEntity"></param>
        /// <returns></returns>
        public IEnumerable<TaxEntityProfileLocationVM> GetTaxEntityLocations(long taxEntityId)
        {
            return _repo.GetTaxEntityLocations(taxEntityId);
        }


        /// <summary>
        /// Gets tax entity profile location with specified id
        /// </summary>
        /// <param name="taxEntityId"></param>
        /// <param name="locationId"></param>
        /// <returns></returns>
        public TaxEntityProfileLocationVM GetTaxEntityLocationWithId(long taxEntityId, int locationId)
        {
            return _repo.GetTaxEntityLocationWithId(taxEntityId, locationId);
        }
    }
}