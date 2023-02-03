using Orchard.Data;
using NHibernate.Cfg;
using Orchard.Utility;
using FluentNHibernate.Cfg;
using Parkway.CBS.Core.Models;
using FluentNHibernate.Automapping;

namespace Parkway.CBS.Core.Events
{
    public class TaxEntityProfileLocationNHibernatePersistenceConfig : ISessionConfigurationEvents
    {
        public void Building(Configuration cfg)
        { }

        public void ComputingHash(Hash hash) { }

        public void Created(FluentConfiguration cfg, AutoPersistenceModel defaultModel)
        {
            defaultModel.Override<TaxEntityProfileLocation>(mapping => mapping.Map(x => x.Code, nameof(TaxEntityProfileLocation.Code)).Column(nameof(TaxEntityProfileLocation.Code)).ReadOnly());
        }

        public void Finished(Configuration cfg) { }

        public void Prepared(FluentConfiguration cfg) { }
    }
}