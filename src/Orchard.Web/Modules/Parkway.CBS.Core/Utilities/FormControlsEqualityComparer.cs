using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Core.Utilities
{
    internal class FormHelperModelEqualityComparer : IEqualityComparer<FormHelperModel>
    {
        private List<FormHelperModel> duplicates;

        public FormHelperModelEqualityComparer()
        {
            duplicates = new List<FormHelperModel>();
        }

        public List<FormHelperModel> GetDuplicates()
        {
            return duplicates;
        }

        public bool Equals(FormHelperModel x, FormHelperModel y)
        {
            bool sameCat = x.TaxEntityCategory.Identifier == y.TaxEntityCategory.Identifier;
            bool sameName = x.FormControls.Name == y.FormControls.Name;
            var isTrue = sameName && sameCat;
            if (isTrue) { duplicates.Add(y); }
            return (isTrue);
        }

        public int GetHashCode(FormHelperModel obj)
        {
            return obj.FormControls.TechnicalName.GetHashCode();
        }
    }
}