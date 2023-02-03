using Orchard;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Parkway.CBS.Core.Models.Enums;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface IRefDataTempManager<RefDataTemp> : IDependency//, IBaseManager<RefDataTemp>
    {
        /// <summary>
        /// Save ref data temporarily
        /// </summary>
        /// <param name="refDataTempDataTable"></param>
        void SaveBundle(DataTable refDataTempDataTable, string tableName);


        /// <summary>
        /// Update ref data temp table
        /// </summary>
        /// <param name="columnNameAndValue"></param>
        /// <param name="batchNumber"></param>
        /// <param name="revenueHeadId"></param>
        /// <param name="billingId"></param>
        void UpdateRefDataTemp(Dictionary<string, string> columnNameAndValue, string batchNumber, int revenueHeadId, int billingId);
    }
}
