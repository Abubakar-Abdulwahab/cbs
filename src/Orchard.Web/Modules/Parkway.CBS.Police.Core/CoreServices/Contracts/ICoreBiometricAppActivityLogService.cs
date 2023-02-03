using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Police.Core.CoreServices.Contracts
{
    public interface ICoreBiometricAppActivityLogService : IDependency
    {
        void LogBiometricAppDetail(HttpRequestMessage httpRequest, string IP, out string appVersion);
    }
}
