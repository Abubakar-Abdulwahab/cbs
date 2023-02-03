using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ClientFileServices.Implementations.Contracts
{
    public interface IFileProcessor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantName"></param>
        /// <param name="filePath"></param>
        void ProcessFile(string tenantName, string filePath, long batchId);
    }
}
