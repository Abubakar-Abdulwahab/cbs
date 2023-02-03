using Orchard.Data;
using NHibernate.Cfg;
using Orchard.Utility;
using FluentNHibernate.Cfg;
using FluentNHibernate.Automapping;
using Parkway.CBS.OSGOF.Admin.Models;

namespace Parkway.CBS.OSGOF.Admin.Events
{
    /// <summary>
    /// Mapping hook for 
    /// <see cref="CellSiteClientPaymentBatch"/>
    /// </summary>
    public class CellSiteClientPaymentBatchNhibernatePersistenceConfiguration : ISessionConfigurationEvents
    {
        public void Building(Configuration cfg)
        { }

        public void ComputingHash(Hash hash) { }

        public void Created(FluentConfiguration cfg, AutoPersistenceModel defaultModel)
        {
            defaultModel.Override<CellSiteClientPaymentBatch>(mapping => mapping.Map(x => x.BatchRef, "BatchRef").Column("BatchRef").ReadOnly());
            //defaultModel.Override<CellSiteClientPaymentBatch>(mapping => mapping.ReadOnly(,).References(x => x.BatchRef).ReadOnly());
        }

        public void Finished(Configuration cfg) { }

        public void Prepared(FluentConfiguration cfg) { }
    }
}