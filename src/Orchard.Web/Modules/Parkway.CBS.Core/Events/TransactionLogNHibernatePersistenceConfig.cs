using Orchard.Data;
using NHibernate.Cfg;
using Orchard.Utility;
using FluentNHibernate.Cfg;
using Parkway.CBS.Core.Models;
using FluentNHibernate.Automapping;

namespace Parkway.CBS.Core.Events
{
    public class TransactionLogNHibernatePersistenceConfig : ISessionConfigurationEvents
    {
        public void Building(Configuration cfg)
        { }

        public void ComputingHash(Hash hash) { }

        public void Created(FluentConfiguration cfg, AutoPersistenceModel defaultModel)
        {
            defaultModel.Override<TransactionLog>(mapping => mapping.Map(x => x.CompositeUniquePaymentReference, "CompositeUniquePaymentReference").Column("CompositeUniquePaymentReference").ReadOnly());
        }

        public void Finished(Configuration cfg) { }

        public void Prepared(FluentConfiguration cfg) { }
    }
}