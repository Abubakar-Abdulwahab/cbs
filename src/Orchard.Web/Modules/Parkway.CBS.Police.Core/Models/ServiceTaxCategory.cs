using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class ServiceTaxCategory : CBSModel
    {
        public virtual PSService Service { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual TaxEntityCategory TaxCategory { get; set; }

    }
}