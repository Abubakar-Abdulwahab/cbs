using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models
{
    public class PaymentReference : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual Invoice Invoice { get; set; }

        public virtual string InvoiceNumber { get; set; }

        public virtual string ReferenceNumber { get; set; }

        /// <summary>
        /// <see cref="Enums.PaymentProvider"/>
        /// </summary>
        public virtual int PaymentProvider { get; set; }
    }
}