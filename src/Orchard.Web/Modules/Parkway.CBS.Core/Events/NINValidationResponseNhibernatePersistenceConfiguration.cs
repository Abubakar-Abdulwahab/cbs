using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using NHibernate.Cfg;
using Orchard.Data;
using Orchard.Utility;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.Events
{
    public class NINValidationResponseNhibernatePersistenceConfiguration : ISessionConfigurationEvents
    {
        public void Building(Configuration cfg) { }

        public void ComputingHash(Hash hash) { }

        public void Created(FluentConfiguration cfg, AutoPersistenceModel defaultModel)
        {
            defaultModel.Override<NINValidationResponse>(mapping => mapping.Map(x => x.Photo).Length(int.MaxValue));
            defaultModel.Override<NINValidationResponse>(mapping => mapping.Map(x => x.Signature).Length(int.MaxValue));
            defaultModel.Override<NINValidationResponse>(mapping => mapping.Map(x => x.ResponseDump).Length(int.MaxValue));
        }

        public void Finished(Configuration cfg) { }

        public void Prepared(FluentConfiguration cfg) { }
    }
}