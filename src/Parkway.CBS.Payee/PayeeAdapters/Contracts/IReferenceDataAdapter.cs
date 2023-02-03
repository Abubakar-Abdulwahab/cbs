using Parkway.CBS.Payee.Models;
using Parkway.CBS.Payee.PayeeAdapters.IPPIS;
using Parkway.CBS.Payee.PayeeAdapters.ReferenceData;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Payee.PayeeAdapters.Contracts
{

    public interface IReferenceDataAdapter
    {
        ReferenceDataResponse GetReferenceDataResponseModels(string filePath);
    }
}
