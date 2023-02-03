using System;

namespace Parkway.CBS.Core.Models
{
    public class APIRequest : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual short CallType {get;set;}

        public virtual Int64 ResourceIdentifier { get; set; }

        public virtual string RequestIdentifier { get; set; }

        public virtual ExpertSystemSettings ExpertSystemSettings { get;set; }
    }
}