using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HTTP.Handlers.Contracts
{
    public interface ICoreIPPISBatchService : IDependency
    {
        /// <summary>
        /// Save IPPIS batch 
        /// </summary>
        /// <param name="iPPISBatch">IPPISBatch</param>
        void SaveProcessBatch(IPPISBatch iPPISBatch);
    }
}