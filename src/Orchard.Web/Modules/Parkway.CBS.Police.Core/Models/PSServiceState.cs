using Parkway.CBS.Core.Models;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Models
{
    public class PSServiceState : CBSModel
    {
        public virtual StateModel State { get; set; }

        public virtual PSService Service { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual ICollection<PSServiceStateCommand> Commands { get; set; }

    }
}