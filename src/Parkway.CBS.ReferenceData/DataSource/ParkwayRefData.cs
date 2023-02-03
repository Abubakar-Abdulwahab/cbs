using Parkway.CBS.ReferenceData.DataSource.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parkway.CBS.ReferenceData.Configuration;

namespace Parkway.CBS.ReferenceData.DataSource
{
    public class ParkwayRefData : BaseReferenceDataSource, IReferenceDataSource
    {
        private readonly IRefDataSystem _implementingSystem;

        public ParkwayRefData(IRefDataSystem implementingSystem = null)
        {
            _implementingSystem = implementingSystem;
        }
       
        public string ReferenceDataSourceName() => "Parkway";


        public List<GenericRefDataTemp> GetActiveBillableTaxEntitesPerRevenueHead(int revenueHeadId, RefData refData, dynamic processDetails)
        {
            if(_implementingSystem != null)
            {
                return _implementingSystem.GetActiveBillableTaxEntitesPerRevenueHead(revenueHeadId, refData, processDetails);
            }

            return new List<GenericRefDataTemp>();
        }

    }
}
