using System.Collections.Generic;

namespace Parkway.CBS.Core.HelperModels
{
    public class StateModelVM
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<LGAVM> LGAs { get; set; }
    }
}