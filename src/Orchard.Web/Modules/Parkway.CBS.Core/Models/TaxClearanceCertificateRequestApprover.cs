using Orchard.Users.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Parkway.CBS.Core.Models.Enums;


namespace Parkway.CBS.Core.Models
{
    public class TaxClearanceCertificateRequestApprover : CBSModel
    {
        /// <summary>
        /// this value will hold the user that has been asssigned to perform the
        /// action value 
        /// </summary>
        public virtual UserPartRecord AssignedApprover { get; set; }

        /// <summary>
        /// <see cref="TCCApprovalLevel"/>
        /// </summary>
        public virtual int ApprovalLevelId { get; set; }

        /// <summary>
        /// Contact email for this approval level
        /// </summary>
        public virtual string ContactEmail { get; set; }

        /// <summary>
        /// Contact phone number for this approval level
        /// </summary>
        public virtual string ContactPhoneNumber { get; set; }


    }
}