using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models
{
    public class Asset : CBSBaseModel
    {
        public virtual long Id { get; set; }
        public virtual TINRegistrationForm TIN { get; set; }
        public virtual string TypeOfAsset { get; set; }
        public virtual decimal MarketValue { get; set; }
        public virtual DateTime OwnershipDate { get; set; }
        public virtual string LocationOfAsset { get; set; }
    }
}