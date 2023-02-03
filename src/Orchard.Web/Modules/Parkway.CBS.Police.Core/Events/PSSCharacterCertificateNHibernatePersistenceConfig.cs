using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using NHibernate.Cfg;
using Orchard.Data;
using Orchard.Utility;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Events
{
    public class PSSCharacterCertificateNHibernatePersistenceConfig : ISessionConfigurationEvents
    {
        public void Building(Configuration cfg)
        { }

        public void ComputingHash(Hash hash) { }

        public void Created(FluentConfiguration cfg, AutoPersistenceModel defaultModel)
        {
            defaultModel.Override<PSSCharacterCertificate>(mapping => mapping.Map(x => x.PassportPhotoBlob, "PassportPhotoBlob").Length(int.MaxValue));
            defaultModel.Override<PSSCharacterCertificate>(mapping => mapping.Map(x => x.CentralRegistrarSignatureBlob, "CentralRegistrarSignatureBlob").Length(int.MaxValue));
        }

        public void Finished(Configuration cfg) { }

        public void Prepared(FluentConfiguration cfg) { }

    }
}