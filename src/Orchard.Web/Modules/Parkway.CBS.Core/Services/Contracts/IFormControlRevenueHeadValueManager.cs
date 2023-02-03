using Orchard;
using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface IFormControlRevenueHeadValueManager<FormControlRevenueHeadValue> : IDependency, IBaseManager<FormControlRevenueHeadValue>
    {
        /// <summary>
        /// Get the revenue head form control values for a particular invoice
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        List<FormControlRevenueHeadValueVM> GetRevenueHeadFormControlValues(Int64 invoiceId);
    }
}
