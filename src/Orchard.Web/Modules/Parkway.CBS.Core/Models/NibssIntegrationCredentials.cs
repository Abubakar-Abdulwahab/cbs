using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models
{
    public class NibssIntegrationCredentials : CBSModel
    {
        /// <summary>
        /// String IV encryption credential for Nibss
        /// </summary>
        public virtual string IV { get; set; }

        /// <summary>
        /// String SecretKey encryption credential for Nibss
        /// </summary>
        public virtual string SecretKey { get; set; }

        /// <summary>
        /// Bool value to indicate if record is active or not
        /// </summary>
        public virtual bool IsActive { get; set; }
    }
}