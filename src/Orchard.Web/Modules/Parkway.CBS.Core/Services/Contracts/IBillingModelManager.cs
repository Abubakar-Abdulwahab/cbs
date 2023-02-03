using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface IBillingModelManager<BillingModel> : IDependency, IBaseManager<BillingModel>
    {
        /// <summary>
        /// Get the direct assessment model
        /// </summary>
        /// <param name="billingId"></param>
        /// <returns>string</returns>
        string GetDirectAssessmentModel(int billingId);

    }
}
