using Orchard.Users.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models
{
    public class TaxClearanceCertificateRequestApprovalLog : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual TaxClearanceCertificateRequest Request { get; set; }

        /// <summary>
        /// <see cref="Enums.TCCRequestStatus"/>
        /// </summary>
        public virtual int Status { get; set; }

        /// <summary>
        /// <see cref="Enums.TCCApprovalLevel"/>
        /// </summary>
        public virtual int ApprovalLevelId { get; set; }

        public virtual UserPartRecord AddedByAdminUser { get; set; }

        public virtual string Comment { get; set; }
    }
}