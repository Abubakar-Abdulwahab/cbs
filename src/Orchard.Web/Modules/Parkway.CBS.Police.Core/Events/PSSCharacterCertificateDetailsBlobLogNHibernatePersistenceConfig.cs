using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using NHibernate.Cfg;
using Orchard.Data;
using Orchard.Utility;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Events
{

    public class PSSCharacterCertificateDetailsBlobLogNHibernatePersistenceConfig : ISessionConfigurationEvents
    {
        public void Building(Configuration cfg)
        { }

        public void ComputingHash(Hash hash) { }

        public void Created(FluentConfiguration cfg, AutoPersistenceModel defaultModel)
        {
            defaultModel.Override<PSSCharacterCertificateDetailsBlobLog>(mapping => mapping.Map(x => x.PassportPhotographBlob, "PassportPhotographBlob").Length(int.MaxValue));
            defaultModel.Override<PSSCharacterCertificateDetailsBlobLog>(mapping => mapping.Map(x => x.InternationalPassportDataPageBlob, "InternationalPassportDataPageBlob").Length(int.MaxValue));
            defaultModel.Override<PSSCharacterCertificateDetailsBlobLog>(mapping => mapping.Map(x => x.SignatureBlob, "SignatureBlob").Length(int.MaxValue));
        }

        public void Finished(Configuration cfg) { }

        public void Prepared(FluentConfiguration cfg) { }

    }
}