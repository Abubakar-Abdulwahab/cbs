using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models
{
    public class ReferenceDataBuildingProperties : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual ReferenceDataAsset ReferenceDataAsset { get; set; }

        public virtual int Purpose { get; set; }

        public virtual int Structure { get; set; }

        public virtual string Address { get; set; }

        public virtual decimal RentAmount { get; set; }

        public virtual LGA LGA { get; set; }
    }
}