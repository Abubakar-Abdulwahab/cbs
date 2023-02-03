using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Module.API.Controllers.Handlers.Contracts
{
    public interface IAPIRevenueHeadHandler : IDependency
    {
        APIResponse CreateRevenueHead(RevenueHeadController callBack, CreateRevenueHeadRequestModel model, dynamic headerParams = null);

        /// <summary>
        /// Edit revenue head
        /// </summary>
        /// <param name="integrationController"></param>
        /// <param name="model"></param>
        /// <param name="p"></param>
        /// <returns>APIResponse</returns>
        APIResponse EditRevenueHead(RevenueHeadController callBack, EditRevenueHeadModel model, dynamic headerParams = null);
    }
}
