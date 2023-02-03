using Hangfire;
using Parkway.CBS.HangFireInterface.Configuration;
using System;
using System.Configuration;

namespace Parkway.CBS.POSSAP.Services.Implementations.PSSSettlement.Processes.Contracts
{
    public class ProcessComp : IProcessComp
    {
        public void StartHangFireService()
        {
            string conStringName = ConfigurationManager.AppSettings["HangfireConnectionStringName"];

            if (string.IsNullOrEmpty(conStringName))
            {
                throw new Exception("Unable to get the hangfire connection string name");
            }

            //Call the Hangfire storage
            GlobalConfiguration.Configuration.UseSqlServerStorage(HangFireScheduler.GetConnectionString(conStringName));
        }


        /// <summary>
        /// Checks if batch status is equal to expected status
        /// </summary>
        /// <param name="batchStatus"></param>
        /// <param name="ExpectedStatus"></param>
        /// <returns></returns>

        public static bool CheckBatchStatus(Police.Core.Models.Enums.PSSSettlementBatchStatus batchStatus, Police.Core.Models.Enums.PSSSettlementBatchStatus ExpectedStatus)
        {
            return batchStatus == ExpectedStatus;
        }

    }
}
