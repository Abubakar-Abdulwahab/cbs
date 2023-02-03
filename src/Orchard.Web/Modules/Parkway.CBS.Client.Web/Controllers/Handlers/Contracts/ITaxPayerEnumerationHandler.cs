using Orchard;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;
using System.Web;

namespace Parkway.CBS.Client.Web.Controllers.Handlers.Contracts
{
    public interface ITaxPayerEnumerationHandler : IDependency
    {
        /// <summary>
        /// Gets States and LGAs
        /// </summary>
        /// <returns></returns>
        List<Core.Models.StateModel> GetStatesAndLgas();

        /// <summary>
        /// Process enumeration line items from on screen form.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="userModel"></param>
        /// <returns>BatchToken</returns>
        string ProcessEnumerationItemsForOnScreenForm(ICollection<TaxPayerEnumerationLine> items, UserDetailsModel userModel);

        /// <summary>
        /// Process enumeration line items for file upload.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="userModel"></param>
        /// <returns></returns>
        string ProcessEnumerationItemsForFileUpload(HttpPostedFileBase file, UserDetailsModel userModel);

        /// <summary>
        /// Checks for the completion status of the enumeration batch line items upload.
        /// </summary>
        /// <param name="batchToken"></param>
        /// <param name="taxEntityId"></param>
        /// <returns></returns>
        APIResponse CheckIfEnumerationUploadIsCompleted(string batchToken, long taxEntityId);

        /// <summary>
        /// Get enumeration line items for enumeration batch with id embedded in batch token
        /// </summary>
        /// <param name="batchToken"></param>
        /// <param name="taxEntityId"></param>
        /// <returns></returns>
        APIResponse GetLineItemsForEnumerationWithId(string batchToken, long taxEntityId);

        /// <summary>
        /// Get paged line items for enumeration batch with specified id embedded in provided batch token.
        /// </summary>
        /// <param name="batchToken"></param>
        /// <param name="taxEntityId"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        APIResponse GetPagedLineItemsForEnumerationWithId(string batchToken, long taxEntityId, int page);
    }
}
