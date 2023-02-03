using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPSSRequestExtractDetailsCategoryManager<PSSRequestExtractDetailsCategory> : IDependency, IBaseManager<PSSRequestExtractDetailsCategory>
    {
        /// <summary>
        /// Gets extract category names for display on extract document
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns></returns>
        IEnumerable<string> GetExtractCategoriesForExtractDocument(string fileRefNumber);


        /// <summary>
        /// Gets selected extract categories and sub categories for request with specified file ref number
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns></returns>
        IEnumerable<PSSRequestExtractDetailsCategoryVM> GetExtractCategoriesForRequest(string fileRefNumber);
    }
}
