using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.Models
{
    public class PoliceCollectionLog : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual TransactionLog TransactionLog { get; set; }

        public virtual PSSRequest Request { get; set; }
    }
}