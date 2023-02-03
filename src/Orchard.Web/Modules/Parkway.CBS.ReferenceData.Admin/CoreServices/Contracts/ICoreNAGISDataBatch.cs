using Orchard;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ReferenceData.Admin.CoreServices.Contracts
{
    public interface ICoreNAGISDataBatch : IDependency
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<NagisDataBatch> GetNAGISDataRecords();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        string GetNAGISDataBatchRef(int Id);

    }
}
