using System;
using Parkway.CBS.Core.Models;


namespace Parkway.CBS.Police.Core.Models
{
    public class AdminRequestActivityLog : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual string ActionName { get; set; }

        public virtual string Message { get; set; }

        public virtual PSService Service { get; set; }

        public virtual PSSRequest Request { get; set; }

        public virtual TaxEntity TaxEntity { get; set; }

        public virtual PSSAdminUsers ActionByAdminUser { get; set; }

        /// <summary>
        /// the command the admin user was in when this activity was made
        /// </summary>
        public virtual Command CommandTimeStamp { get; set; }

        /// <summary>
        /// Definition level when the request when this activity was made
        /// </summary>
        public virtual PSServiceRequestFlowDefinitionLevel FlowDefinitionLevelTimeStamp { get; set; }
    }
}