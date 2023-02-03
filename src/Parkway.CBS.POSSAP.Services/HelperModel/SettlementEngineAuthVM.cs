using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.POSSAP.Services.HelperModel
{
    public class SettlementEngineAuthVM
    {
        public string ClientCode { get; set; }

        /// <summary>
        /// HMAC 256 of client code, hash with secret
        /// </summary>
        public string hmac { get; set; }
    }
}
