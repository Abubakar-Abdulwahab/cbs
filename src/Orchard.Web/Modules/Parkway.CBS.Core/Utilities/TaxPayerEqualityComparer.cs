using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Utilities
{
    public class TaxPayerEqualityComparer : IEqualityComparer<TaxEntity>
    {
        private List<TaxEntity> duplicates;
        public TaxPayerEqualityComparer()
        {
            duplicates = new List<TaxEntity>();
        }

        public List<TaxEntity> GetDuplicates()
        {
            return duplicates;
        }

        public bool Equals(TaxEntity x, TaxEntity y)
        {
            bool result = x.TaxPayerIdentificationNumber == y.TaxPayerIdentificationNumber;
            if (result) { duplicates.Add(y); }
            return result;
        }

        public int GetHashCode(TaxEntity obj)
        {
            return obj.TaxPayerIdentificationNumber.GetHashCode();
        }
    }
}