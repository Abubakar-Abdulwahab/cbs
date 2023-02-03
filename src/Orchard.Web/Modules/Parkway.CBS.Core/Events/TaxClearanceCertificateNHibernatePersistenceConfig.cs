using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using NHibernate.Cfg;
using Orchard.Data;
using Orchard.Utility;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.Events
{
    public class TaxClearanceCertificateNHibernatePersistenceConfig : ISessionConfigurationEvents
    {
        public void Building(Configuration cfg)
    { }

    public void ComputingHash(Hash hash) { }

    public void Created(FluentConfiguration cfg, AutoPersistenceModel defaultModel)
    {
        defaultModel.Override<TaxClearanceCertificate>(mapping => mapping.Map(x => x.RevenueOfficerSignatureBlob, "RevenueOfficerSignatureBlob").Length(int.MaxValue));
        defaultModel.Override<TaxClearanceCertificate>(mapping => mapping.Map(x => x.DirectorOfRevenueSignatureBlob, "DirectorOfRevenueSignatureBlob").Length(int.MaxValue));
    }

    public void Finished(Configuration cfg) { }

    public void Prepared(FluentConfiguration cfg) { }
    }
}