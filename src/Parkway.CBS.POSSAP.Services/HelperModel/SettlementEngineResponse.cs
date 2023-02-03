using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.POSSAP.Services.HelperModel
{
    public class SettlementEngineResponse
    {
        /// <summary>
        /// Does the response have any errors.
        /// <para>If the response has any error the bool value is true.</para>
        /// </summary>
        public bool Error { get; set; }
        /// <summary>
        /// Error code
        /// </summary>
        public string ErrorCode { get; set; }
        /// <summary>
        /// Error ErrorMessage
        /// </summary>
        public string ErrorMessage { get; set; }
        /// <summary>
        /// </summary>
        public dynamic ResponseObject { get; set; }
    }
}