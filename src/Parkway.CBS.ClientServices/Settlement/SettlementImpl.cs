using Parkway.CBS.ClientServices.Loggers;

namespace Parkway.CBS.ClientServices.Settlement
{
    public class SettlementImpl : Contracts.Settlement
    {
        private Logger Logger;

        public SettlementImpl()
        {
            Logger = LoggerImpl.GetLogger;
        }


        /// <summary>
        /// In this method we start the settlement process for the day
        /// <para>This process gathers all the settlement rules for the day before</para>
        /// </summary>
        /// <param name="tenantName"></param>
        public void DoSettlements(string tenantName)
        {
            Logger.Info("Logging", tenantName);
            Logger.Error("Logging", tenantName);
        }
    }
}
