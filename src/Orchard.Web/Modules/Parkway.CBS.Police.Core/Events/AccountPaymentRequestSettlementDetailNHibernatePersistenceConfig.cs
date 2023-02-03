using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using NHibernate.Cfg;
using Orchard.Data;
using Orchard.Utility;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Events
{
    public class AccountPaymentRequestSettlementDetailNHibernatePersistenceConfig : ISessionConfigurationEvents
    {
        public void Building(Configuration cfg)
        { }

        public void ComputingHash(Hash hash) { }

        public void Created(FluentConfiguration cfg, AutoPersistenceModel defaultModel)
        {
            defaultModel.Override<AccountPaymentRequestSettlementDetail>(mapping => mapping.Map(x => x.SettlementEngineRequestJSON, nameof(AccountPaymentRequestSettlementDetail.SettlementEngineRequestJSON)).Length(4000));
            defaultModel.Override<AccountPaymentRequestSettlementDetail>(mapping => mapping.Map(x => x.SettlementEngineResponseJSON, nameof(AccountPaymentRequestSettlementDetail.SettlementEngineResponseJSON)).Length(4000));
        }

        public void Finished(Configuration cfg) { }

        public void Prepared(FluentConfiguration cfg) { }

    }
}