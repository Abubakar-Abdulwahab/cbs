using System;
using Orchard.Logging;
using System.Collections.Generic;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.PSSServiceType.CharacterCertificateOptions.Contracts;

namespace Parkway.CBS.Police.Core.CoreServices
{
    public class CoreCharacterCertificateOptions : ICoreCharacterCertificateOptions
    {
        private readonly IEnumerable<Lazy<ICharacterCertificateOptions>> _optionsImpl;

        public ILogger Logger { get; set; }

        public CoreCharacterCertificateOptions(IEnumerable<Lazy<ICharacterCertificateOptions>> optionsImpl)
        {
            _optionsImpl = optionsImpl;
            Logger = NullLogger.Instance;
        }

    }
}