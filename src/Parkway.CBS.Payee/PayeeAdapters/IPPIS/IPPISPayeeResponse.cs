using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Payee.PayeeAdapters.IPPIS
{

    /// <summary>
    /// Response to get paye response
    /// </summary>
    public class IPPISPayeeResponse : GetPayeResponse
    {
        /// <summary>
        /// if HeaderValidateObject.Error is false, get the list of PayeeAssessmentLineRecordModel
        /// </summary>
        public ConcurrentStack<IPPISAssessmentLineRecordModel> Payees { get; set; }
    }
}
