using Hangfire;
using Parkway.CBS.HangFireInterface.Configuration;
using Parkway.CBS.POSSAP.EGSRegularization.Implementations.RecurringInvoice.Contracts;
using System;
using System.Configuration;

namespace Parkway.CBS.POSSAP.EGSRegularization.Implementations.RecurringInvoice
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
    }
}
