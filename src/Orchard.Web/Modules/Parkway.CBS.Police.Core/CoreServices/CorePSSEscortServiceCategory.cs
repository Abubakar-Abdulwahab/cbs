using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.CoreServices
{
    public class CorePSSEscortServiceCategory : ICorePSSEscortServiceCategory
    {
        private readonly IPSSEscortServiceCategoryManager<PSSEscortServiceCategory> _repo;

        public CorePSSEscortServiceCategory(IPSSEscortServiceCategoryManager<PSSEscortServiceCategory> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Gets all active escort service categories
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PSSEscortServiceCategoryVM> GetEscortServiceCategories()
        {
            return _repo.GetEscortServiceCategories();
        }

        /// <summary>
        /// Gets an active escort service category with specified id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PSSEscortServiceCategoryVM GetEscortServiceCategoryWithId(int id)
        {
            return _repo.GetEscortServiceCategoryWithId(id);
        }

        /// <summary>
        /// Checks if category type for specified service category exists
        /// </summary>
        /// <param name="serviceCategoryId"></param>
        /// <param name="categoryTypeId"></param>
        /// <returns></returns>
        public bool CheckIfCategoryTypeInServiceCategory(int serviceCategoryId, int categoryTypeId)
        {
            return _repo.Count(x => x.Parent == new PSSEscortServiceCategory { Id = serviceCategoryId } && x.Id == categoryTypeId) > 0;
        }

        /// <summary>
        /// Gets category types for service category with specified id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<PSSEscortServiceCategoryVM> GetCategoryTypesForServiceCategoryWithId(int id)
        {
            return _repo.GetCategoryTypesForServiceCategoryWithId(id);
        }
    }
}