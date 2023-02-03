using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Entities.VMs
{
    public class CellSitesFileValidationObject
    {
        public string HeaderErrorMessage { get; set; }

        public bool HeaderHasErrors { get; set; }

        public bool CellsHasErrors { get; set; }

        public string CellsErrorMessage { get; set; }

        public string ScheduleStagingBatchNumber { get; set; }
    }
}
