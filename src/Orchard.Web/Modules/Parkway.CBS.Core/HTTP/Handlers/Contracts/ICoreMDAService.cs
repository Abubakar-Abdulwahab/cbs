using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using System.Collections.Generic;
using System.Web;
using Orchard.Users.Models;

namespace Parkway.CBS.Core.HTTP.Handlers.Contracts
{
    public interface ICoreMDAService : IDependency
    {
        #region Create


        /// <summary>
        /// Try create MDA
        /// </summary>
        /// <param name="expertSystem"></param>
        /// <param name="mda"></param>
        /// <param name="errors"></param>
        /// <param name="files"></param>
        /// <returns>MDACreatedModel</returns>
        MDACreatedModel TrySaveMDA(ExpertSystemSettings expertSystem, MDA model, UserPartRecord user, ref List<ErrorModel> errors, HttpFileCollectionBase files, string requestReference = null);


        #endregion

        #region Edit

        /// <summary>
        /// Try edit MDA record
        /// </summary>
        /// <param name="expertSystem"></param>
        /// <param name="updatedMDA">Updated model</param>
        /// <param name="mdaId"></param>
        /// <param name="user"></param>
        /// <param name="errors"></param>
        /// <param name="files"></param>
        /// <returns>MDAEditedModel</returns>
        MDAEditedModel TryUpdate(ExpertSystemSettings expertSystem, MDA updatedMDA, int mdaId, UserPartRecord user,  ref List<ErrorModel> errors, HttpFileCollectionBase files, string mdaSlug = "");

        #endregion
    }
}
