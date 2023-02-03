using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using NHibernate.Cfg;
using Orchard.Data;
using Orchard.Utility;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Events
{
    public class CharacterCertificateBiometricsNHibernatePersistenceConfig : ISessionConfigurationEvents
    {
        public void Building(Configuration cfg)
        { }

        public void ComputingHash(Hash hash) { }

        public void Created(FluentConfiguration cfg, AutoPersistenceModel defaultModel)
        {
            defaultModel.Override<CharacterCertificateBiometrics>(mapping => mapping.Map(x => x.PassportImage, "PassportImage").Length(int.MaxValue));
            defaultModel.Override<CharacterCertificateBiometrics>(mapping => mapping.Map(x => x.RightThumb, "RightThumb").Length(int.MaxValue));
            defaultModel.Override<CharacterCertificateBiometrics>(mapping => mapping.Map(x => x.RightIndex, "RightIndex").Length(int.MaxValue));
            defaultModel.Override<CharacterCertificateBiometrics>(mapping => mapping.Map(x => x.RightMiddle, "RightMiddle").Length(int.MaxValue));
            defaultModel.Override<CharacterCertificateBiometrics>(mapping => mapping.Map(x => x.RightRing, "RightRing").Length(int.MaxValue));
            defaultModel.Override<CharacterCertificateBiometrics>(mapping => mapping.Map(x => x.RightPinky, "RightPinky").Length(int.MaxValue));
            defaultModel.Override<CharacterCertificateBiometrics>(mapping => mapping.Map(x => x.LeftThumb, "LeftThumb").Length(int.MaxValue));
            defaultModel.Override<CharacterCertificateBiometrics>(mapping => mapping.Map(x => x.LeftIndex, "LeftIndex").Length(int.MaxValue));
            defaultModel.Override<CharacterCertificateBiometrics>(mapping => mapping.Map(x => x.LeftMiddle, "LeftMiddle").Length(int.MaxValue));
            defaultModel.Override<CharacterCertificateBiometrics>(mapping => mapping.Map(x => x.LeftRing, "LeftRing").Length(int.MaxValue));
            defaultModel.Override<CharacterCertificateBiometrics>(mapping => mapping.Map(x => x.LeftPinky, "LeftPinky").Length(int.MaxValue));
        }

        public void Finished(Configuration cfg) { }

        public void Prepared(FluentConfiguration cfg) { }

    }
}