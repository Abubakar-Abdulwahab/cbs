using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.HTTP.Handlers.Contracts
{
    public interface ICoreNIBSSIntegrationService : IDependency
    {
        /// <summary>
        /// Gets the NIBSS IV and SecretKey
        /// </summary>
        /// <returns></returns>
        void GetNibssIntegrationCredential(ref string IV, ref string SecretKey);
    }
}
