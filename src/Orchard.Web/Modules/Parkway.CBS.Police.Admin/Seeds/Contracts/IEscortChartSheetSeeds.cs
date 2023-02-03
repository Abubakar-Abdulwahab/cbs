using Orchard;
using Parkway.CBS.Police.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Police.Admin.Seeds.Contracts
{
    public interface IEscortChartSheetSeeds : IDependency
    {
        /// <summary>
        /// Upload escort chart sheet rates
        /// </summary>
        /// <returns></returns>
        EscortChartSheetStatVM UploadChartSheet();

        /// <summary>
        /// Process escort chart sheet rates
        /// </summary>
        /// <returns>bool</returns>
        bool ProcessChartSheet();
    }
}
