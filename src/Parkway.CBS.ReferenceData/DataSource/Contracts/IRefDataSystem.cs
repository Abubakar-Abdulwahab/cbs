using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parkway.CBS.ReferenceData.Configuration;

namespace Parkway.CBS.ReferenceData.DataSource.Contracts
{
    public interface IRefDataSystem
    {
        List<GenericRefDataTemp> GetActiveBillableTaxEntitesPerRevenueHead(int revenueHeadId, RefData refData, dynamic processDetails);
    }
}
