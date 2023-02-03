using Orchard;
using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Module.API.Controllers.Handlers.Contracts
{
    public interface IAPIPAYEValidationHandler : IDependency
    {
        /// <summary>
        /// Validate batch items for a specified batch identifier
        /// </summary>
        /// <param name="model"></param>
        /// <param name="headerParams"></param>
        /// <returns>APIResponse</returns>
        APIResponse ValidateBatchItem(PAYEValidateBatchModel model, dynamic headerParams = null);
    }
}
