using Parkway.CBS.ClientFileServices.Implementations.Models;
using Parkway.CBS.Entities.VMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ClientFileServices.Implementations.Contracts
{
    public interface IReferenceDataFileProcessor
    {
        /// <summary>
        /// Do file processing 
        /// </summary>
        /// <returns>ValidateFileResponse object containing the batch Id and other props indicating whether an error has occurred or not</returns>
        /// <exception cref="Exception">Throw an exception if something doesn't get saved or header values are incorrect</exception>
        ValidateFileResponse SaveFile(string tenantName, string filePath, long batchId);
    }
}
