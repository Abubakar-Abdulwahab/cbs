using Orchard;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace Parkway.CBS.Police.Core.CoreServices
{
    public class CoreBiometricAppActivityLogService : ICoreBiometricAppActivityLogService
    {
        private readonly IBiometricAppActivityLogManager<BiometricAppActivityLog> _biometricAppActivityLogManager;

        public CoreBiometricAppActivityLogService(IBiometricAppActivityLogManager<BiometricAppActivityLog> biometricAppActivityLogManager)
        {
            _biometricAppActivityLogManager = biometricAppActivityLogManager;
        }

        public void LogBiometricAppDetail(HttpRequestMessage httpRequest, string IP, out string appVersion)
        {
            appVersion = httpRequest.Headers.GetValues("Version").FirstOrDefault();
            var mac = httpRequest.Headers.GetValues("Mac").FirstOrDefault();
            var ip = HttpContext.Current.Request.UserHostAddress;

            _biometricAppActivityLogManager.Save(new BiometricAppActivityLog
            {
                Version = appVersion,
                MacAddress = mac,
                IPAddress = ip
            });
        }
    }
}