using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Module.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Module.Controllers.Handlers.Contracts
{
    public interface IFormHandler : IDependency
    {
        /// <summary>
        /// Get view for form setup
        /// </summary>
        /// <param name="revenueHeadId"></param>
        /// <param name="revenueHeadSlug"></param>
        /// <returns>FormSetupViewModel</returns>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        /// <exception cref="CannotFindRevenueHeadException"></exception>
        FormSetupViewModel GetFormSetupView(int revenueHeadId, string revenueHeadSlug);

        /// <summary>
        /// Create and save the form controls
        /// </summary>
        /// <param name="formController"></param>
        /// <param name="controlCollection"></param>
        /// <param name="revenueHeadId"></param>
        /// <param name="revenueHeadSlug"></param>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        void TrySaveFormDetails(FormController formController, ICollection<FormControlViewModel> controlCollection, int revenueHeadId, string revenueHeadSlug);

        /// <summary>
        /// Get view for form setup edit
        /// </summary>
        /// <param name="revenueHeadId"></param>
        /// <param name="revenueHeadSlug"></param>
        FormSetupEditModel GetEditFormSetupView(int revenueHeadId, string revenueHeadSlug, out List<FormControlRevenueHeadMetaDataExtended> alreadyExistingControls);

        /// <summary>
        /// Save or update form controls
        /// </summary>
        /// <param name="formController"></param>
        /// <param name="controlCollection"></param>
        /// <param name="revenueHeadId"></param>
        /// <param name="revenueHeadSlug"></param>
        void TryUpdateFormDetails( ICollection<FormControlViewModel> controlCollection, int revenueHeadId, string revenueHeadSlug);

        
    }
}
