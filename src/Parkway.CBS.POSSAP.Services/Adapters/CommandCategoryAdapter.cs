using System;
using Parkway.CBS.POSSAP.Services.Adapters.Contracts;

namespace Parkway.CBS.POSSAP.Services.Adapters
{
    public class CommandCategoryAdapter : IPOSSAPExternalDataSourceImplementation
    {

        /// <summary>
        /// Start the processing for getting data the spcificed external
        /// source
        /// </summary>
        /// <param name="tenantName"></param>
        public void StartProceesForExternalDataSource(string tenantName)
        {
            //we need to log the call
            //lets start a process log for this call
            throw new NotImplementedException();
        }


    }
}
