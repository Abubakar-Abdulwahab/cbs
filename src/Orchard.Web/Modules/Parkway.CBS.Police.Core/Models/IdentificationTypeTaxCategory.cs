using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class IdentificationTypeTaxCategory : CBSModel
    {
        public virtual IdentificationType IdentificationType { get; set; }

        public virtual TaxEntityCategory TaxCategory { get; set; }
    }
}