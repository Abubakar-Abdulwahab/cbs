using Parkway.CBS.Payee.ReferenceDataImplementation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Payee.PayeeAdapters.ReferenceData
{
    public class ReferenceDataResponse
    {
        /// <summary>
        /// check to see if there are any errors here, that is if the header values have been read to be in correct order
        /// </summary>
        public HeaderValidateObject HeaderValidateObject { get; set; }

        /// <summary>
        /// if HeaderValidateObject.Error is false, get the list of PayeeAssessmentLineRecordModel
        /// </summary>
        public List<ReferenceDataLineRecordModel> ReferenceDataLineRecordss { get; set; }

        /// <summary>
        /// if HeaderValidateObject.Error is false, get the list of PayeeAssessmentLineRecordModel
        /// </summary>
        public ConcurrentStack<ReferenceDataLineRecordModel> ReferenceDataLineRecords { get; set; }

    }
}
