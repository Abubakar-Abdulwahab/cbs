using System.Collections.Generic;

namespace Parkway.CBS.Core.Models
{
    public class StateModel : CBSModel
    {
        public virtual string Name { get; set; }

        public virtual string ShortName { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual ICollection<LGA> LGAs { get; set; }
    }
}