using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models
{
    public class ReferenceDataBusinessPremises : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual ReferenceDataAsset ReferenceDataAsset { get; set; }

        public virtual int OrganizationType { get; set; }

        public virtual DateTime CommencementDate { get; set; }

        public virtual int NoofEmployees { get; set; }
    }
}