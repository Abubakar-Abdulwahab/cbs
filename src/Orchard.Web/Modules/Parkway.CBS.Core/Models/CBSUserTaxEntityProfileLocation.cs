using System;

namespace Parkway.CBS.Core.Models
{
    public class CBSUserTaxEntityProfileLocation : CBSModel
    {
        public virtual CBSUser CBSUser { get; set; }

        public virtual TaxEntityProfileLocation TaxEntityProfileLocation { get; set; }
    }
}