using Parkway.CBS.ReferenceData.DataSource.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parkway.CBS.ReferenceData.Configuration;

namespace Parkway.CBS.ReferenceData.DataSource
{
    public class Adapter : BaseReferenceDataSource, IReferenceDataSource
    {
        private readonly IRefDataSystem _customAdapterSystem;

        public Adapter(IRefDataSystem customAdapterSystem = null)
        {
            _customAdapterSystem = customAdapterSystem;
        }

        public string ReferenceDataSourceName() => "Adapter";


        public List<GenericRefDataTemp> GetActiveBillableTaxEntitesPerRevenueHead(int revenueHeadId, RefData refData, dynamic processDetails)
        {
            if(_customAdapterSystem != null)
            {
                return _customAdapterSystem.GetActiveBillableTaxEntitesPerRevenueHead(revenueHeadId, refData, processDetails);
            }
            return new List<GenericRefDataTemp>();
        }
    }
}
