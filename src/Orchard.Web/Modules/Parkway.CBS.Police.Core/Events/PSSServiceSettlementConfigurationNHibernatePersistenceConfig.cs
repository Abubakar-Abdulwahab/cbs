using Orchard.Data;
using NHibernate.Cfg;
using Orchard.Utility;
using FluentNHibernate.Cfg;
using FluentNHibernate.Automapping;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Events
{
    public class PSSServiceSettlementConfigurationNHibernatePersistenceConfig : ISessionConfigurationEvents
    {
        public void Building(Configuration cfg)
        { }

        public void ComputingHash(Hash hash) { }

        public void Created(FluentConfiguration cfg, AutoPersistenceModel defaultModel)
        {
            defaultModel.Override<PSSServiceSettlementConfiguration>(mapping => mapping.Map(x => x.CompositeUniqueValue, nameof(PSSServiceSettlementConfiguration.CompositeUniqueValue)).Column(nameof(PSSServiceSettlementConfiguration.CompositeUniqueValue)).ReadOnly());
        }

        public void Finished(Configuration cfg) { }

        public void Prepared(FluentConfiguration cfg) { }

    }
}