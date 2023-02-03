using Parkway.CBS.ReferenceData.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ReferenceData.DataSource.Contracts
{
    public interface IReferenceDataSource
    {

        /// <summary>
        /// Source name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>string</returns>
        string ReferenceDataSourceName();


        List<GenericRefDataTemp> GetActiveBillableTaxEntitesPerRevenueHead(int revenueHeadId, RefData refData, dynamic processDetails);
    }
}
