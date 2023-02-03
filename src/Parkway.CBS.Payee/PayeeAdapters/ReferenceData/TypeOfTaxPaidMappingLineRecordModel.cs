using Parkway.CBS.Payee.ReferenceDataImplementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Payee.PayeeAdapters.ReferenceData
{
    public class TypeOfTaxPaidMappingLineRecordModel
    {
        public int SerialNumberId { get; internal set; }

        public int ReferenceDataTypeOfTaxPaid { get; internal set; }

        public int BatchId { get; internal set; }

    }
}
