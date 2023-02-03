using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Entities.DTO
{
    public class StatesAndLGAs
    {
        public int StateId { get; set; }

        public string Name { get; set; }

        public string StateCode { get; set; }

        public List<LGAModel> LGAs { get; set; }

    }
}
