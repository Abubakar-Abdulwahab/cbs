using Parkway.CBS.Payee.PayeeAdapters.IPPIS;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Payee.Models
{
    public class IPPISBatchGroup : BatchGroup
    {
        /// <summary>
        /// if HeaderValidateObject.Error is false, get the list of PayeeAssessmentLineRecordModel
        /// </summary>
        public List<IPPISAssessmentLineRecordModel> Payees { get; set; }
    }
}
