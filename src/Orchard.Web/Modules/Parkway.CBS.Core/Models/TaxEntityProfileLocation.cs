using System;

namespace Parkway.CBS.Core.Models
{
    public class TaxEntityProfileLocation : CBSModel
    {
        public virtual string Name { get; set; }

        public virtual StateModel State { get; set; }

        public virtual LGA LGA { get; set; }

        public virtual string Address { get; set; }

        public virtual TaxEntity TaxEntity { get; set; }

        public virtual bool IsDefault { get; set; }

        public virtual string Code { get; set; }
    }
}