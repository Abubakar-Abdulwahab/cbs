using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class TaxPayersReferenceDataRequestModel
    {
        public List<TaxEntity> TaxPayers { get; set; }
        public int TaxPayerType { get; set; }
        public Int64 Count { get; set; }
        public Int64 BlockSize { get; set; }
        public int Pointer { get; set; }
    }
}