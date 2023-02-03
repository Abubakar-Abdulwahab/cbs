using System;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class PSSRequestInvoice : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual Invoice Invoice { get; set; }

        public virtual PSSRequest Request { get; set; }
    }
}