using Orchard;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface IFormControlRevenueHeadManager<FormControlRevenueHead> : IDependency, IBaseManager<FormControlRevenueHead>
    {

        /// <summary>
        /// Get the form controls for the given params
        /// </summary>
        /// <param name="revenueHeadId"></param>
        /// <param name="categoryId"></param>
        IEnumerable<FormControlViewModel> GetDBForms(int revenueHeadId, int categoryId);

    }
}
