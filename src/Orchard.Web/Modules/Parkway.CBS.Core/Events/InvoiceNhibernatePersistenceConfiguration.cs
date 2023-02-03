using Orchard.Data;
using NHibernate.Cfg;
using Orchard.Utility;
using FluentNHibernate.Cfg;
using Parkway.CBS.Core.Models;
using FluentNHibernate.Automapping;

namespace Parkway.CBS.Core.Events
{
    public class InvoiceNhibernatePersistenceConfiguration : ISessionConfigurationEvents
    {
        public void Building(Configuration cfg)
        { }

        public void ComputingHash(Hash hash) { }

        public void Created(FluentConfiguration cfg, AutoPersistenceModel defaultModel)
        {
            defaultModel.Override<Invoice>(mapping => mapping.References(x => x.InvoiceAmountDueSummary).ReadOnly());
        }

        public void Finished(Configuration cfg) { }

        public void Prepared(FluentNHibernate.Cfg.FluentConfiguration cfg) { }
    }
}